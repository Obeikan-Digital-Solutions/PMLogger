using System.ComponentModel.DataAnnotations;

namespace PMLogger.PM.Models
{
    public class PmPlanDateChange
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string PlanName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MachineName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? AreaName { get; set; }

        [StringLength(255)]
        public string? ChecklistType { get; set; }

        public DateTime? OriginalDate { get; set; }

        public DateTime? PreviousDate { get; set; }

        public DateTime? NewDate { get; set; }

        [StringLength(255)]
        public string? ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public string? Reason { get; set; }
    }
}

