using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // <-- REQUERIDO

namespace WebApiRestGestApp.Models
{
    [Table("t_05_registro_eventos", Schema = "public")]
    public class RegistroEvento
    {
        [Column("ideven")]
        [JsonPropertyName("ideven")]
        public int IdEven { get; set; }

        [Column("iduser")]
        [JsonPropertyName("iduser")]
        public Guid IdUser { get; set; }

        [Column("tipo")]
        [JsonPropertyName("tipo")]
        public int? Tipo { get; set; }

        [Column("fecha")]
        [JsonPropertyName("fecha")]
        [MaxLength(10)]
        public string? Fecha { get; set; }

        [Column("hora")]
        [JsonPropertyName("hora")]
        [MaxLength(10)]
        public string? Hora { get; set; }

        [Column("descrip")]
        [JsonPropertyName("descrip")]
        [MaxLength(2000)]
        public string? Descrip { get; set; }

        [Column("alarma")]
        [JsonPropertyName("alarma")]
        public int? Alarma { get; set; }

        [Column("estado")]
        [JsonPropertyName("estado")]
        public int? Estado { get; set; }
    }
}