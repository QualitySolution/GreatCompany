using Autofac;
using FluentNHibernate.Conventions;
using GreatCompany.Data;
using GreatCompany.Data.Mappings;
using GreatCompany.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using QS.Cloud.Client;
using QS.Deletion;
using QS.Deletion.Configuration;
using QS.ErrorReporting;
using QS.ErrorReporting.Handlers;
using QS.Extensions.Observable.Collections.List;
using QS.Journal;
using QS.Journal.Actions;
using QS.Journal.Search;
using QS.Journal.Views;
using QS.Navigation;
using QS.Project;
using QS.Project.Core;
using QS.Project.DB;
using QS.Project.Domain;
using QS.Project.Services;
using QS.Services;
using QS.Validation;
using QS.Project.Versioning.ViewModels;
using QS.Project.Versioning.Views;
using QS.Project.Versioning;

namespace GreatCompany;

internal static class DependencyInjection {
	public static IServiceCollection AddDatabaseSettings(this IServiceCollection services, IDatabaseConnectionSettings settings) {
		return services
			.AddMappingAssemblies(typeof(UserBase).Assembly)
			.AddSingleton<IDatabaseConfigurationExposer, EntityAutoMapping>()
			.AddDatabaseConnection()
			.AddSingleton(settings)
			.AddDatabaseConnectionString()
			.AddDatabaseInfo() // нужно отчёту об ошибке
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

	public static ContainerBuilder AddApplicationInfo(this ContainerBuilder builder) {
		builder.Register(_ => new ApplicationInfo {
			ProductCode = ApplicationConstants.ProductCode,
		}).As<IApplicationInfo>().SingleInstance();

		return builder;
	}

	public static ContainerBuilder AddAvaloniaNavigation(this ContainerBuilder builder) {
		builder.Register(_ => new DefaultPageHashGenerator()).As<IPageHashGenerator>().SingleInstance();
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
					ctx.Resolve<AvaloniaViewResolver>()
				)
				// Общие вью журнала сопоставляем явно. Таблицу к журналу подбирает уже сам JournalView — по суффиксу GridView
				.RegisterView<IJournalViewModel, JournalView>()
				.RegisterView<SearchViewModel, SearchView>()
				.RegisterView<IButtonJournalActionsViewModel, ButtonJournalActionsView>()
		).SingleInstance();

		builder.Register(ctx => {
			var contextCopy = ctx.Resolve<IComponentContext>();
			return new AvaloniaViewFactory(
				() => contextCopy.Resolve<IAvaloniaViewResolver>(),
				serviceType => contextCopy.ResolveOptional(serviceType));
		}).AsSelf().SingleInstance();

		builder.RegisterType<MainWindow>();
		builder.RegisterType<ChangeLogViewModel>().AsSelf();

		return builder;
	}

	/// <summary>
	/// диалоги показывают дерево зависимостей, правила задаёт <see cref="DeletionConfiguration"/>.
	/// </summary>
	public static ContainerBuilder AddDeletion(this ContainerBuilder builder) {
		builder.RegisterModule(new DeletionAutofacModule());
		builder.Register(ctx => {
			var configuration = new DeleteConfiguration(ctx.Resolve<NHibernate.Cfg.Configuration>());
			Data.DeletionConfiguration.ConfigureDeletion(configuration);
			return configuration;
		}).AsSelf().SingleInstance();
		builder.RegisterType<DeleteEntityGUIService>().As<IDeleteEntityService>().SingleInstance();

		builder.RegisterDecorator<BackgroundDeleteEntityService, IDeleteEntityService>();

		return builder;
	}

	/// <summary>
	/// цепочку обработчиков проходит каждая непредвиденная ошибка, неопознанная попадает в отчёт
	/// </summary>
	public static ContainerBuilder AddErrorReporting(this ContainerBuilder builder) {
		builder.RegisterType<ErrorHandlingService>().As<IErrorHandlingService>().SingleInstance();
		builder.RegisterType<DesktopErrorReporter>().As<IErrorReporter>().SingleInstance();
		builder.RegisterType<LogService>().As<ILogService>().SingleInstance();

#if DEBUG
		builder.Register(_ => new ErrorReportingSettings(false, true, false, 300)).As<IErrorReportingSettings>().SingleInstance();
#else
		builder.Register(_ => new ErrorReportingSettings(true, false, true, 300)).As<IErrorReportingSettings>().SingleInstance();
#endif

		// Кто именно поймал ошибку
		builder.Register(ctx => ctx.Resolve<IUserService>().GetCurrentUser()).As<IUserInfo>().SingleInstance();

		// разбор останавливается на первом обработчике, вернувшем true
		// MySqlExceptionErrorNumberLogger всегда возвращает false — он только пишет номера ошибок MySQL в лог
		builder.RegisterType<MySqlExceptionErrorNumberLogger>().As<IErrorHandler>();
		builder.RegisterType<ConnectionIsLost>().As<IErrorHandler>();
		builder.RegisterType<MySqlExceptionAccessDenied>().As<IErrorHandler>();
		builder.RegisterType<MySqlExceptionNoSpace>().As<IErrorHandler>();
		builder.RegisterType<MySqlException1055OnlyFullGroupBy>().As<IErrorHandler>();
		builder.RegisterType<MySqlException1366IncorrectStringValue>().As<IErrorHandler>();
		builder.RegisterType<NHibernateFlushAfterException>().As<IErrorHandler>();
		builder.RegisterType<NHibernateStaleObjectStateException>().As<IErrorHandler>();

		return builder;
	}

	public static ContainerBuilder AddValidation(this ContainerBuilder builder) {
		builder.Register(_ => new ObjectValidator()).As<IValidator>();

		return builder;
	}

	public static ContainerBuilder AddViewModels(this ContainerBuilder builder) {
		builder.RegisterAssemblyTypes(typeof(DependencyInjection).Assembly)
			.Where(t => t.Name.EndsWith("ViewModel", StringComparison.Ordinal))
			.AsSelf();

		builder.RegisterType<CardDependencies>().AsSelf();

		return builder;
	}
}
