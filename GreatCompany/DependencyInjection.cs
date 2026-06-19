using Autofac;
using FluentNHibernate.Conventions;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using QS.BaseParameters;
using QS.Cloud.Client;
using QS.DomainModel.UoW;
using QS.Extensions.Observable.Collections.List;
using QS.Navigation;
using QS.Project;
using QS.Project.Core;
using QS.Project.DB;
using QS.Project.Domain;
using QS.Project.Repositories;
using QS.Services;
using QS.Project.Versioning.ViewModels;
using QS.Project.Versioning.Views;
using QS.Project.Versioning;
using System.Data.Common;

namespace GreatCompany;

internal static class DependencyInjection {
	public static IServiceCollection AddDatabaseSettings(this IServiceCollection services, IDatabaseConnectionSettings settings) {
		return services
			.AddMappingAssemblies(typeof(UserBase).Assembly)
			.AddDatabaseConnection()
			.AddSingleton(settings)
			.AddDatabaseConnectionString()
			.AddSqlConfiguration()
			.AddSingleton<IConvention, ObservableListConvention>()
			.AddNHibernateConfiguration();
	}

	public static IServiceCollection AddClassConfig(this IServiceCollection services, string login, string sessionId) {
		return services
			.AddSessionFactory()
			.AddSingleton<ISessionProvider, DefaultSessionProvider>()
			.AddSingleton<IOrmConfig, DefaultOrmConfig>()
			.AddGuiTrackedUoW()
			.AddEntityChangeWatcher()
			.AddUserService(login)
			.AddSingleton<ISessionInfoProvider>(new SessionInfoProvider(sessionId))
			.AddSingleton<AliveCloudClient>();
	}

	public static ContainerBuilder AutofacDatabaseConfig(this ContainerBuilder builder) {
		builder.Register(_ => new ApplicationInfo {
			ProductCode = ApplicationConstants.ProductCode,
		}).As<IApplicationInfo>().SingleInstance();
		builder.RegisterType<DefaultSessionProvider>().As<ISessionProvider>();
		builder.Register(c => new MySqlConnectionFactory(c.Resolve<MySqlConnectionStringBuilder>().ConnectionString)).As<IConnectionFactory>();
		builder.Register<DbConnection>(c => c.Resolve<IConnectionFactory>().OpenConnection()).AsSelf().InstancePerLifetimeScope();
		builder.RegisterType<ParametersService>().UsingConstructor(typeof(Func<DbConnection>)).AsSelf().SingleInstance();
		builder.RegisterType<ChangeLogViewModel>().AsSelf();

		return builder;
	}

	public static ContainerBuilder AddAvaloniaNavigation(this ContainerBuilder builder) {
		builder.RegisterType<AvaloniaNavigationManager>().AsSelf().As<INavigationManager>().SingleInstance();
		builder.RegisterType<AvaloniaPageTabFactory>().AsSelf();
		builder.RegisterType<AvaloniaPageWindowFactory>().AsSelf();

		builder.Register(ctx => new AvaloniaViewResolver(
			ctx.Resolve<AvaloniaViewFactory>(),
			typeof(ChangeLogView).Assembly,
			typeof(MainWindow).Assembly
		)).AsSelf();

		builder.Register<IAvaloniaViewResolver>(ctx =>
			new AvaloniaRegisteredViewResolver(
				ctx.Resolve<AvaloniaViewFactory>(),
				ctx.Resolve<AvaloniaViewResolver>())
		).SingleInstance();

		builder.Register(ctx => {
			var contextCopy = ctx.Resolve<IComponentContext>();
			return new AvaloniaViewFactory(() => contextCopy.Resolve<IAvaloniaViewResolver>());
		}).AsSelf().SingleInstance();

		builder.RegisterType<MainWindow>();
		builder.RegisterType<ChangeLogViewModel>().AsSelf();

		return builder;
	}
}
