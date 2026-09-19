using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using QS.Dialog;
using QS.ErrorReporting;
using QS.Launcher.AppRunner;
using QS.Project.DB;

namespace GreatCompany;

public partial class GreatCompanyApp : Application {
	private readonly IServiceProvider? startupServices;
	private readonly CrashReporting? crashReporting;
	private readonly string? connectionString;
	private readonly string? login;
	private readonly string? sessionId;
	private readonly string? baseTitle;
	private ILifetimeScope? mainContainer;
	private bool isShuttingDown;

	public GreatCompanyApp() : this(null, null, null, null, null, null) {
	}

	public GreatCompanyApp(IServiceProvider? startupServices, CrashReporting? crashReporting,
		string? connectionString, string? login, string? sessionId, string? baseTitle) {
		this.startupServices = startupServices;
		this.crashReporting = crashReporting;
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

		// Контейнера ещё нет, разбирать ошибку нечем — но перехват уже нужен: без него
		// падение в фазе лончера или в сборке контейнера закрывает приложение молча
		DispatcherExceptionHandler.Install();
		RxAppExceptionHandler.Install();

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

		runner.OnLogin = response => Dispatcher.UIThread.Post(() => {
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

		// главного окна нет - вход не состоялся или окно не создалось, работать дальше нечему
		launcherWindow.Closed += (_, _) => {
			if(desktop.MainWindow == null)
				ShutdownApplication(desktop);
		};

		launcherWindow.Show();
	}

	private MainWindow CreateMainWindow(string? connString, string? userLogin, string? userSessionId, string? userBaseTitle) {
		if(string.IsNullOrWhiteSpace(connString))
			throw new InvalidOperationException("Строка подключения не установлена.");

		if(string.IsNullOrWhiteSpace(userLogin))
			throw new InvalidOperationException("Логин пользователя не передан.");

		var settings = new DatabaseConnectionSettings(new MySqlConnectionStringBuilder(connString));
		mainContainer?.Dispose(); // вход из лончера повторный: контейнер прошлого сеанса больше не нужен
		mainContainer = CompositionRoot.BuildContainer(
			settings, userLogin, userSessionId ?? string.Empty);

		var errorHandling = mainContainer.Resolve<IErrorHandlingService>();
		DispatcherExceptionHandler.Install(errorHandling);
		RxAppExceptionHandler.Install(errorHandling);
		if(crashReporting != null) {
			crashReporting.Reporter = mainContainer.Resolve<IErrorReporter>();
			crashReporting.Settings = mainContainer.Resolve<IErrorReportingSettings>();
		}

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
