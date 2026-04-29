using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMLogger.PM.Models
{
    public class PmPlanStatusHistory
    {
        [Key]
        public int Id { get; set; }

        public int PlanId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FromStatus { get; set; }

        [StringLength(100)]
        public string? ToStatus { get; set; }

        [StringLength(255)]
        public string? Actor { get; set; }

        [StringLength(50)]
        public string? ActorRole { get; set; }

        public string? Comment { get; set; }

        public string? Details { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("PlanId")]
        public virtual PmPlan PmPlan { get; set; } = null!;
    }
}

