using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 

namespace WebApiRestGestApp.Models
{
    [Table("t_05_agenda_gestacional", Schema = "public")]
    public class AgendaGestacional
    {
        [Column("id")]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [Column("nrosem")]
        [JsonPropertyName("nrosem")]
        public int Nrosem { get; set; }

        [Column("fec_marker")]
        [JsonPropertyName("fec_marker")]
        [MaxLength(10)]
        public string? FecMarker { get; set; }
    }
}