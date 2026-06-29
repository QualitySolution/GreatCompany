using Autofac;
using Autofac.Extensions.DependencyInjection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using QS.Launcher.AppRunner;
using QS.Project;
using QS.Project.DB;
using QS.ViewModels.Resolve;

namespace GreatCompany;

public partial class GreatCompanyApp : Application {
	private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
	private readonly string? connectionString;
	private readonly string? login;
	private readonly string? sessionId;
	private readonly string? baseTitle;
	private ILifetimeScope? mainContainer;
	private bool isShuttingDown;

	public GreatCompanyApp() : this(null, null, null, null) {
	}

	public GreatCompanyApp(string? connectionString, string? login, string? sessionId, string? baseTitle) {
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

		var launcherWindow = Program.StartupServiceProvider.GetRequiredService<QS.Launcher.Views.MainWindow>();
		var runner = Program.StartupServiceProvider.GetRequiredService<InProcessRunner>();
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
		try {
			if(string.IsNullOrWhiteSpace(connString))
				throw new InvalidOperationException("Строка подключения не установлена.");

			var connectionStringBuilder = new MySqlConnectionStringBuilder(connString);
			IDatabaseConnectionSettings databaseConnectionSettings = new DatabaseConnectionSettings(connectionStringBuilder);

			var containerBuilder = new ContainerBuilder()
				.AutofacDatabaseConfig()
				.AddAvaloniaNavigation()
				.AddCashFlow();

			ILifetimeScope? builtContainer = null;
			containerBuilder
				.Register(_ => new AutofacViewModelResolver(builtContainer!))
				.As<IViewModelResolver>()
				.SingleInstance();

			var services = new ServiceCollection();
			services.AddDatabaseSettings(databaseConnectionSettings);
			services.AddClassConfig(userLogin ?? string.Empty, userSessionId ?? string.Empty);
			services.AddGuiClasses();
			services.AddInteractive();
			containerBuilder.Populate(services);

			var container = containerBuilder.Build();
			builtContainer = container;
			mainContainer = container;

			var viewResolver = container.Resolve<QS.Navigation.IAvaloniaViewResolver>();
			DataTemplates.Add(viewResolver);

			return container.Resolve<MainWindow>(
				new TypedParameter(typeof(string), userLogin),
				new TypedParameter(typeof(string), userSessionId),
				new TypedParameter(typeof(string), userBaseTitle));
		}
		catch(Exception ex) {
			logger.Error(ex, "Не удалось создать главное окно.");
			throw;
		}
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

	private void DisposeApplicationServices() {
		mainContainer?.Dispose();
		mainContainer = null;

		Program.StartupServiceProvider?.Dispose();
	}
}
