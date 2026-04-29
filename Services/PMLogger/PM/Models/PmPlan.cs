using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMLogger.PM.Models
{
    public class PmPlan
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
        public string? ChecklistType { get; set; }

        public int? ChecklistVersion { get; set; }

        [StringLength(255)]
        public string? AssignedTechnician { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public DateTime? ScheduledDate { get; set; }

        [StringLength(50)]
        public string? PlanStatus { get; set; } = "Draft";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? BaselineDate { get; set; }

        [StringLength(50)]
        public string? Priority { get; set; } = "Medium";

        [StringLength(50)]
        public string? Shift { get; set; }

        public string? Notes { get; set; }

        public bool ExecutionSubmitted { get; set; } = false;

        public DateTime? ExecutionSubmittedDate { get; set; }

        [StringLength(255)]
        public string? ExecutionSubmittedBy { get; set; }

        [StringLength(255)]
        public string? ExecutedBy { get; set; }

        public DateTime? ExecutedAt { get; set; }

        [StringLength(255)]
        public string? RejectedBy { get; set; }

        public DateTime? RejectedDate { get; set; }

        public string? RejectionReason { get; set; }

        [StringLength(255)]
        public string? OperationApprovedBy { get; set; }

        public DateTime? OperationApprovedDate { get; set; }

        [StringLength(255)]
        public string? SupervisorApprovedBy { get; set; }

        public DateTime? SupervisorApprovedDate { get; set; }

        [StringLength(255)]
        public string? SupervisorReturnedBy { get; set; }

        public DateTime? SupervisorReturnedDate { get; set; }

        public string? ExecutionComment { get; set; }

        public string? OperationComment { get; set; }

        public string? SupervisorComment { get; set; }

        public int? ScheduleId { get; set; }

        public int CompanyId { get; set; }

        // Navigation properties
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        [ForeignKey("ScheduleId")]
        public virtual PmMachineSchedule? PmMachineSchedule { get; set; }

        public virtual ICollection<PmPlanStatusHistory> PmPlanStatusHistories { get; set; } = new List<PmPlanStatusHistory>();
    }
}

