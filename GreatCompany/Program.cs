using Avalonia;
using GreatCompany.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QS.Launcher;
using QS.Launcher.AppRunner;
using QS.Project;
using ReactiveUI.Avalonia;

namespace GreatCompany;

public static class Program {
	private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

	[STAThread]
	public static void Main(string[] args) {
		// Последний рубеж: всё, что нигде не перехватили, хотя бы попадает в лог
		AppDomain.CurrentDomain.UnhandledException += (_, e) =>
			logger.Fatal(e.ExceptionObject as Exception, "Необработанное исключение.");
		TaskScheduler.UnobservedTaskException += (_, e) => {
			logger.Error(e.Exception, "Необработанное исключение в фоновой задаче.");
			e.SetObserved();
		};

		string? connectionString = Environment.GetEnvironmentVariable("QS_CONNECTION_STRING");
		string? login = Environment.GetEnvironmentVariable("QS_LOGIN");
		string? sessionId = Environment.GetEnvironmentVariable("QS_SessionId");
		string? baseTitle = Environment.GetEnvironmentVariable("QS_BaseTitle");

		ClearConnectionEnvironment();

		var startLauncher = string.IsNullOrEmpty(connectionString);
		// Сервисы лончера нужны все время работы приложения: окно входа может открыться повторно,
		// поэтому освобождаем их только когда рабочий цикл Avalonia завершился
		using var startupServices = ConfigureStartupServices(startLauncher);

		if(startLauncher) {
			var runner = startupServices.GetRequiredService<InProcessRunner>();
			runner.OnLogin = response => {
				login = response.Login;
				sessionId = response.Parameters.GetValueOrDefault("SessionId");
				connectionString = response.ConnectionString;
				baseTitle = response.Parameters.GetValueOrDefault("BaseTitle");
			};
		}

		BuildAvaloniaApp(startupServices, connectionString, login, sessionId, baseTitle)
			.StartWithClassicDesktopLifetime(args);
	}

	private static ServiceProvider ConfigureStartupServices(bool withLauncher) {
		var startupServices = new ServiceCollection();

		if(withLauncher) {
			startupServices
				.AddGreatCompanyLauncherConfiguration(options => options.IsStandalone = false)
				.AddLauncherDependencies()
				.AddPages()
				.AddLauncherViewModels()
				.UseInProcessRunner()
				.AddInteractive();
		}

		return startupServices.BuildServiceProvider();
	}

	private static void ClearConnectionEnvironment() {
		Environment.SetEnvironmentVariable("QS_CONNECTION_STRING", null, EnvironmentVariableTarget.Process);
		Environment.SetEnvironmentVariable("QS_LOGIN", null, EnvironmentVariableTarget.Process);
		Environment.SetEnvironmentVariable("QS_SessionId", null, EnvironmentVariableTarget.Process);
		Environment.SetEnvironmentVariable("QS_BaseTitle", null, EnvironmentVariableTarget.Process);
	}

	// Нужен дизайнеру Avalonia: он поднимает приложение без строки подключения и лончера
	public static AppBuilder BuildAvaloniaApp()
		=> BuildAvaloniaApp(null, null, null, null, null);

	public static AppBuilder BuildAvaloniaApp(IServiceProvider? startupServices,
		string? connectionString, string? login, string? sessionId, string? baseTitle)
		=> AppBuilder.Configure(() => new GreatCompanyApp(startupServices, connectionString, login, sessionId, baseTitle))
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
}
