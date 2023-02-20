using Common.Core.Base;
using Mind.Context.Data.Security;
using Mind.Entity.Tables;
using Mind.Repository.Mind.Contracts;

namespace Mind.Repository.Mind.Repository;

public class CustomerRepository : RepositoryBase<Customer, MindContext>, iCustomerRepository
{
    public CustomerRepository(MindContext context) : base(context) { }
}
