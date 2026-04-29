using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMLogger.PM.Models
{
    public class PmMachineRunningHourHistory
    {
        [Key]
        public int Id { get; set; }

        public int MachineId { get; set; }

        [Required]
        [StringLength(255)]
        public string MachineName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? RunningHours { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MaxRunningHours { get; set; }

        public DateTime RecordedDate { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        public string? RecordedBy { get; set; }

        // Navigation property
        [ForeignKey("MachineId")]
        public virtual PmMachineRunningHour PmMachineRunningHour { get; set; } = null!;
    }
}

