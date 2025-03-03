using System;
using System.ComponentModel.DataAnnotations;
using IsThereAnyoneWho.Core.Models;

namespace IsThereAnyoneWho.API.Models
{
    public class ProvisionViewModel
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal FinalPrice { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }

    public class CreateProvisionModel
    {
        [Required]
        public int ServiceId { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }

    public class UpdateProvisionStatusModel
    {
        [Required]
        public ProvisionStatus Status { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }
}
