using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 

namespace WebApiRestGestApp.Models
{
    [Table("t_05_registro_hemoglobina", Schema = "public")]
    public class RegistroHemoglobina
    {
        [Column("idh")]
        [JsonPropertyName("idh")]
        public int Idh { get; set; }

        [Column("iduser")]
        [JsonPropertyName("iduser")]
        public Guid IdUser { get; set; }

        [Column("hemo")]
        [JsonPropertyName("hemo")]
        [MaxLength(10)]
        public string? Hemo { get; set; }

        [Column("fecha")]
        [JsonPropertyName("fecha")]
        [MaxLength(10)]
        public string? Fecha { get; set; }

        [Column("hora")]
        [JsonPropertyName("hora")]
        [MaxLength(10)]
        public string? Hora { get; set; }

        [Column("lat")]
        [JsonPropertyName("lat")]
        [MaxLength(100)]
        public string? Lat { get; set; }

        [Column("long")]
        [JsonPropertyName("long")]
        [MaxLength(100)]
        public string? Long { get; set; }

        [Column("altu")]
        [JsonPropertyName("altu")]
        [MaxLength(100)]
        public string? Altu { get; set; }

        [Column("estado")]
        [JsonPropertyName("estado")]
        public int? Estado { get; set; }

        [Column("obs")]
        [JsonPropertyName("obs")]
        [MaxLength(1000)]
        public string? Obs { get; set; }

        [Column("feccre_movil")]
        [JsonPropertyName("feccre_movil")]
        [MaxLength(10)]
        public string? Feccre_Movil { get; set; }

        [Column("horacre_movil")]
        [JsonPropertyName("horacre_movil")]
        [MaxLength(10)]
        public string? Horacre_Movil { get; set; }

        [Column("appversion")]
        [JsonPropertyName("appversion")]
        [MaxLength(10)]
        public string? AppVersion { get; set; }
    }
}