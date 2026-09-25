using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Tool.hbm2ddl;
using QS.Project;
using QS.Project.DB;
using QS.ViewModels.Resolve;

namespace GreatCompany;

internal static class CompositionRoot {
	public static ILifetimeScope BuildContainer(IDatabaseConnectionSettings settings, string login, string sessionId) {
		var builder = new ContainerBuilder()
			.AddApplicationInfo()
			.AddAvaloniaNavigation()
			.AddDeletion()
			.AddErrorReporting()
			.AddValidation()
			.AddViewModels();

		builder.Register(c => new AutofacViewModelResolver(c.Resolve<ILifetimeScope>()))
			.As<IViewModelResolver>().SingleInstance();

		var services = new ServiceCollection();
		services.AddDatabaseSettings(settings);
		services.AddClassConfig(login, sessionId);
		services.AddGuiClasses();
		services.AddInteractive();
		builder.Populate(services);

		var container = builder.Build();
		ValidateSchemaInDebug(container);
		return container;
	}

	private static void ValidateSchemaInDebug(ILifetimeScope container) {
#if DEBUG
		var configuration = container.Resolve<NHibernate.Cfg.Configuration>();

		try {
			new SchemaValidator(configuration).Validate();
		}
		catch(NHibernate.SchemaValidationException ex) {
			throw new InvalidOperationException(
				"Схема базы не совпадает с маппингом:\n" +
				String.Join("\n", ex.ValidationErrors), ex);
		}
#endif
	}
}
