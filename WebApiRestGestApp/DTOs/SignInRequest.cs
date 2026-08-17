using System.ComponentModel.DataAnnotations;

namespace WebApiRestGestApp.DTOs
{
    public class SignInRequest
    {
        [Required(ErrorMessage = "El username es obligatorio")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "El password es obligatorio")]
        public string Password { get; set; } = string.Empty;
    }
}