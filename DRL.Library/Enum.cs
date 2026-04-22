using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DRL.Library
{
    public enum PermissionFlag
    {
        NONE = 0,
        VIEW = 1,
        ADD = 2,
        EDIT = 4,
        DELETE = 8
    }

    public enum DatabaseErrorCode
    {
        [Description("D0x001")] EntityAlreadyExists = 1,

        [Description("D0x002")] NoDataFound = 2,

        [Description("D0x003")] InvalidData = 3,

        [Description("D0x004")] CanNotDelete = 4,

        [Description("D0x005")] DuplicateData = 5
    }

    public enum BusinessErrorCode
    {
        [Description("B0x001")] Success = 1,

        [Description("B0x002")] EntityCanNotBeEmpty = 2,

        [Description("B0x003")] InvalidEntity = 3,

        [Description("B0x004")] RequestCanNotBeNull = 4,

        [Description("B0x005")] OperationalError = 5,

        [Description("B0x006")] Created = 6,

        [Description("B0x007")] Updated = 7,

        [Description("B0x008")] Deleted = 8,

        [Description("B0x009")] NoContent = 9,

        [Description("B0x010")] InvalidData = 10,

        [Description("B0x011")] InvalidLength = 11
    }

    public enum SamplingPlanStatus
    {
        Draft = 1,
        Scheduled = 2,
        Active = 3,
        Executed = 4,
        Close = 5
    }
}
