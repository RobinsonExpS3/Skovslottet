using Microsoft.AspNetCore.Identity;
using Slottet.Domain.Entities;

namespace Slottet.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid? StaffID { get; set; }
        public Staff? Staff { get; set; }
    }
}
