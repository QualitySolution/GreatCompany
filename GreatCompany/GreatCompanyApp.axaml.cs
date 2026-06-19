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
			if(string.IsNullOrEmpty(connectionString))
				ShowLauncher(desktop);
			else
				desktop.MainWindow = CreateMainWindow(connectionString, login, sessionId, baseTitle);
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void ShowLauncher(IClassicDesktopStyleApplicationLifetime desktop) {
		desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

		var launcherWindow = Program.StartupServiceProvider.GetRequiredService<QS.Launcher.Views.MainWindow>();
		var runner = Program.StartupServiceProvider.GetRequiredService<InProcessRunner>();
		var loginReceived = new ManualResetEventSlim(false);

		string? resultLogin = null;
		string? resultSessionId = null;
		string? resultBaseTitle = null;
		string? resultConnectionString = null;
		var previousCallback = runner.OnLogin;

		runner.OnLogin = response => {
			previousCallback?.Invoke(response);
			resultLogin = response.Login;
			resultSessionId = response.Parameters.GetValueOrDefault("SessionId");
			resultBaseTitle = response.Parameters.GetValueOrDefault("BaseTitle");
			resultConnectionString = response.ConnectionString;
			loginReceived.Set();
		};

		launcherWindow.Show();

		Task.Run(() => {
			loginReceived.Wait();

			Dispatcher.UIThread.Post(() => {
				var mainWindow = CreateMainWindow(resultConnectionString, resultLogin, resultSessionId, resultBaseTitle);
				desktop.MainWindow = mainWindow;
				desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
				mainWindow.Show();
				launcherWindow.Close();
			});
		});
	}

	private MainWindow CreateMainWindow(string? connString, string? userLogin, string? userSessionId, string? userBaseTitle) {
		try {
			if(string.IsNullOrWhiteSpace(connString))
				throw new InvalidOperationException("Строка подключения не установлена.");

			var connectionStringBuilder = new MySqlConnectionStringBuilder(connString);
			IDatabaseConnectionSettings databaseConnectionSettings = new DatabaseConnectionSettings(connectionStringBuilder);

			var containerBuilder = new ContainerBuilder()
				.AutofacDatabaseConfig()
				.AddAvaloniaNavigation();

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
}
