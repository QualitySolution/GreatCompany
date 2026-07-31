using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using QS.Project;
using QS.Project.DB;
using QS.ViewModels.Resolve;

namespace GreatCompany;

internal static class CompositionRoot {
	public static ILifetimeScope BuildContainer(IDatabaseConnectionSettings settings, string login, string sessionId) {
		var builder = new ContainerBuilder()
			.AutofacDatabaseConfig()
			.AddAvaloniaNavigation()
			.AddCashFlow();

		builder.Register(c => new AutofacViewModelResolver(c.Resolve<ILifetimeScope>()))
			.As<IViewModelResolver>().SingleInstance();

		var services = new ServiceCollection();
		services.AddDatabaseSettings(settings);
		services.AddClassConfig(login, sessionId);
		services.AddGuiClasses();
		services.AddInteractive();
		builder.Populate(services);

		return builder.Build();
	}
}
