using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMLogger.PM.Models
{
    public class PmMachineRunningHour
    {
        [Key]
        public int Id { get; set; }

        public int MachineId { get; set; }

        [Required]
        [StringLength(255)]
        public string MachineName { get; set; } = string.Empty;

        public int? AreaId { get; set; }

        [StringLength(255)]
        public string? AreaName { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentRunningHours { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal MaxRunningHours { get; set; } = 8000;

        [StringLength(20)]
        public string AlarmStatus { get; set; } = "green";

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        public string? UpdatedBy { get; set; }

        public string? Notes { get; set; }

        public int CompanyId { get; set; }

        // Navigation properties
        [ForeignKey("MachineId")]
        public virtual PmMachine? PmMachine { get; set; }

        [ForeignKey("AreaId")]
        public virtual PmArea? PmArea { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        public virtual ICollection<PmMachineRunningHourHistory> PmMachineRunningHourHistories { get; set; } = new List<PmMachineRunningHourHistory>();
    }
}

