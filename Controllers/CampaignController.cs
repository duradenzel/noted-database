using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using noted_database.Data;
using noted_database.Models;
namespace noted_database.Controllers;

[ApiController]
[Route("api/campaign")]
public class CampaignController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public CampaignController(ApplicationDbContext dbcontext)
    {
        _dbContext = dbcontext;
    }

   [HttpGet("get")]
    public async Task<IActionResult> GetCampaigns([FromQuery] string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new User { Email = email };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        var userParticipations = await _dbContext.CampaignParticipants
            .Where(cp => cp.UserId == user.UserId)
            .Select(cp => cp.CampaignId)
            .ToListAsync();

        var campaigns = await _dbContext.Campaigns
            .Where(c => userParticipations.Contains(c.CampaignId))
            .ToListAsync();

        return Ok(new { Campaigns = campaigns });
    }

}

