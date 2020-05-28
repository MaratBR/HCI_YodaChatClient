using Autofac;
using Autofac.Features.ResolveAnything;
using System.Windows;
using YodaApp.Services;

namespace YodaApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // создать контейнер для dependency injection
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule(new ServiceAutofacModule());
            containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            var container = containerBuilder.Build();
            var startup = container.Resolve<IStartUpService>();
            startup.Start();
        }
    }
}