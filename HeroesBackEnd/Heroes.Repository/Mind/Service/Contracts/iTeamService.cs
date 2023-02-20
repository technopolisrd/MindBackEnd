using Mind.Entity.Tables.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mind.Repository.Mind.Service.Contracts;

public interface iTeamService
{
    public bool ExistOrUsed(int? id);

    public Task<List<TeamDTO>> GetAll();

    public Task<List<TeamDTO>> GetAllBySearch(string searchString);

    public Task<TeamDTO> GetDataById(int id);

    public Task<List<DropDownDTO>> GetAllForDropDown();

    public Task<TeamDTO> AddAsync(TeamDTO entity, int user);

    public Task<TeamDTO> UpdateAsync(TeamDTO entity, int user);

    public Task<TeamDTO> RemoveAsync(int id, int user);
}
