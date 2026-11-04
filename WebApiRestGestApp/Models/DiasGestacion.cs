using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 

namespace WebApiRestGestApp.Models
{
    [Table("t_05_dias_gestacion", Schema = "public")]
    public class DiasGestacion
    {
        [Column("id_diasg")]
        [JsonPropertyName("id_diasg")]
        public int IdDiasg { get; set; }

        [Column("iduser")]
        [JsonPropertyName("iduser")]
        public Guid IdUser { get; set; }

        [Column("nroseman")]
        [JsonPropertyName("nroseman")]
        public int NroSeman { get; set; }

        [Column("fec_seman")]
        [JsonPropertyName("fec_seman")]
        [MaxLength(10)]
        public string? FecSeman { get; set; }

        [Column("fec_diagesta")]
        [JsonPropertyName("fec_diagesta")]
        [MaxLength(10)]
        public string? FecDiagesta { get; set; }

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

        [Column("video_asignado")]
        [JsonPropertyName("video_asignado")]
        public int? Video_Asignado { get; set; } 
    }
}