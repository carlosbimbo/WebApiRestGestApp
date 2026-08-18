using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiRestGestApp.Models
{
    [Table("t_05_agenda_gestacional", Schema = "public")]
    public class AgendaGestacional
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("nrosem")]
        public int Nrosem { get; set; }

        [Column("fec_marker")]
        [MaxLength(10)]
        public string? FecMarker { get; set; }
    }
}