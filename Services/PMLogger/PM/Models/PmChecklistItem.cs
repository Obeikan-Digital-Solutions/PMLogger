using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMLogger.PM.Models
{
    public class PmChecklistItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ItemText { get; set; } = string.Empty;

        public int SequenceOrder { get; set; }

        public int DurationMinutes { get; set; }

        [StringLength(255)]
        public string? Assembly { get; set; }

        [StringLength(255)]
        public string? Unit { get; set; }

        [StringLength(500)]
        public string? LubricantOrTools { get; set; }

        public int? PpeCodeId { get; set; }

        [StringLength(100)]
        public string? Frequency { get; set; }

        public int VersionNumber { get; set; } = 1;

        public bool IsNote { get; set; } = false;

        public bool IsLubricationSummary { get; set; } = false;

        public int AreaId { get; set; }

        public int MachineId { get; set; }

        public int ChecklistTypeId { get; set; }

        public int CompanyId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("AreaId")]
        public virtual PmArea PmArea { get; set; } = null!;

        [ForeignKey("MachineId")]
        public virtual PmMachine PmMachine { get; set; } = null!;

        [ForeignKey("ChecklistTypeId")]
        public virtual PmChecklistType PmChecklistType { get; set; } = null!;

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        [ForeignKey("PpeCodeId")]
        public virtual PmPpeCode? PmPpeCode { get; set; }
    }
}

