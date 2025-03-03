namespace IsThereAnyoneWho.Core.Models
{
    public enum ProvisionStatus
    {
        Pending,
        Accepted,
        InProgress,
        Completed,
        Cancelled
    }

    public class Provision
    {
        public int Id { get; set; }
        public string PersonId { get; set; }
        public int ServiceId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal FinalPrice { get; set; }
        public ProvisionStatus Status { get; set; }
        public string Notes { get; set; }

        // Navigation properties
        public virtual Person Person { get; set; }
        public virtual Service Service { get; set; }
    }
}
