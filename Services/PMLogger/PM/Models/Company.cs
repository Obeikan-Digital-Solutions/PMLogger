using System.ComponentModel.DataAnnotations;

namespace PMLogger.PM.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CompanyCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<PmPlan> PmPlans { get; set; } = new List<PmPlan>();
        public virtual ICollection<PmArea> PmAreas { get; set; } = new List<PmArea>();
        public virtual ICollection<PmMachine> PmMachines { get; set; } = new List<PmMachine>();
        public virtual ICollection<PmChecklistItem> PmChecklistItems { get; set; } = new List<PmChecklistItem>();
        public virtual ICollection<ChecklistExecution> ChecklistExecutions { get; set; } = new List<ChecklistExecution>();
        public virtual ICollection<PmMachineRunningHour> PmMachineRunningHours { get; set; } = new List<PmMachineRunningHour>();
        public virtual ICollection<PmMachineSchedule> PmMachineSchedules { get; set; } = new List<PmMachineSchedule>();
    }
}

