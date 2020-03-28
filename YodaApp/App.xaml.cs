using Autofac;
using Autofac.Features.ResolveAnything;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YodaApp.Persistence;
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
