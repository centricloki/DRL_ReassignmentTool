using DRL.Entity;
using DRL.Library;
using System.Collections.Generic;

namespace DRL.Core.Interface
{
    public interface IBrandStyleService
    {
        List<ENTBrandStyleMaster> GetBrandStyleMaster();
        ActionStatus UpdateBrandStyleMaster(int brandStyleId, int sortOrder);
    }
}
