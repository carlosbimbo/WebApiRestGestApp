using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApiRestGestApp.Models
{
    [Table("t_05_etapa_gestacional", Schema = "public")]
    public class EtapaGestacional
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [Column("opcgesta")]
        [JsonPropertyName("opcgesta")]
        public int? OpcGesta { get; set; }

        [Column("fur")]
        [JsonPropertyName("fur")]
        [MaxLength(10)]
        public string? Fur { get; set; }

        [Column("fec_proba_parto")]
        [JsonPropertyName("fec_proba_parto")]
        [MaxLength(10)]
        public string? FecProbaParto { get; set; }

        [Column("eco_nro_sem_emb")]
        [JsonPropertyName("eco_nro_sem_emb")]
        public int? EcoNroSemEmb { get; set; }

        [Column("eco_nro_dias_emb")]
        [JsonPropertyName("eco_nro_dias_emb")]
        public int? EcoNroDiasEmb { get; set; }

        [Column("hemoglo")]
        [JsonPropertyName("hemoglo")]
        [MaxLength(10)]
        public string? Hemoglo { get; set; }

        [Column("calcu_nrosema")]
        [JsonPropertyName("calcu_nrosema")]
        public int? CalcuNroSema { get; set; }

        [Column("calcu_nrodias")]
        [JsonPropertyName("calcu_nrodias")]
        public int? CalcuNroDias { get; set; }

        [Column("calcu_nrodias_parto")]
        [JsonPropertyName("calcu_nrodias_parto")]
        public int? CalcuNroDiasParto { get; set; }

        [Column("calcu_fecaprox_parto")]
        [JsonPropertyName("calcu_fecaprox_parto")]
        [MaxLength(10)]
        public string? CalcuFecAproxParto { get; set; }

        [Column("eco_fechaori")]
        [JsonPropertyName("eco_fechaori")]
        [MaxLength(10)]
        public string? EcoFechaOri { get; set; }

        [Column("nroseman_real")]
        [JsonPropertyName("nroseman_real")]
        public int? Nroseman_Real { get; set; }

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