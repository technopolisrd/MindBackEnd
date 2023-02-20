using Common.Core.Contracts.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Mind.Context.Data.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mind.Repository.Common;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly MindContext _DataContext;

    public UnitOfWork(MindContext dataContext)
    {
        _DataContext = dataContext;
    }

    public IDbContextTransaction CreateTransaction()
    {
        return this._DataContext.Database.BeginTransaction();
    }

    public void Dispose()
    {
        if (_DataContext != null)
        {
            _DataContext.Dispose();
        }
    }

    public int SaveChanges()
    {
        return _DataContext.SaveChanges();
    }
}
