using System.ComponentModel.DataAnnotations;

namespace PMLogger.PM.Models
{
    public class PmChecklistType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<PmChecklistItem> PmChecklistItems { get; set; } = new List<PmChecklistItem>();
    }
}

