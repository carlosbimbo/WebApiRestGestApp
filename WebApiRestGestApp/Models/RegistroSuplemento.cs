using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiRestGestApp.Models
{
    [Table("t_05_registro_suplementos", Schema = "public")]
    public class RegistroSuplemento
    {
        [Column("idsuple")]
        public int IdSuple { get; set; }

        [Column("iduser")]
        public Guid IdUser { get; set; }

        [Required]
        [Column("fecha")]
        [MaxLength(10)]
        public string Fecha { get; set; } = string.Empty;

        [Column("tipo_suple")]
        public int? TipoSuple { get; set; }

        [Column("foto")]
        [MaxLength(100)]
        public string? Foto { get; set; }

        [Column("nro_sema")]
        public int? NroSema { get; set; }

        [Column("destinationuri")]
        [MaxLength(200)]
        public string? DestinationUri { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}