using noted_database.Data.Repositories;
using noted_database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace noted_database.Services
{
    public class SessionService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;

        public SessionService(IUserRepository userRepository, ISessionRepository sessionRepository)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<List<Session>> GetSessionsByCampaignId(int campaignId)
        {
            return await _sessionRepository.GetSessionsByCampaignId(campaignId);
        }

        public async Task<Session> GetSessionById(int id)
        {
            return await _sessionRepository.GetSessionById(id);
        }

        public async Task<bool> InsertSession(Session session)
        {
            return await _sessionRepository.InsertSession(session);
        }

        public async Task<bool> UpdateSession(Session session)
        {
            return await _sessionRepository.UpdateSession(session);
        }

        public async Task<bool> DeleteSession(int id)
        {
            return await _sessionRepository.DeleteSession(id);
        }
    }
}
