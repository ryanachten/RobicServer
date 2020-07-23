using System.ComponentModel.DataAnnotations;

namespace RobicServer.Models.DTOs
{
    public class UserForRegisterDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 10 characters")]
        public string Password { get; set; }
    }
}