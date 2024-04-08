using noted_database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Diagnostics;

namespace noted_database.Data.Repositories
{
    public class SessionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SessionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Session>> GetSessionsByCampaignId(int campaignId)
        {
            return await _dbContext.Sessions
                .Where(s => s.CampaignId == campaignId)
                .ToListAsync();

          
        }

         public async Task<Session> GetSessionById(int id)
        {
            return await _dbContext.Sessions.FirstOrDefaultAsync(s => s.SessionId == id);
        }
      
        public async Task<bool> InsertSession(Session session)
        {      
                try
                {
                    _dbContext.Sessions.Add(session);
                    await _dbContext.SaveChangesAsync();
                             
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            
        }

       public async Task<bool> UpdateSession(Session session)
        {
            try
            {
                var existingSession = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.SessionId == session.SessionId);

                if (existingSession == null)
                {
                    return false; 
                }

                
                existingSession.Title = session.Title;
                existingSession.Summary = session.Summary;

                await _dbContext.SaveChangesAsync();
                
                return true; 
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database update error: " + ex.Message);
                return false; 
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
                return false; 
            }
        }

        public async Task<bool> DeleteSession(int id)
        {
            try
            {
                
                var existingSession = await _dbContext.Sessions.FindAsync(id);

                if (existingSession == null)
                {
                    return false; 
                }

              
                _dbContext.Sessions.Remove(existingSession);
                await _dbContext.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {

                Console.WriteLine("An error occurred while deleting the session: " + ex.Message);
                return false; 
            }
        }

    }
}
