using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace noted_database.Models{
 public class Campaign
    {
        [Key]
        public int CampaignId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int MaxPlayers { get; set; }

        [ForeignKey("DmId")]
        public int DmId { get; set; }
    }
}