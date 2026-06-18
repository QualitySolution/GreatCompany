using Avalonia;
using GreatCompany.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QS.Launcher;
using QS.Launcher.AppRunner;
using QS.Project;
using ReactiveUI.Avalonia;

namespace GreatCompany;

public class Program {
	private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
	internal static ServiceProvider StartupServiceProvider = null!;

	[STAThread]
	public static void Main(string[] args) {
		logger.Info("=== Старт приложения ===");

		string? connectionString = Environment.GetEnvironmentVariable("QS_CONNECTION_STRING");
		string? login = Environment.GetEnvironmentVariable("QS_LOGIN");
		string? sessionId = Environment.GetEnvironmentVariable("QS_SessionId");
		string? baseTitle = Environment.GetEnvironmentVariable("QS_BaseTitle");

		ClearConnectionEnvironment();

		var startLauncher = string.IsNullOrEmpty(connectionString);
		ConfigureStartupServices(startLauncher);

		if(startLauncher) {
			var runner = StartupServiceProvider.GetRequiredService<InProcessRunner>();
			runner.OnLogin = response => {
				login = response.Login;
				sessionId = response.Parameters.GetValueOrDefault("SessionId");
				connectionString = response.ConnectionString;
				baseTitle = response.Parameters.GetValueOrDefault("BaseTitle");
			};
		}

		BuildAvaloniaApp(connectionString, login, sessionId, baseTitle).StartWithClassicDesktopLifetime(args);
		logger.Info("=== Завершение приложения ===");
	}

	private static void ConfigureStartupServices(bool withLauncher) {
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

		StartupServiceProvider = startupServices.BuildServiceProvider();
	}

	private static void ClearConnectionEnvironment() {
		Environment.SetEnvironmentVariable("QS_CONNECTION_STRING", null, EnvironmentVariableTarget.Process);
		Environment.SetEnvironmentVariable("QS_LOGIN", null, EnvironmentVariableTarget.Process);
		Environment.SetEnvironmentVariable("QS_SessionId", null, EnvironmentVariableTarget.Process);
		Environment.SetEnvironmentVariable("QS_BaseTitle", null, EnvironmentVariableTarget.Process);
	}

	public static AppBuilder BuildAvaloniaApp()
		=> BuildAvaloniaApp(null, null, null, null);

	public static AppBuilder BuildAvaloniaApp(string? connectionString, string? login, string? sessionId, string? baseTitle)
		=> AppBuilder.Configure(() => new GreatCompanyApp(connectionString, login, sessionId, baseTitle))
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
}
