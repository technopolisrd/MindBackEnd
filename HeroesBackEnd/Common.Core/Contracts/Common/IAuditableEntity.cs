using System;

namespace Common.Core.Contracts.Common
{
    public interface IAuditableEntity
    {
        DateTime CreatedDate { get; set; }
        string CreatedById { get; set; }
        DateTime UpdatedDate { get; set; }
        string UpdatedById { get; set; }
    }
}
