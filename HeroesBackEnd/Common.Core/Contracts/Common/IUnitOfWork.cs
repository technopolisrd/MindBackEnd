using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Core.Contracts.Common
{
    public interface IUnitOfWork
    {
        IDbContextTransaction CreateTransaction();

        int SaveChanges();
    }
}
