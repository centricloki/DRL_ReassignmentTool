using Autofac;

using DRL.Core.Hepler;
using DRL.Core.Interface;
using DRL.Core.Manager;
using DRL.Core.Service;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Model.DataBase;
using DRL.Model.Models;
using DRL.Model.Repository.Implementation;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Implementation;
using DRL.Model.UnitOfWork.Interface;

using Microsoft.EntityFrameworkCore;

namespace DRL.Core.IocConfig
{
    public class IocConfigurations : Module
    {
        protected override void Load(ContainerBuilder autofacBuilder)
        {
            // Register DbContext from the Microsoft DI container that was already registered via AddDbContextPool
            // This is critical: Autofac.Populate() should have already added it, but we ensure it's properly typed
            autofacBuilder.Register(c =>
            {
                // Resolve from the ambient service provider (populated by Populate call)
                var context = c.Resolve<DRLNewContext>();
                return (DbContext)context;
            })
            .As<DbContext>()
            .InstancePerLifetimeScope();

            // Register UnitOfWork with IUnitOfWork - now DbContext will resolve
            autofacBuilder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            // Register Repository
            autofacBuilder.RegisterType<UserRepository>()
                .As<IUserRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<RoleRepository>()
                .As<IRoleRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<RegionRepository>()
                .As<IRegionRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<TerritoryRepository>()
                .As<ITerritoryRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<ZoneRepository>()
                .As<IZoneRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<TeamRepository>()
                .As<ITeamRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<CityRepository>()
                .As<ICityRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<StateRepository>()
                .As<IStateRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<CustomerRepository>()
                .As<ICustomerRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<BrandStyleRepository>()
                .As<IBrandStyleRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<AVPMasterRepository>()
                .As<IAVPMasterRepository>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<BDMasterRepository>()
                .As<IBDMasterRepository>()
                .InstancePerLifetimeScope();

            // Register Helpers
            autofacBuilder.RegisterType<AuthenticationService>()
                .As<IAuthenticationService>()
                .InstancePerLifetimeScope();

            // Register Manager/Services
            autofacBuilder.RegisterType<UserService>()
                .As<IUserService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<RoleService>()
                .As<IRoleService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<RegionService>()
                .As<IRegionService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<TerritoryService>()
                .As<ITerritoryService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<ZoneService>()
                .As<IZoneService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<CityService>()
                .As<ICityService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<StateService>()
                .As<IStateService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<CustomerService>()
                .As<ICustomerService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<BrandStyleService>()
                .As<IBrandStyleService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<UserReportService>()
                .As<IUserReportService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<AVPService>()
                .As<IAVPService>()
                .InstancePerLifetimeScope();
            autofacBuilder.RegisterType<BDService>()
                .As<IBDService>()
                .InstancePerLifetimeScope();

            autofacBuilder.RegisterType<NavigationPermissionService>()
            .As<INavigationPermissionService>()
            .InstancePerLifetimeScope();

        }
    }
}