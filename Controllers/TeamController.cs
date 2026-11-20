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
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await _context.Team
                .Select(team => new TeamOutput
                {
                    Country = team.Country,
                    Name = team.Name,
                    PlayersQuantity = team.PlayersQuantity,
                    Enabled = team.Enabled,
                    Id = team.Id,
                    LastUpdated = team.LastUpdated
                }).ToListAsync();
            return Ok(teams);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetTeamById([FromRoute] int id)
        {
            TeamLeagueOutput output = await _context.Team.Include(x => x.TeamLeagues).ThenInclude(x => x.League).Select(
                t => new TeamLeagueOutput
                {
                    IdTeam = t.Id,
                    Name = t.Name,
                    Leagues = t.TeamLeagues
                    .Select(tl => tl.League.Name)
                    .ToList()
                }
                ).FirstAsync(x => x.IdTeam == id);
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
        public async Task<IActionResult> AddTeamLeague([FromBody] TeamLeagueInput leagues, int id)
        {
            Team? team = await _context.Team.Include(x => x.TeamLeagues).FirstOrDefaultAsync(x => x.Id == id);
            if (team == null)
                return NotFound();
            var newIds = leagues.LeaguesIds.Except(team.TeamLeagues.Select(x => x.IdLeague)).ToList();
            if (newIds.Count == 0) return NoContent();

            foreach (int idLeague in newIds)
            {
                League? league = await _context.League.FirstOrDefaultAsync(x => x.Id == idLeague);
                if (league == null) return NotFound($"Id League {idLeague} Not Found");
                team.TeamLeagues.Add(new TeamLeague
                {
                    Team = team,
                    League = league,
                    LastUpdated = DateTime.Now,
                    Enabled = true
                });
            }

            await _context.SaveChangesAsync();
            TeamLeagueOutput output = await _context.Team.Include(x => x.TeamLeagues).ThenInclude(x => x.League).Select(
                t => new TeamLeagueOutput
                {
                    IdTeam = t.Id,
                    Name = t.Name,
                    Leagues = t.TeamLeagues
                    .Select(tl => tl.League.Name)
                    .ToList()
                }
                ).FirstAsync(x => x.IdTeam == id);
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