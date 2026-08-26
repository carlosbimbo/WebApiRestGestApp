using System.ComponentModel.DataAnnotations;

namespace WebApiRestGestApp.DTOs
{
    public class UsernameRequest
    {
        [Required(ErrorMessage = "El username es obligatorio")]
        public string Username { get; set; } = string.Empty;
    }
}