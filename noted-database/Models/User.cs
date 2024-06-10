using System.ComponentModel.DataAnnotations;

namespace noted_database.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }
       
    }
}