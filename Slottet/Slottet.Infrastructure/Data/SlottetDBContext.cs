using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data
{
    public class SlottetDBContext : DbContext
    {

        public SlottetDBContext(DbContextOptions<SlottetDBContext> options)
        : base(options)
        {
        }

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<DepartmentTask> DepartmentTasks => Set<DepartmentTask>();
        public DbSet<GroceryDay> GroceryDays => Set<GroceryDay>();
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<Phone> Phones => Set<Phone>();
        public DbSet<PN> PNs => Set<PN>();
        public DbSet<Resident> Residents => Set<Resident>();
        public DbSet<ResidentPaymentMethod> ResidentPaymentMethods => Set<ResidentPaymentMethod>();
        public DbSet<ResidentStatus> ResidentStatuses => Set<ResidentStatus>();
        public DbSet<RiskLevel> RiskLevels => Set<RiskLevel>();
        public DbSet<ShiftBoard> ShiftBoards => Set<ShiftBoard>();
        public DbSet<SpecialResponsibility> SpecialResponsibilities => Set<SpecialResponsibility>();
        public DbSet<Staff> Staffs => Set<Staff>();
        public DbSet<StaffResidentStatus> StaffResidentStatuses => Set<StaffResidentStatus>();
        public DbSet<StaffShift> StaffShifts => Set<StaffShift>();
        public DbSet<StaffPhone> StaffPhones => Set<StaffPhone>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SlottetDBContext).Assembly);
        }
    }
}
