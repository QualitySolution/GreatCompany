using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using QS.Launcher.AppRunner;
using QS.Project.DB;
using ReactiveUI;
using System.Reactive;

namespace GreatCompany;

public partial class GreatCompanyApp : Application {
	private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
	private readonly IServiceProvider? startupServices;
	private readonly string? connectionString;
	private readonly string? login;
	private readonly string? sessionId;
	private readonly string? baseTitle;
	private ILifetimeScope? mainContainer;
	private bool isShuttingDown;

	public GreatCompanyApp() : this(null, null, null, null, null) {
	}

	public GreatCompanyApp(IServiceProvider? startupServices,
		string? connectionString, string? login, string? sessionId, string? baseTitle) {
		this.startupServices = startupServices;
		this.connectionString = connectionString;
		this.login = login;
		this.sessionId = sessionId;
		this.baseTitle = baseTitle;
	}

	public override void Initialize() {
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted() {
		if(Design.IsDesignMode) {
			base.OnFrameworkInitializationCompleted();
			return;
		}

		if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			desktop.Exit += (_, _) => DisposeApplicationServices();

			if(string.IsNullOrEmpty(connectionString))
				ShowLauncher(desktop);
			else {
				var mainWindow = CreateMainWindow(connectionString, login, sessionId, baseTitle);
				SetupMainWindowLifetime(desktop, mainWindow);
				desktop.MainWindow = mainWindow;
			}
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void ShowLauncher(IClassicDesktopStyleApplicationLifetime desktop) {
		desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

		var services = startupServices ?? throw new InvalidOperationException("Сервисы лончера не переданы.");
		var launcherWindow = services.GetRequiredService<QS.Launcher.Views.MainWindow>();
		var runner = services.GetRequiredService<InProcessRunner>();
		var previousCallback = runner.OnLogin;
		var loginAccepted = false;

		runner.OnLogin = response => {
			previousCallback?.Invoke(response);

			loginAccepted = true;
			Dispatcher.UIThread.Post(() => {
				var mainWindow = CreateMainWindow(
					response.ConnectionString,
					response.Login,
					response.Parameters.GetValueOrDefault("SessionId"),
					response.Parameters.GetValueOrDefault("BaseTitle"));

				SetupMainWindowLifetime(desktop, mainWindow);
				desktop.MainWindow = mainWindow;
				mainWindow.Show();
				launcherWindow.Close();
			});
		};

		launcherWindow.Closed += (_, _) => {
			if(!loginAccepted)
				ShutdownApplication(desktop);
		};

		launcherWindow.Show();
	}

	private MainWindow CreateMainWindow(string? connString, string? userLogin, string? userSessionId, string? userBaseTitle) {
		if(string.IsNullOrWhiteSpace(connString))
			throw new InvalidOperationException("Строка подключения не установлена.");

		var settings = new DatabaseConnectionSettings(new MySqlConnectionStringBuilder(connString));
		mainContainer?.Dispose(); // вход из лончера повторный: контейнер прошлого сеанса больше не нужен
		mainContainer = CompositionRoot.BuildContainer(
			settings, userLogin ?? string.Empty, userSessionId ?? string.Empty);

		// Исключение в ReactiveCommand по умолчанию роняет приложение — вместо этого лог + сообщение
		var interactiveMessage = mainContainer.Resolve<QS.Dialog.IInteractiveMessage>();
		RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex => {
			logger.Error(ex, "Необработанная ошибка в команде интерфейса.");
			interactiveMessage.ShowMessage(QS.Dialog.ImportanceLevel.Error, ex.Message, "Ошибка");
		});

		DataTemplates.Add(mainContainer.Resolve<QS.Navigation.IAvaloniaViewResolver>());

		// Параметры окна — только именованными: три строковых подряд Autofac по типу не различит
		return mainContainer.Resolve<MainWindow>(
			new NamedParameter("login", userLogin),
			new NamedParameter("sessionId", userSessionId),
			new NamedParameter("baseTitle", userBaseTitle));
	}

	private void SetupMainWindowLifetime(IClassicDesktopStyleApplicationLifetime desktop, MainWindow mainWindow) {
		mainWindow.Closed += (_, _) => ShutdownApplication(desktop);
	}

	private void ShutdownApplication(IClassicDesktopStyleApplicationLifetime desktop) {
		if(isShuttingDown)
			return;

		isShuttingDown = true;
		DisposeApplicationServices();
		desktop.Shutdown();
	}

	// Сервисы лончера освобождает Program — он их и создал
	private void DisposeApplicationServices() {
		mainContainer?.Dispose();
		mainContainer = null;
	}
}
