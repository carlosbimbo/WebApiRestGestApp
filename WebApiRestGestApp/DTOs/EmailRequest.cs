using System.ComponentModel.DataAnnotations;

namespace WebApiRestGestApp.DTOs
{
    public class EmailRequest
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;
    }
}