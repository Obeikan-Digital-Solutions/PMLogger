using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMLogger.PM.Models
{
    public class PmMachineSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string PlanName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string AreaName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MachineName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? AssignedTechnician { get; set; }

        public DateTime? ScheduledDate { get; set; }

        [StringLength(50)]
        public string? Shift { get; set; }

        [StringLength(50)]
        public string? Priority { get; set; } = "Medium";

        public string? Notes { get; set; }

        [StringLength(255)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public int CompanyId { get; set; }

        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        public virtual ICollection<PmPlan> PmPlans { get; set; } = new List<PmPlan>();
    }
}

