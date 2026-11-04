using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApiRestGestApp.DTOs
{
    public class SaveExpoTokenUsernameRequest
    {
        [Required(ErrorMessage = "El parámetro username es obligatorio")]
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "El parámetro expopushtoken es obligatorio")]
        [JsonPropertyName("expopushtoken")]
        public string ExpoPushToken { get; set; } = string.Empty;
    }
}