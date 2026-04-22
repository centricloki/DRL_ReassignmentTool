using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DRL.Core.IocConfig
{
    public class Configuration
    {
        public static ContainerBuilder BuilderAutofac { get; private set; }

        public static AutofacServiceProvider Initialize(IServiceCollection services)
        {
            BuilderAutofac = new ContainerBuilder();
            BuilderAutofac.RegisterModule(new IocConfigurations());
            BuilderAutofac.Populate(services);
            var container = BuilderAutofac.Build();
            return new AutofacServiceProvider(container);
        }
    }
}
