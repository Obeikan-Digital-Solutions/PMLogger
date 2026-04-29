using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMLogger.PM.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Passcode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Team { get; set; }

        [StringLength(500)]
        public string? AreaAccess { get; set; }

        public bool IsActive { get; set; } = true;

        public int? CompanyId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }
    }
}

