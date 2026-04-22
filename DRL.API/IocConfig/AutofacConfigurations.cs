using DRL.Core.Interface;
using DRL.Core.Manager;
using DRL.Model.Repository.Implementation;
using DRL.Model.Repository.Interface;
using Autofac;

namespace DRL.API.IocConfig
{
    public class AutofacConfigurations : Module
    {
        protected override void Load(ContainerBuilder autofacBuilder)
        {
            autofacBuilder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerDependency();
            

            autofacBuilder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();
        }
    }
}