using Microsoft.AspNetCore.Identity;

namespace IsThereAnyoneWho.Core.Models
{
    public enum PersonType
    {
        Consumer,
        Provider
    }

    public class Person : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public PersonType PersonType { get; set; }
        public string ProfileDescription { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Provision> Provisions { get; set; }
    }
}
