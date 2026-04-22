using System;

namespace DRL.Model
{
    public interface IEntityBase
    {
        int RecordId { get; set; }
        Guid RefId { get; set; }
    }
}