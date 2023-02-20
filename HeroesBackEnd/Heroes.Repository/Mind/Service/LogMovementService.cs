using LinqKit;
using Mind.Context.Data.Security;
using Mind.Entity.Tables;
using Mind.Entity.Tables.DTO;
using Mind.Repository.Mind.Contracts;
using Mind.Repository.Mind.Service.Contracts;
using Microsoft.Extensions.Configuration;
using Mind.Business.Log;

namespace Mind.Repository.Mind.Service;

#nullable disable

public class LogMovementService : iLogMovementService
{
    private readonly iLogMovementRepository _repo;
    private readonly IConfiguration Configuration;

    public LogMovementService(iLogMovementRepository repo, IConfiguration configuration)
    {
        _repo = repo;
        Configuration = configuration; ;
    }

    public async Task<List<LogMovementsDTO>> GetAll()
    {
        var entity = await _repo.GetAll(logs => logs.Select(log => new LogMovementsDTO
        {
            ID = log.ID,
            CustomerID = log.CustomerID,
            CustomerName = log.customer.CustomerName,
            AccountID = log.AccountID,
            UserName = log.customer.account.FirstName + " " + log.customer.account.LastName,
            StartDate = log.StartDate,
            EndDate = log.EndDate,
            Action = log.Action,
        }));

        if (entity.Count() == 0)
        {
            return null;
        }

        return entity.ToList();
    }
}
