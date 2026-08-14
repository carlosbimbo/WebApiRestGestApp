using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiRestGestApp.Models
{
    [Table("users", Schema = "public")]
    public class User
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("username")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Column("password")]
        [MaxLength(255)]
        public string? Password { get; set; }

        [Column("dni")]
        [MaxLength(8)]
        public string? Dni { get; set; }

        [Column("nombape")]
        [MaxLength(150)]
        public string? Nombape { get; set; }

        [Column("lati")]
        public double? Lati { get; set; }

        [Column("longi")]
        public double? Longi { get; set; }

        [Column("altura")]
        public double? Altura { get; set; }

        [Column("lati_viv")]
        public double? LatiViv { get; set; }

        [Column("longi_viv")]
        public double? LongiViv { get; set; }

        [Column("altura_viv")]
        public double? AlturaViv { get; set; }

        [Column("profileImage")] 
        [MaxLength(255)]
        public string? ProfileImage { get; set; }

        [Column("expopushtoken")]
        [MaxLength(255)]
        public string? ExpoPushToken { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("email")]
        [MaxLength(100)]
        public string? Email { get; set; }
    }
}