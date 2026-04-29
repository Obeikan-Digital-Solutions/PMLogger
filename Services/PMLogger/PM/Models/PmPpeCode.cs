using System.ComponentModel.DataAnnotations;

namespace PMLogger.PM.Models
{
    public class PmPpeCode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<PmChecklistItem> PmChecklistItems { get; set; } = new List<PmChecklistItem>();
    }
}

