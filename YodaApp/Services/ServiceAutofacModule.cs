using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApiClient;
using YodaApiClient.Implementation;
using YodaApp.Persistence;
using YodaApp.Services.Implementation;

namespace YodaApp.Services
{
    class ServiceAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<StartUpService>()
                .As<IStartUpService>()
                .SingleInstance();

            builder
                .RegisterType<ApiProvider>()
                .As<IApiProvider>()
                .SingleInstance();

            builder
                .RegisterType<AppConfigStore>()
                .As<IStore>()
                .SingleInstance();

            builder
                .RegisterType<WindowService>()
                .As<IWindowService>()
                .SingleInstance();

            builder
              .RegisterType<WindowFactory>()
              .As<IWindowFactory>()
              .SingleInstance();

            builder
              .RegisterType<AuthenticationService>()
              .As<IAuthenticationService>()
              .SingleInstance();

            builder.Register(c => c.Resolve<IStore>().GetConfiguration());
        }
    }
}
