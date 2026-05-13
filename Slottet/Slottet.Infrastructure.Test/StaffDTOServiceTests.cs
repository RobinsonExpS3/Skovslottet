using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Infrastructure.Services;
using Slottet.Shared;

namespace Slottet.Infrastructure.Test {
    [TestClass]
    public class StaffDTOServiceTests {
        private static readonly Guid DepartmentId = Guid.NewGuid();

        private static Department ValidDepartment() => new Department {
            DepartmentID = DepartmentId,
            DepartmentName = "Slottet"
        };

        private static Staff ValidStaff() => new Staff {
            StaffID = Guid.NewGuid(),
            StaffName = "Anna Andersen",
            Initials = "AA",
            Role = "Pædagog",
            DepartmentID = DepartmentId
        };

        private SlottetDBContext CreateContext() {
            var options = new DbContextOptionsBuilder<SlottetDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SlottetDBContext(options);
        }

        [TestMethod]
        public async Task GetAllStaffAsync_ReturnsAllStaff() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var staff1 = ValidStaff();

            var staff2 = new Staff {
                StaffID = Guid.NewGuid(),
                StaffName = "Bo Berg",
                Initials = "BB",
                Role = "Pædagog",
                DepartmentID = DepartmentId
            };

            context.Staffs.AddRange(staff1, staff2);
            await context.SaveChangesAsync();

            var service = new StaffDTOService(context);

            var result = await service.GetAllStaffAsync();

            var items = result.ToList();

            Assert.HasCount(2, items);
            Assert.IsTrue(items.Any(s => s.StaffID == staff1.StaffID));
            Assert.IsTrue(items.Any(s => s.StaffID == staff2.StaffID));
        }

        [TestMethod]
        public async Task GetStaffByIdAsync_ReturnsMappedStaff_WhenFound() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var staff = ValidStaff();
            context.Staffs.Add(staff);

            await context.SaveChangesAsync();

            var service = new StaffDTOService(context);

            var result = await service.GetStaffByIdAsync(staff.StaffID);

            Assert.IsNotNull(result);
            Assert.AreEqual(staff.StaffID, result.StaffID);
            Assert.AreEqual("Anna Andersen", result.StaffName);
            Assert.AreEqual("AA", result.Initials);
            Assert.AreEqual("Pædagog", result.Role);
            Assert.AreEqual(DepartmentId, result.DepartmentID);
        }

        [TestMethod]
        public async Task GetStaffByIdAsync_ReturnsNull_WhenMissing() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var staff = ValidStaff();
            context.Staffs.Add(staff);

            await context.SaveChangesAsync();

            var service = new StaffDTOService(context);

            var result = await service.GetStaffByIdAsync(Guid.NewGuid());
        }

        [TestMethod]
        public async Task PostStaffAsync_CreatesStaff_WhenDepartmentExists() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());
            await context.SaveChangesAsync();

            var service = new StaffDTOService(context);

            var dto = new EditStaffDTO {
                StaffID = Guid.Empty,
                StaffName = "Bo Berg",
                Initials = "BB",
                Role = "Pædagog",
                DepartmentID = DepartmentId
            };

            var result = await service.PostStaffAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.StaffID);
            Assert.AreEqual("Bo Berg", result.StaffName);
            Assert.AreEqual(1, context.Staffs.Count());
        }

        [TestMethod]
        public async Task PostStaffAsync_ThrowsArgumentException_WhenDepartmentDoesNotExist() {
            using var context = CreateContext();

            var service = new StaffDTOService(context);

            var dto = new EditStaffDTO {
                StaffID = Guid.NewGuid(),
                StaffName = "Bo Berg",
                Initials = "BB",
                Role = "Pædagog",
                DepartmentID = Guid.NewGuid()
            };

            try {
                await service.PostStaffAsync(dto);

                Assert.Fail("Expected argument exception was not thrown, test fails");
            } catch(ArgumentException) {
                // Test passes since argument excetion is thrown
            }
        }

        [TestMethod]
        public async Task PutStaffAsync_ReturnsTrue_AndUpdatesStaff_WhenFound() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var staff = ValidStaff();
            context.Staffs.Add(staff);

            await context.SaveChangesAsync();

            var service = new StaffDTOService(context);

            var dto = new EditStaffDTO {
                StaffID = staff.StaffID,
                StaffName = "Updated Name",
                Initials = "UN",
                Role = "Admin",
                DepartmentID = DepartmentId
            };

            var result = await service.PutStaffAsync(staff.StaffID, dto);

            var updatedStaff = await context.Staffs
                .FirstAsync(s => s.StaffID == staff.StaffID);

            Assert.IsTrue(result);
            Assert.AreEqual("Updated Name", updatedStaff.StaffName);
            Assert.AreEqual("UN", updatedStaff.Initials);
            Assert.AreEqual("Admin", updatedStaff.Role);
            Assert.AreEqual(DepartmentId, updatedStaff.DepartmentID);
        }

        [TestMethod]
        public async Task PutStaffAsync_ReturnsFalse_WhenStaffMissing() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());
            await context.SaveChangesAsync();

            var service = new StaffDTOService(context);

            var dto = new EditStaffDTO {
                StaffID = Guid.NewGuid(),
                StaffName = "Missing Name",
                Initials = "MN",
                Role = "Pædagog",
                DepartmentID = DepartmentId
            };

            var result = await service.PutStaffAsync(Guid.NewGuid(), dto);

            Assert.IsFalse(result);
            Assert.AreEqual(0, context.Staffs.Count());
        }

        [TestMethod]
        public async Task DeleteStaffAsync_ReturnsTrue_AndDeletesStaff_WhenFound() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var staff = ValidStaff();
            context.Staffs.Add(staff);

            await context.SaveChangesAsync();

            var service = new StaffDTOService(context);

            var result = await service.DeleteStaffAsync(staff.StaffID);

            Assert.IsTrue(result);
            Assert.IsFalse(context.Staffs.Any(s => s.StaffID == staff.StaffID));
        }

        [TestMethod]
        public async Task DeleteStaffAsync_ReturnsFalse_WhenStaffMissing() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var staff = ValidStaff();
            context.Staffs.Add(staff);

            await context.SaveChangesAsync();

            var service = new StaffDTOService(context);

            var result = await service.DeleteStaffAsync(Guid.NewGuid());

            Assert.IsFalse(result);
            Assert.AreEqual(1, context.Staffs.Count());
        }
    }
}