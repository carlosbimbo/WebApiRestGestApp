using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiRestGestApp.Models
{
    [Table("t_05_dias_gestacion", Schema = "public")]
    public class DiasGestacion
    {
        [Column("id_diasg")]
        public int IdDiasg { get; set; }

        [Column("iduser")]
        public Guid IdUser { get; set; }

        [Column("nroseman")]
        public int NroSeman { get; set; }

        [Column("fec_seman")]
        [MaxLength(10)]
        public string? FecSeman { get; set; }

        [Column("fec_diagesta")]
        [MaxLength(10)]
        public string? FecDiagesta { get; set; }
    }
}