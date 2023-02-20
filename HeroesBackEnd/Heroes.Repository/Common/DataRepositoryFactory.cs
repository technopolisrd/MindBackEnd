using Common.Core.Contracts.Common;
using Common.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Mind.Repository.Common;

#nullable disable

public class DataRepositoryFactory
{
    private readonly IServiceProvider services;

    public DataRepositoryFactory() { }

    public DataRepositoryFactory(IServiceProvider services)
    {
        this.services = services;
    }

    public TRepository GetCustomDataRepository<TRepository>() where TRepository : IDataRepository
    {
        //Import instance of the repository from the DI container
        var instance = services.GetService<TRepository>();

        return instance;
    }

    public IDataRepository<TEntity> GetDataRepository<TEntity>() where TEntity : class, new()
    {
        //Import instance of T from the DI container
        var instance = services.GetService<IDataRepository<TEntity>>();

        return instance;
    }

    public IUnitOfWork GetUnitOfWork()
    {
        //Import instance of T from the DI container
        var instance = services.GetService<IUnitOfWork>();

        return instance;
    }
}
