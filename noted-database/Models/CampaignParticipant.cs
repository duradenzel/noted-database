using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace noted_database.Models{
 public class CampaignParticipant
    {
        [Key]
        public int ParticipantId {get; set;}

        [ForeignKey("CampaignId")]
        public int CampaignId { get; set; }


        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public bool IsDm {get; set;}

        public Campaign Campaign { get; set; }

        public User User { get; set; }
    }
}