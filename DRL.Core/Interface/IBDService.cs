using DRL.Entity;
using DRL.Entity.Response;
using DRL.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRL.Core.Interface
{
    public interface IBDService
    {
        List<ENTLookUpItem> GetBDsLookup();
        ENTBD GetBD(int bdId);
        List<ENTBDResponse> GetBDList();
        ActionStatus CheckBDNameExists(string bdName, int bdId);
        ActionStatus Insert(ENTBD bd);
        ActionStatus Update(ENTBD bd);
        ActionStatus DeleteBD(ENTPatchRequest activeStatus);
        ActionStatus ManageBDStatus(ENTPatchRequest activeStatus);
    }
}
