using LinqKit;
using Mind.Entity.Tables;
using Mind.Entity.Tables.DTO;
using Mind.Repository.Mind.Contracts;
using Mind.Repository.Mind.Service.Contracts;
using Microsoft.Extensions.Configuration;
using Mind.Business.Log;
using Mind.Repository.Common;

namespace Mind.Repository.Mind.Service;

#nullable disable

public class TeamService : iTeamService
{
    private readonly iTeamRepository _repo;
    private readonly IConfiguration Configuration;

    public TeamService(iTeamRepository repo, IConfiguration configuration)
    {
        _repo = repo;
        Configuration = configuration; ;
    }

    LogHistory logHistory = null;
    Audit audit = null;

    public async Task<TeamDTO> AddAsync(TeamDTO teamDTO, int userID)
    {
        Team teamTable = new Team();

        teamTable.TeamName = teamDTO.TeamName;
        teamTable.CustomerID = int.Parse(teamDTO.CustomerID);
        teamTable.AccountID = int.Parse(teamDTO.AccountID);
        teamTable.StartDate = teamDTO.StartDate;
        teamTable.EndDate = teamDTO.EndDate;

        audit = new Audit(Configuration);
        audit.FillAudit(ref teamTable, 0, userID);

        Team resultWithDbID;

        try
        {
            resultWithDbID = await _repo.Add(teamTable);

            logHistory = new LogHistory(Configuration);

            logHistory.AddLogHistory(teamTable, "INSERT", userID);
        }
        catch (Exception)
        {
            return null;
        }

        if (resultWithDbID == null)
        {
            return null;
        }

        teamDTO.ID = resultWithDbID.ID.ToString();

        return teamDTO;
    }

    public bool ExistOrUsed(int? id)
    {
        if (id != null)
        {
            return _repo.Exists((int)id);
        }

        return false;
    }

    public async Task<List<TeamDTO>> GetAll()
    {
        var entity = await _repo.GetAll(teams => teams.Select(team => new TeamDTO
        {
            ID = team.ID.ToString(),
            TeamName = team.TeamName,
            CustomerID = team.CustomerID.ToString(),
            CustomerName = team.customer.CustomerName,
            AccountID = team.AccountID.ToString(),
            UserName = team.customer.account.FirstName + " " + team.customer.account.LastName,
            StartDate = team.StartDate,
            EndDate= team.EndDate
        }));

        if (entity.Count() == 0)
        {
            return null;
        }

        return entity.ToList();
    }

    public async Task<List<TeamDTO>> GetAllBySearch(string searchString)
    {
        var predicate = PredicateBuilder.New<Team>();

        string[] searchStr = searchString.Split("|");

        foreach (var search in searchStr)
        {
            if (!String.IsNullOrEmpty(search))
            {
                predicate = predicate.Or(x => x.ID.ToString().Contains(search));
                predicate = predicate.Or(x => x.TeamName.Contains(search));
                predicate = predicate.Or(x => x.CustomerID.ToString().Contains(search));
                predicate = predicate.Or(x => x.customer.CustomerName.Contains(search));
                predicate = predicate.Or(x => x.AccountID.ToString().Contains(search));
                predicate = predicate.Or(x => x.customer.account.FirstName.Contains(search));
                predicate = predicate.Or(x => x.customer.account.LastName.Contains(search));
                predicate = predicate.Or(x => x.StartDate.ToString().Contains(search));
                predicate = predicate.Or(x => x.EndDate.ToString().Contains(search));
            }
        }

        var entity = await _repo.GetAll(teams => teams.Select(team => new TeamDTO
        {
            ID = team.ID.ToString(),
            TeamName = team.TeamName,
            CustomerID = team.CustomerID.ToString(),
            CustomerName = team.customer.CustomerName,
            AccountID = team.AccountID.ToString(),
            UserName = team.customer.account.FirstName + " " + team.customer.account.LastName,
            StartDate = team.StartDate,
            EndDate = team.EndDate
        }), predicate);

        if (entity.Count() == 0)
        {
            return null;
        }

        return entity.ToList();
    }

    public async Task<List<DropDownDTO>> GetAllForDropDown()
    {
        var entity = await _repo.GetAll(teams => teams.Select(team => new DropDownDTO
        {
            Code = team.ID.ToString(),
            Name = team.TeamName
        }));

        if (entity.Count() == 0)
        {
            return null;
        }

        return entity.ToList();
    }

    public async Task<TeamDTO> GetDataById(int id)
    {
        var predicate = PredicateBuilder.New<Team>();
        predicate = predicate.Or(x => x.ID == id);

        var team = await _repo.GetAll(teams => teams.Select(team => new TeamDTO
        {
            ID = team.ID.ToString(),
            TeamName = team.TeamName,
            CustomerID = team.CustomerID.ToString(),
            CustomerName = team.customer.CustomerName,
            AccountID = team.AccountID.ToString(),
            UserName = team.customer.account.FirstName + " " + team.customer.account.LastName,
            StartDate = team.StartDate,
            EndDate = team.EndDate
        }), predicate);

        if (team == null)
        {
            return null;
        }

        return team.FirstOrDefault();
    }

    public async Task<TeamDTO> RemoveAsync(int id, int user)
    {
        Team teamTable;
        TeamDTO teamDTO;

        try
        {
            teamTable = await _repo.Get(id);
        }
        catch (Exception)
        {
            teamDTO = new TeamDTO();
            teamDTO.ID = "0";
            return teamDTO;
        }

        if (teamTable == null)
        {
            teamDTO = new TeamDTO();
            teamDTO.ID = "0";
            return teamDTO;
        }

        teamTable.Deferred = true;

        audit = new Audit(Configuration);
        audit.FillAudit(ref teamTable, 2, user);

        Team result;

        try
        {
            result = await _repo.Update(teamTable);

            logHistory = new LogHistory(Configuration);

            logHistory.AddLogHistory(teamTable, "DELETE", user);
        }
        catch (Exception)
        {
            return null;
        }

        teamDTO = new TeamDTO();

        teamDTO.ID = result.ID.ToString();
        teamDTO.TeamName = result.TeamName;
        teamDTO.CustomerID = result.CustomerID.ToString();        
        teamDTO.AccountID = result.AccountID.ToString();
        teamDTO.StartDate = result.StartDate;
        teamDTO.EndDate = result.EndDate;

        return teamDTO;
    }

    public async Task<TeamDTO> UpdateAsync(TeamDTO entity, int user)
    {
        Team teamTable;
        bool sendLog = false;

        try
        {
            teamTable = await _repo.Get(int.Parse(entity.ID));

            if (teamTable.CustomerID != int.Parse(entity.CustomerID))
            {
                sendLog = true;
            }
        }
        catch (Exception)
        {
            TeamDTO team = new TeamDTO();
            team.ID = "0";
            return team;
        }

        if (teamTable == null)
        {
            TeamDTO team = new TeamDTO();
            team.ID = "0";
            return team;
        }

        teamTable.TeamName = entity.TeamName;
        teamTable.CustomerID = int.Parse(entity.CustomerID);
        teamTable.AccountID = int.Parse(entity.AccountID);
        teamTable.StartDate = entity.StartDate;
        teamTable.EndDate = entity.EndDate;

        audit = new Audit(Configuration);
        audit.FillAudit(ref teamTable, 1, user);

        Team result;

        try
        {
            result = await _repo.Update(teamTable);

            if (sendLog)
            {
                logHistory = new LogHistory(Configuration);                

                logHistory.AddLogHistory(teamTable, "UPDATE", user);
            }

        }
        catch (Exception)
        {
            return null;
        }

        return entity;
    }

}
