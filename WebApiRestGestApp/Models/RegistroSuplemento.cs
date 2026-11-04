using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApiRestGestApp.Models
{
    [Table("t_05_registro_suplementos", Schema = "public")]
    public class RegistroSuplemento
    {
        [Column("idsuple")]
        [JsonPropertyName("idsuple")]
        public int IdSuple { get; set; }

        [Column("iduser")]
        [JsonPropertyName("iduser")]
        public Guid IdUser { get; set; }

        [Required]
        [Column("fecha")]
        [JsonPropertyName("fecha")]
        [MaxLength(10)]
        public string Fecha { get; set; } = string.Empty;

        [Column("tipo_suple")]
        [JsonPropertyName("tipo_suple")]
        public int? TipoSuple { get; set; }

        [Column("foto")]
        [JsonPropertyName("foto")]
        [MaxLength(100)]
        public string? Foto { get; set; }

        [Column("nro_sema")]
        [JsonPropertyName("nro_sema")]
        public int? NroSema { get; set; }

        [Column("destinationuri")]
        [JsonPropertyName("destinationuri")]
        [MaxLength(200)]
        public string? DestinationUri { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

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