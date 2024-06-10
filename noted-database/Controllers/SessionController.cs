using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using noted_database.Data;
using noted_database.Models;
using noted_database.Services;
namespace noted_database.Controllers{

    [ApiController]
    [Route("sessions")]
    public class SessionController : ControllerBase
    {
        private readonly SessionService _sessionService;
        private readonly CampaignService _campaignService;
        public SessionController(SessionService sessionService, CampaignService campaignService)
        {
            _sessionService = sessionService;
            _campaignService = campaignService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSessions([FromQuery] int campaignId)
        {
           var sessions = await _sessionService.GetSessionsByCampaignId(campaignId);
           return Ok(new {Sessions = sessions});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id)
        {
            var session = await _sessionService.GetSessionById(id);
            if (session == null)
            {
                return NotFound();
            }

            return Ok(session);
        }

       
        [HttpPost]
        public async Task<IActionResult> InsertSession([FromBody] Session session)
        {
            await _sessionService.InsertSession(session);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession(int id, [FromBody] Session session)
        {
            var existingSession= await _sessionService.GetSessionById(id); 
            if (existingSession != null){
                await _sessionService.UpdateSession(session);
                
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var result = await _sessionService.DeleteSession(id);

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

