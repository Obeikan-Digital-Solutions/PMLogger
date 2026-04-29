using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMLogger.PM.Models
{
    public class ChecklistExecution
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string AreaName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MachineName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string ChecklistType { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Technician { get; set; } = string.Empty;

        public DateTime ExecutionDate { get; set; } = DateTime.UtcNow;

        public string? CompletedItems { get; set; } // JSON array

        public string? Notes { get; set; } // JSON object

        public bool IsSubmission { get; set; } = false;

        public int CompanyId { get; set; }

        // Navigation property
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
    }
}

