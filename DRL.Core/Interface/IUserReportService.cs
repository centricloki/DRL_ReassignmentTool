using DRL.Entity;
using System.Collections.Generic;

namespace DRL.Core.Interface
{
    public interface IUserReportService
    {
        List<ENTUserReportList> GetUserReport(int roleId = 0, int entityId = 0);

        List<ENTUserReportHierarchyNode> GetUserReportHierarchy();
    }
}
