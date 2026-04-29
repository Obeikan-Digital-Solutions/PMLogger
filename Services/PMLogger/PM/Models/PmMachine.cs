using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMLogger.PM.Models
{
    public class PmMachine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public int AreaId { get; set; }

        public int CompanyId { get; set; }

        // Navigation properties
        [ForeignKey("AreaId")]
        public virtual PmArea PmArea { get; set; } = null!;

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        public virtual ICollection<PmChecklistItem> PmChecklistItems { get; set; } = new List<PmChecklistItem>();
        public virtual ICollection<PmMachineRunningHour> PmMachineRunningHours { get; set; } = new List<PmMachineRunningHour>();
        public virtual ICollection<PmMachineSchedule> PmMachineSchedules { get; set; } = new List<PmMachineSchedule>();
    }
}

