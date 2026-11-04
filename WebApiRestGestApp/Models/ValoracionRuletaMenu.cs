using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // <-- NUEVO USING REQUERIDO

namespace WebApiRestGestApp.Models
{
    [Table("t_05_valoracion_ruleta_menu", Schema = "public")]
    public class ValoracionRuletaMenu
    {
        [Column("iduser")]
        [JsonPropertyName("iduser")] // Buena práctica para asegurar el mapeo exacto
        public string IdUser { get; set; } = null!;

        [Column("idmenu")]
        [JsonPropertyName("idmenu")]
        public int IdMenu { get; set; }

        [Column("flag_valora")]
        [JsonPropertyName("flag_valora")] 
        public int? FlagValora { get; set; }

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