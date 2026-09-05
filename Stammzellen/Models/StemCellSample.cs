using System;
using System.ComponentModel.DataAnnotations;

namespace Stammzellen.Models
{
    public class StemCellSample
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie den Zelltyp an.")]
        [Display(Name = "Zelltyp")]
        public string CellType { get; set; } = string.Empty; // z.B. iPSC, MSC

        [Required(ErrorMessage = "Die Chargennummer ist erforderlich.")]
        [Display(Name = "Chargennummer")]
        public string BatchNumber { get; set; } = string.Empty;

        [Display(Name = "Vitalität (%)")]
        [Range(0, 100, ErrorMessage = "Die Vitalität muss zwischen 0 and 100 liegen.")]
        public double ViabilityPercentage { get; set; }

        [Display(Name = "Menge (Mio. Zellen)")]
        public double CellCountMillions { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Lagerungsdatum")]
        public DateTime StorageDate { get; set; } = DateTime.Now;

        [Display(Name = "Labor-Standort")]
        public string StorageLocation { get; set; } = string.Empty;

        [Display(Name = "Anmerkungen")]
        public string? Notes { get; set; }
    }
}
