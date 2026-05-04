using Microsoft.AspNetCore.Identity;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid? StaffID { get; set; }
        public Staff? Staff { get; set; }
    }
}
