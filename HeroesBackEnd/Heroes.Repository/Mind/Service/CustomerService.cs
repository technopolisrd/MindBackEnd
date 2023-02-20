using LinqKit;
using Mind.Context.Data.Security;
using Mind.Entity.Tables;
using Mind.Entity.Tables.DTO;
using Mind.Repository.Mind.Contracts;
using Mind.Repository.Mind.Service.Contracts;
using Microsoft.Extensions.Configuration;
using Mind.Repository.Common;

namespace Mind.Repository.Mind.Service;

#nullable disable

public class CustomerService : iCustomerService
{
    private readonly iCustomerRepository _repo;
    private readonly IConfiguration Configuration;

    public CustomerService(iCustomerRepository repo, IConfiguration configuration)
    {
        _repo = repo;
        Configuration = configuration; ;
    }

    Audit audit = null;

    public async Task<CustomerDTO> AddAsync(CustomerDTO custDTO, int userID)
    {
        Customer custTable = new Customer();

        custTable.AccountName = custDTO.AccountName;
        custTable.CustomerName = custDTO.CustomerName;
        custTable.AccountID = int.Parse(custDTO.AccountID);

        audit = new Audit(Configuration);
        audit.FillAudit(ref custTable, 0, userID);

        Customer resultWithDbID;

        try
        {
            resultWithDbID = await _repo.Add(custTable);
        }
        catch (Exception)
        {
            return null;
        }

        if (resultWithDbID == null)
        {
            return null;
        }

        custDTO.ID = resultWithDbID.ID.ToString();

        return custDTO;
    }

    public bool ExistOrUsed(int? id)
    {
        if (id != null)
        {
            return _repo.Exists((int)id);
        }

        return false;
    }

    public async Task<List<CustomerDTO>> GetAll()
    {
        var entity = await _repo.GetAll(customers => customers.Select(customer => new CustomerDTO
        {
            ID = customer.ID.ToString(),
            AccountID= customer.AccountID.ToString(),
            AccountName = customer.AccountName,
            CustomerName = customer.CustomerName,
            UserName = customer.account.FirstName + " " + customer.account.LastName
        }));

        if (entity.Count() == 0)
        {
            return null;
        }

        return entity.ToList();
    }

    public async Task<List<CustomerDTO>> GetAllBySearch(string searchString)
    {
        var predicate = PredicateBuilder.New<Customer>();

        string[] searchStr = searchString.Split("|");

        foreach (var search in searchStr)
        {
            if (!String.IsNullOrEmpty(search))
            {
                predicate = predicate.Or(x => x.ID.ToString().Contains(search));
                predicate = predicate.Or(x => x.AccountID.ToString().Contains(search));
                predicate = predicate.Or(x => x.AccountName.Contains(search));
                predicate = predicate.Or(x => x.CustomerName.Contains(search));
                predicate = predicate.Or(x => x.account.FirstName.Contains(search));
                predicate = predicate.Or(x => x.account.LastName.Contains(search));
            }
        }

        var entity = await _repo.GetAll(customers => customers.Select(customer => new CustomerDTO
        {
            ID = customer.ID.ToString(),
            AccountID = customer.AccountID.ToString(),
            AccountName = customer.AccountName,
            CustomerName = customer.CustomerName,
            UserName = customer.account.FirstName + " " + customer.account.LastName
        }), predicate);

        if (entity.Count() == 0)
        {
            return null;
        }

        return entity.ToList();
    }

    public async Task<List<DropDownDTO>> GetAllForDropDown()
    {
        var entity = await _repo.GetAll(customers => customers.Select(customer => new DropDownDTO
        {
            Code = customer.ID.ToString(),
            Name = customer.CustomerName
        }));

        if (entity.Count() == 0)
        {
            return null;
        }

        return entity.ToList();
    }

    public async Task<CustomerDTO> GetDataById(int id)
    {
        var customer = await _repo.Get(id);

        if (customer == null)
        {
            return null;
        }

        CustomerDTO customerDTO = new CustomerDTO();

        customerDTO.ID = customer.ID.ToString();
        customerDTO.AccountID = customer.AccountID.ToString();
        customerDTO.AccountName = customer.AccountName;
        customerDTO.CustomerName = customer.CustomerName;
        customerDTO.UserName = customer.account.FirstName + " " + customer.account.LastName;

        return customerDTO;
    }

    public async Task<CustomerDTO> RemoveAsync(int id, int user)
    {
        Customer custTable;
        CustomerDTO customerDTO;

        try
        {
            custTable = await _repo.Get(id);
        }
        catch (Exception)
        {
            customerDTO = new CustomerDTO();
            customerDTO.ID = "0";
            return customerDTO;
        }

        if (custTable == null)
        {
            customerDTO = new CustomerDTO();
            customerDTO.ID = "0";
            return customerDTO;
        }

        custTable.Deferred = true;

        audit = new Audit(Configuration);
        audit.FillAudit(ref custTable, 2, user);

        Customer result;

        try
        {
            result = await _repo.Update(custTable);
        }
        catch (Exception)
        {
            return null;
        }

        customerDTO = new CustomerDTO();

        customerDTO.ID = result.ID.ToString();
        customerDTO.AccountName = result.AccountName;
        customerDTO.CustomerName = result.CustomerName;
        customerDTO.AccountID = result.AccountID.ToString();

        return customerDTO;
    }

    public async Task<CustomerDTO> UpdateAsync(CustomerDTO entity, int user)
    {
        Customer custTable;

        try
        {
            custTable = await _repo.Get(int.Parse(entity.ID));
        }
        catch (Exception)
        {
            CustomerDTO cust = new CustomerDTO();
            cust.ID = "0";
            return cust;
        }

        if (custTable == null)
        {
            CustomerDTO cust = new CustomerDTO();
            cust.ID = "0";
            return cust;
        }

        custTable.AccountName = entity.AccountName;
        custTable.CustomerName = entity.CustomerName;
        custTable.AccountID = int.Parse(entity.AccountID);

        audit = new Audit(Configuration);
        audit.FillAudit(ref custTable, 1, user);

        Customer result;

        try
        {
            result = await _repo.Update(custTable);
        }
        catch (Exception)
        {
            return null;
        }

        return entity;
    }

}
