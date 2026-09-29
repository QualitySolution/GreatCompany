using Avalonia;
using GreatCompany.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QS.ErrorReporting;
using QS.Launcher;
using QS.Project;
using ReactiveUI.Avalonia;

namespace GreatCompany;

public static class Program {
	private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

	[STAThread]
	public static void Main(string[] args) {
		logger.Info("=== Старт приложения ===");

		// отправителя отчёта задаём после входа в базу, до этого контейнера нет
		var crashReporting = new CrashReporting();
		crashReporting.Subscribe();

		TaskScheduler.UnobservedTaskException += (_, e) => {
			logger.Error(e.Exception, "Необработанное исключение в фоновой задаче.");
			e.SetObserved();
		};

		string? connectionString = Environment.GetEnvironmentVariable("QS_CONNECTION_STRING");
		string? login = Environment.GetEnvironmentVariable("QS_LOGIN");
		string? sessionId = Environment.GetEnvironmentVariable("QS_SessionId");
		string? baseTitle = Environment.GetEnvironmentVariable("QS_BaseTitle");

		ClearConnectionEnvironment();

		// окно лончера создаётся из этих сервисов внутри рабочего цикла Avalonia, освобождаем после него
		using var startupServices = ConfigureStartupServices(string.IsNullOrEmpty(connectionString));

		BuildAvaloniaApp(startupServices, crashReporting, connectionString, login, sessionId, baseTitle)
			.StartWithClassicDesktopLifetime(args);
		logger.Info("=== Завершение приложения ===");
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

	// нужен дизайнеру Avalonia, он поднимает приложение без строки подключения и лончера
	public static AppBuilder BuildAvaloniaApp()
		=> BuildAvaloniaApp(null, null, null, null, null, null);

	public static AppBuilder BuildAvaloniaApp(IServiceProvider? startupServices, CrashReporting? crashReporting,
		string? connectionString, string? login, string? sessionId, string? baseTitle)
		=> AppBuilder.Configure(() => new GreatCompanyApp(startupServices, crashReporting, connectionString, login, sessionId, baseTitle))
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
}
