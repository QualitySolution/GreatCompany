using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using QS.Cloud.Client;
using QS.Launcher;
using QS.Project.Versioning;
using QS.Utilities.Extensions;

namespace GreatCompany.Configuration;

public static class GreatCompanyLauncherConfiguration {
	public static IServiceCollection AddGreatCompanyLauncherConfiguration(
		this IServiceCollection services,
		Action<LauncherOptions>? configureOptions = null) {
		var assembly = Assembly.GetExecutingAssembly();

		services.AddConnectionType(new QsCloudConnectionTypeBase());

		var options = new LauncherOptions {
			AppTitle = ApplicationConstants.ProductTitle,
			LogoImage = assembly.GetResourceByteArray("GreatCompany.Assets.logo.png"),
			LogoIcon = assembly.GetResourceByteArray("GreatCompany.Assets.logo.ico"),
			ConnectionsJsonFileName = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"GreatCompany",
				"connections.json"),
			OldConfigFilename = null,
			MakeDefaultConnections = () => [
				new Dictionary<string, string>(StringComparer.Ordinal) {
					{ "Title", "По умолчанию" },
					{ "Type", "QSCloud" },
					{ "Account", "qsolution" },
					{ "Last", "true" },
				}
			],
		};

		configureOptions?.Invoke(options);

		services.AddLauncherOptions(options);
		services.AddSingleton<IApplicationInfo, ApplicationInfo>(provider => new ApplicationInfo {
			ProductCode = ApplicationConstants.ProductCode,
		});

		return services;
	}
}
