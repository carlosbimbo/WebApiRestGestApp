using System.ComponentModel.DataAnnotations;

namespace WebApiRestGestApp.DTOs
{
    public class RecoverPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        public string Nombape { get; set; } = string.Empty;
    }
}