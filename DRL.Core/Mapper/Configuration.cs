using AutoMapper;
using DRL.Core.Mapper.Mappings;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper
{
    public class Configuration
    {
        public static IMapper Mapper { get; private set; }

        public static void Initialize()
        {
            var config = new MapperConfiguration(mc =>
            {
                mc.CreateMap<EF.UserMaster, ENTUser>().ConvertUsing(new UserToENTUser());
                mc.CreateMap<ENTUser, EF.UserMaster>().ConvertUsing(new ENTUserToUser());

                mc.CreateMap<EF.TerritoryMaster, ENTTeam>().ConvertUsing(new TerritoryMasterToENTTeam());
                mc.CreateMap<ENTTeam, EF.TerritoryMaster>().ConvertUsing(new ENTTeamToTerritoryMaster());

                mc.CreateMap<EF.RoleMaster, ENTRole>().ReverseMap();
                mc.CreateMap<EF.RegionMaster, ENTRegion>().ReverseMap();
                mc.CreateMap<EF.RoleMaster, ENTLookUpItem>().ConvertUsing(new RoleMasterToENTLookUpItem());
                mc.CreateMap<EF.RegionMaster, ENTLookUpItem>().ConvertUsing(new RegionMasterToENTLookUpItem());
                mc.CreateMap<EF.ZoneMaster, ENTLookUpItem>().ConvertUsing(new ZoneMasterToENTLookUpItem());
                mc.CreateMap<EF.TerritoryMaster, ENTLookUpItem>().ConvertUsing(new TerritoryMasterToENTLookUpItem());

                mc.CreateMap<EF.CityMaster, ENTLookUpItem>().ConvertUsing(new CityMasterToENTLookUpItem());
                mc.CreateMap<EF.StateMaster, ENTLookUpItem>().ConvertUsing(new StateMasterToENTLookUpItem());
                mc.CreateMap<EF.AVPMaster, ENTLookUpItem>().ConvertUsing(new AVPMasterToENTLookUpItem());
                mc.CreateMap<EF.BDMaster, ENTLookUpItem>().ConvertUsing(new BDMasterToENTLookUpItem());
                mc.CreateMap<EF.ZoneMaster, ENTZone>().ConvertUsing(new ZoneMasterToENTZone());
                
            });

            Mapper = config.CreateMapper();
        }
    }
}