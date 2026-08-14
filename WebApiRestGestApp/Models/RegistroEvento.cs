using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiRestGestApp.Models
{
    [Table("t_05_registro_eventos", Schema = "public")]
    public class RegistroEvento
    {
        [Column("ideven")]
        public int IdEven { get; set; }

        [Column("iduser")]
        public Guid IdUser { get; set; }

        [Column("tipo")]
        public int? Tipo { get; set; }

        [Column("fecha")]
        [MaxLength(10)]
        public string? Fecha { get; set; }

        [Column("hora")]
        [MaxLength(10)]
        public string? Hora { get; set; }

        [Column("descrip")]
        [MaxLength(2000)]
        public string? Descrip { get; set; }

        [Column("alarma")]
        public int? Alarma { get; set; }

        [Column("estado")]
        public int? Estado { get; set; }
    }
}