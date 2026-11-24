using API.Data;
using API.DTOs.Input;
using API.DTOs.Output;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class LeaguesController : Controller
    {
        private readonly APIDbContext _context;
        public LeaguesController(APIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLeagues([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.League.AsQueryable();
            int totalCount = query.CountAsync().Result;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (pageNumber > totalPages) return NotFound();
            var leagues = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(league => new LeagueOutput
                {
                    Country = league.Country,
                    Name = league.Name,
                    StartDate = league.StartDate,
                    EndDate = league.EndDate,
                    Enabled = league.Enabled,
                    Id = league.Id,
                    LastUpdated = league.LastUpdated
                }).ToListAsync();
            var response = new PagedResponse<LeagueOutput>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount,
                Content = leagues
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetLeagueById([FromRoute] int id)
        {
            LeagueTeamOutput output = await _context.League.Include(x => x.TeamLeagues).ThenInclude(x => x.Team).Select(
                l => new LeagueTeamOutput
                {
                    Id = l.Id,
                    Name = l.Name,
                    Teams = l.TeamLeagues
                    .Select(tl => tl.Team.Name)
                    .ToList()
                }
                ).FirstAsync(x => x.Id == id);
            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> AddLeague([FromBody] LeagueInput league)
        {
            League newLeague = new()
            {
                Name = league.Name,
                Country = league.Country,
                StartDate = league.StartDate,
                EndDate = league.EndDate,
                Enabled = true,
                LastUpdated = DateTime.Now
            };
            await _context.League.AddAsync(newLeague);
            await _context.SaveChangesAsync();
            LeagueOutput output = _context.League.Select(l => new LeagueOutput
            {
                Id = l.Id,
                Country = l.Country,
                Name = l.Name,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Enabled = l.Enabled,
                LastUpdated = l.LastUpdated
            }).Where(x => x.Name == league.Name).First();
            return Ok(output);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateLeague([FromRoute] int id, [FromBody] LeagueInput newLeague)
        {
            var league = await _context.League.FindAsync(id);

            if (league == null)
                return NotFound();

            league.Name = newLeague.Name;
            league.Country = newLeague.Country;
            league.StartDate = newLeague.StartDate;
            league.EndDate = newLeague.EndDate;
            league.Enabled = newLeague.Enabled;
            league.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();
            LeagueOutput output = _context.League.Select(l => new LeagueOutput
            {
                Id = l.Id,
                Country = l.Country,
                Name = l.Name,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Enabled = l.Enabled,
                LastUpdated = l.LastUpdated
            }).Where(x => x.Name == league.Name).First();
            return Ok(output);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteLeague([FromRoute] int id)
        {
            var league = await _context.League.FindAsync(id);

            if (league == null)
                return NotFound();

            league.Enabled = false;
            league.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}