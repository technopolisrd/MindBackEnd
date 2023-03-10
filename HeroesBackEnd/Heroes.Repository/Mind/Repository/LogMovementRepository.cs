using Common.Core.Base;
using Mind.Context.Data.Security;
using Mind.Entity.Tables;
using Mind.Repository.Mind.Contracts;

namespace Mind.Repository.Mind.Repository;

public class LogMovementRepository : RepositoryBase<LogMovements, MindContext>, iLogMovementRepository
{
    public LogMovementRepository(MindContext context) : base(context) { }
}
