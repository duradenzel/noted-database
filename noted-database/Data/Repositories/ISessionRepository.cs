using noted_database.Models;
namespace noted_database.Data.Repositories
{
      public interface ISessionRepository
    {
        Task<List<Session>> GetSessionsByCampaignId(int campaignId);
        Task<Session> GetSessionById(int id);
        Task<bool> InsertSession(Session session);
        Task<bool> UpdateSession(Session session);
        Task<bool> DeleteSession(int id);
    }
}