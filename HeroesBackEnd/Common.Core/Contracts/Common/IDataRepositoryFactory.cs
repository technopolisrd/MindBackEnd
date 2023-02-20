namespace Common.Core.Contracts.Common
{
    public interface IDataRepositoryFactory
    {
        IDataRepository<TEntity> GetDataRepository<TEntity>() where TEntity : class, new();
        TRepository GetCustomDataRepository<TRepository>() where TRepository : IDataRepository;
        IUnitOfWork GetUnitOfWork();
    }
}
