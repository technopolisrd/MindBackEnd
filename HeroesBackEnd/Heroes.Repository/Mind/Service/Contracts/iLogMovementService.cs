using Mind.Entity.Tables.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mind.Repository.Mind.Service.Contracts;

public interface iLogMovementService
{
    public Task<List<LogMovementsDTO>> GetAll();
}
