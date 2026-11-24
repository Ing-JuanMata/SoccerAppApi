using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using API.DTOs.Input;
using API.DTOs.Output;

namespace API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class TeamController : Controller
    {
        private readonly APIDbContext _context;
        public TeamController(APIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeams([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Team.AsQueryable();
            int totalCount = query.CountAsync().Result;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (pageNumber > totalPages) return NotFound();
            var teams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(team => new TeamOutput
                {
                    Country = team.Country,
                    Name = team.Name,
                    PlayersQuantity = team.PlayersQuantity,
                    Enabled = team.Enabled,
                    Id = team.Id,
                    LastUpdated = team.LastUpdated
                }).ToListAsync();
            var response = new PagedResponse<TeamOutput>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount,
                Content = teams
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetTeamById([FromRoute] int id)
        {
            TeamLeagueOutput output = await _context.Team.Include(x => x.TeamLeagues).ThenInclude(x => x.League).Select(
                t => new TeamLeagueOutput
                {
                    Id = t.Id,
                    Name = t.Name,
                    Leagues = t.TeamLeagues
                    .Select(tl => tl.League.Name)
                    .ToList()
                }
                ).FirstAsync(x => x.Id == id);
            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> AddTeam([FromBody] TeamInput team)
        {
            Team newTeam = new()
            {
                Country = team.Country,
                Name = team.Name,
                PlayersQuantity = team.PlayersQuantity,
                Enabled = true,
                LastUpdated = DateTime.Now
            };
            await _context.Team.AddAsync(newTeam);
            await _context.SaveChangesAsync();
            TeamOutput output = _context.Team
                .Select(team => new TeamOutput
                {
                    Id = team.Id,
                    Name = team.Name,
                    Country = team.Country,
                    PlayersQuantity = team.PlayersQuantity,
                    Enabled = team.Enabled,
                    LastUpdated = team.LastUpdated,

                }).Where(x => x.Name == team.Name).First();
            return Ok(output);
        }

        [HttpPost]
        [Route("{id:int}/leagues")]
        public async Task<IActionResult> SyncTeamLeagues([FromBody] TeamLeagueInput leagues, int id)
        {
            Team? team = await _context.Team.Include(x => x.TeamLeagues).FirstOrDefaultAsync(x => x.Id == id);
            if (team == null)
                return NotFound();
            var leaguesToRemove = team.TeamLeagues.Where(x => !leagues.LeaguesIds.Contains(x.IdLeague)).ToList();
            var existingLeaguesIds = team.TeamLeagues.Select(x => x.IdLeague).ToList();
            var leaguesToAdd = _context.League.Where(x => leagues.LeaguesIds.Contains(x.Id) && !existingLeaguesIds.Contains(x.Id))
                .Select(l => new TeamLeague
                {
                    IdTeam = team.Id,
                    IdLeague = l.Id,
                    League = l,
                    Team = team,
                    Enabled = true,
                    LastUpdated = DateTime.Now
                }).ToList();
            if (leaguesToAdd.Count == 0 && leaguesToRemove.Count == 0) return NoContent();

            foreach (var leagueToRemove in leaguesToRemove)
            {
                team.TeamLeagues.Remove(leagueToRemove);
            }
            foreach (var leagueToAdd in leaguesToAdd)
            {
                team.TeamLeagues.Add(leagueToAdd);
            }

            await _context.SaveChangesAsync();
            TeamLeagueOutput output = await _context.Team.Include(x => x.TeamLeagues).ThenInclude(x => x.League).Select(
                t => new TeamLeagueOutput
                {
                    Id = t.Id,
                    Name = t.Name,
                    Leagues = t.TeamLeagues
                    .Select(tl => tl.League.Name)
                    .ToList()
                }
                ).FirstAsync(x => x.Id == id);
            return Ok(output);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateTeam([FromRoute] int id, [FromBody] TeamInput newTeam)
        {
            var team = await _context.Team.FindAsync(id);

            if (team == null)
                return NotFound();

            team.Name = newTeam.Name;
            team.Country = newTeam.Country;
            team.PlayersQuantity = newTeam.PlayersQuantity;
            team.Enabled = newTeam.Enabled;
            team.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();
            TeamOutput output = new()
            {
                Country = team.Country,
                Name = team.Name,
                PlayersQuantity = team.PlayersQuantity,
                Id = team.Id,
                Enabled = team.Enabled,
                LastUpdated = team.LastUpdated
            };
            return Ok(output);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteTeam([FromRoute] int id)
        {
            var team = await _context.Team.FindAsync(id);

            if (team == null)
                return NotFound();

            team.Enabled = false;
            team.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok();

        }
    }
}