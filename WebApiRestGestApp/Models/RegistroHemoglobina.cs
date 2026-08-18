using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiRestGestApp.Models
{
    [Table("t_05_registro_hemoglobina", Schema = "public")]
    public class RegistroHemoglobina
    {
        [Column("idh")]
        public int Idh { get; set; }

        [Column("iduser")]
        public Guid IdUser { get; set; }

        [Column("hemo")]
        [MaxLength(10)]
        public string? Hemo { get; set; }

        [Column("fecha")]
        [MaxLength(10)]
        public string? Fecha { get; set; }

        [Column("hora")]
        [MaxLength(10)]
        public string? Hora { get; set; }

        [Column("lat")]
        [MaxLength(100)]
        public string? Lat { get; set; }

        [Column("long")]
        [MaxLength(100)]
        public string? Long { get; set; }

        [Column("altu")]
        [MaxLength(100)]
        public string? Altu { get; set; }

        [Column("estado")]
        public int? Estado { get; set; }

        [Column("obs")]
        [MaxLength(1000)]
        public string? Obs { get; set; }
    }
}