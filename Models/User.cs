using System.ComponentModel.DataAnnotations;

namespace YourProjectName.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }
       
    }
}