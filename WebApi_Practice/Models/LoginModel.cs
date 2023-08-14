using System.ComponentModel.DataAnnotations;

namespace WebApi_Practice.Models
{
    public class LoginModel
    {
        [Required]
        public string Username{ get; set; }

        [Required]
        public string Password{ get; set; }
    }
}
