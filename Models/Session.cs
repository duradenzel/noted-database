using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace noted_database.Models{
 public class Session
    {
        [Key]
        public int SessionId { get; set; }

        public int CampaignId { get; set; }

        public DateTime Date { get; set; }

        public string Summary { get; set; }
    }
}