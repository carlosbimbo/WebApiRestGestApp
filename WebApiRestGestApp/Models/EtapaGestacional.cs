using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiRestGestApp.Models
{
    [Table("t_05_etapa_gestacional", Schema = "public")]
    public class EtapaGestacional
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("opcgesta")]
        public int? OpcGesta { get; set; }

        [Column("fur")]
        [MaxLength(10)]
        public string? Fur { get; set; }

        [Column("fec_proba_parto")]
        [MaxLength(10)]
        public string? FecProbaParto { get; set; }

        [Column("eco_nro_sem_emb")]
        public int? EcoNroSemEmb { get; set; }

        [Column("eco_nro_dias_emb")]
        public int? EcoNroDiasEmb { get; set; }

        [Column("hemoglo")]
        [MaxLength(10)]
        public string? Hemoglo { get; set; }

        [Column("calcu_nrosema")]
        public int? CalcuNroSema { get; set; }

        [Column("calcu_nrodias")]
        public int? CalcuNroDias { get; set; }

        [Column("calcu_nrodias_parto")]
        public int? CalcuNroDiasParto { get; set; }

        [Column("calcu_fecaprox_parto")]
        [MaxLength(10)]
        public string? CalcuFecAproxParto { get; set; }

        [Column("eco_fechaori")]
        [MaxLength(10)]
        public string? EcoFechaOri { get; set; }
    }
}