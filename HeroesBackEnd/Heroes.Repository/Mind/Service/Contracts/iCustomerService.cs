using Mind.Entity.Tables.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mind.Repository.Mind.Service.Contracts;

public interface iCustomerService
{
    public bool ExistOrUsed(int? id);

    public Task<List<CustomerDTO>> GetAll();

    public Task<List<CustomerDTO>> GetAllBySearch(string searchString);

    public Task<CustomerDTO> GetDataById(int id);

    public Task<List<DropDownDTO>> GetAllForDropDown();

    public Task<CustomerDTO> AddAsync(CustomerDTO entity, int user);

    public Task<CustomerDTO> UpdateAsync(CustomerDTO entity, int user);

    public Task<CustomerDTO> RemoveAsync(int id, int user);
}
