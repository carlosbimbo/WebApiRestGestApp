using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 

namespace WebApiRestGestApp.Models
{
    [Table("users", Schema = "public")]
    public class User
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("username")]
        [JsonPropertyName("username")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Column("password")]
        [JsonPropertyName("password")]
        [MaxLength(255)]
        public string? Password { get; set; }

        [Column("dni")]
        [JsonPropertyName("dni")]
        [MaxLength(8)]
        public string? Dni { get; set; }

        [Column("nombape")]
        [JsonPropertyName("nombape")]
        [MaxLength(150)]
        public string? Nombape { get; set; }

        [Column("lati")]
        [JsonPropertyName("lati")]
        public double? Lati { get; set; }

        [Column("longi")]
        [JsonPropertyName("longi")]
        public double? Longi { get; set; }

        [Column("altura")]
        [JsonPropertyName("altura")]
        public double? Altura { get; set; }

        [Column("lati_viv")]
        [JsonPropertyName("lati_viv")]
        public double? LatiViv { get; set; }

        [Column("longi_viv")]
        [JsonPropertyName("longi_viv")]
        public double? LongiViv { get; set; }

        [Column("altura_viv")]
        [JsonPropertyName("altura_viv")]
        public double? AlturaViv { get; set; }

        [Column("profileImage")]
        [JsonPropertyName("profileImage")]
        [MaxLength(255)]
        public string? ProfileImage { get; set; }

        [Column("expopushtoken")]
        [JsonPropertyName("expopushtoken")]
        [MaxLength(255)]
        public string? ExpoPushToken { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("email")]
        [JsonPropertyName("email")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Column("feccre_movil")]
        [JsonPropertyName("feccre_movil")]
        [MaxLength(10)]
        public string? Feccre_Movil { get; set; }

        [Column("horacre_movil")]
        [JsonPropertyName("horacre_movil")]
        [MaxLength(10)]
        public string? Horacre_Movil { get; set; }

        [Column("fecupd_movil")]
        [JsonPropertyName("fecupd_movil")]
        [MaxLength(10)]
        public string? Fecupd_Movil { get; set; }

        [Column("horaupd_movil")]
        [JsonPropertyName("horaupd_movil")]
        [MaxLength(10)]
        public string? Horaupd_Movil { get; set; }

        [Column("appversion")]
        [JsonPropertyName("appversion")]
        [MaxLength(10)]
        public string? AppVersion { get; set; }
    }
}