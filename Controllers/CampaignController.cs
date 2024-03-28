using System.Diagnostics;
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
        private readonly UserService _userService;

        public CampaignController(CampaignService campaignService, UserService userService)
        {
            _campaignService = campaignService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCampaigns([FromQuery] string email)
        {
            var campaigns = await _campaignService.GetCampaignsByEmail(email);
            return Ok(new { Campaigns = campaigns });
        }

        [HttpPost]
        public async Task<IActionResult> InsertCampaign([FromBody] Campaign campaign, [FromQuery] string email)
        {
            Debug.WriteLine("Received Email: " + email);
            Debug.WriteLine("Received Campaign title: " +campaign.Title);


            var user = await _userService.GetUserByEmail(email);
             if(user.UserId != null){
                 campaign.DmId = user.UserId;
               await _campaignService.InsertCampaign(campaign);
             }
            return Ok();
        }

    }

}

