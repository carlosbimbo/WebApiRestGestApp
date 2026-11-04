using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiRestGestApp.Models
{
    [Table("alarm_execution")]
    public class AlarmExecution
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("idalar")]
        public int IdAlar { get; set; }

        [Column("fecha", TypeName = "date")]
        public DateTime Fecha { get; set; }

        // NUEVAS VARIABLES AGREGADAS
        [Column("respu_expo")]
        [MaxLength(500)]
        public string? RespuExpo { get; set; }

        [Column("nro_reinte")]
        public int NroReinte { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}