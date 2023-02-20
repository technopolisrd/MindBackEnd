using Common.Core.Base;
using Mind.Context.Data.Security;
using Mind.Entity.Tables;
using Mind.Repository.Mind.Contracts;

namespace Mind.Repository.Mind.Repository;

public class TeamRepository : RepositoryBase<Team, MindContext>, iTeamRepository
{
    public TeamRepository(MindContext context) : base(context) { }
}
