using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using noted_database.Data;
using noted_database.Models;
using noted_database.Services;
namespace noted_database.Controllers{

 [ApiController]
    [Route("campaigns")]
    public class CampaignController : ControllerBase
    {
        private readonly CampaignService _campaignService;

        public CampaignController(CampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCampaigns([FromQuery] string email)
        {
            var campaigns = await _campaignService.GetCampaignsByEmailAsync(email);
            return Ok(new { Campaigns = campaigns });
        }
    }

}

