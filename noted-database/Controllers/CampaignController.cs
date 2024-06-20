using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using noted_database.Data;
using noted_database.Models;
using noted_database.Services;
using Microsoft.AspNetCore.Authorization;
namespace noted_database.Controllers{

    [Route("campaigns")]
    [Authorize]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly UserService _userService;

        public CampaignController(ICampaignService campaignService, UserService userService)
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
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaign(int id)
        {
            var campaign = await _campaignService.GetCampaignById(id);
            if (campaign == null)
            {
                return NotFound();
            }

            return Ok(campaign);
        }

       
        [HttpPost]
        public async Task<IActionResult> InsertCampaign([FromBody] Campaign campaign, [FromQuery] string email)
        {
            var user = await _userService.GetUserByEmail(email);
             if(user != null){
                 campaign.DmId = user.UserId;
               await _campaignService.InsertCampaign(campaign);
             }
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampaign(int id, [FromBody] Campaign campaign)
        {
            var existingCampaign = await _campaignService.GetCampaignById(id); 
            if (existingCampaign != null){
                await _campaignService.UpdateCampaign(campaign);
                
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            var result = await _campaignService.DeleteCampaign(id);

            if (result)
            {
                return Ok(); 
            }
            else
            {
                return NotFound();
            }
        }

    }

}

