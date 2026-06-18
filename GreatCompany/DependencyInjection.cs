using Autofac;
using QS.Navigation;
using QS.Project.Versioning.ViewModels;
using QS.Project.Versioning.Views;

namespace GreatCompany;

internal static class DependencyInjection {
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
