using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApiRestGestApp.DTOs
{
    public class SaveExpoTokenRequest
    {
        [Required(ErrorMessage = "El userId es obligatorio")]
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty; // Cambiado a string

        [Required(ErrorMessage = "El expopushtoken es obligatorio")]
        [JsonPropertyName("expopushtoken")]
        public string ExpoPushToken { get; set; } = string.Empty;
    }
}