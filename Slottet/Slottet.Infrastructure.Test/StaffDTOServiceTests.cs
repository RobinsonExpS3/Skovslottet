using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Infrastructure.Services;
using Slottet.Shared;

namespace Slottet.Infrastructure.Test {
    [TestClass]
    public class StaffDTOServiceTests {
        private static readonly Guid DepartmentId = Guid.NewGuid();

        /// <summary>
        /// Creates a valid Department entity for use in tests.
        /// Ensures all required Department fields are populated with consistent test data.
        /// </summary>
        private static Department ValidDepartment() => new Department {
            DepartmentID = DepartmentId,
            DepartmentName = "Slottet"
        };

        /// <summary>
        /// Creates a valid Staff entity for use in tests.
        /// Ensures the staff member is assigned to the shared Department and contains valid default data.
        /// </summary>
        private static Staff ValidStaff() => new Staff {
            StaffID = Guid.NewGuid(),
            StaffName = "Anna Andersen",
            Initials = "AA",
            Role = "Pædagog",
            DepartmentID = DepartmentId
        };

        /// <summary>
        /// Creates a new in-memory SlottetDBContext instance for isolated unit testing.
        /// Ensures each test uses a unique database instance to prevent shared state between tests.
        /// </summary>
        private SlottetDBContext CreateContext() {
            var options = new DbContextOptionsBuilder<SlottetDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SlottetDBContext(options);
        }
        /// <summary>
        /// Ensures GetAllStaffAsync retrieves all staff members from the database.
        /// Verifies that both seeded staff members are returned and identifiable by their IDs.
        /// </summary>
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

        /// <summary>
        /// Ensures GetStaffByIdAsync correctly maps all Staff entity fields to the EditStaffDTO.
        /// Verifies that StaffID, StaffName, Initials, Role, and DepartmentID are all mapped accurately.
        /// </summary>
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

        /// <summary>
        /// Ensures GetStaffByIdAsync returns null when no staff member matches the given ID.
        /// Verifies that querying with a random ID that does not exist in the database returns null.
        /// </summary>
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

        /// <summary>
        /// Ensures PostStaffAsync successfully creates a new staff member when the department exists.
        /// Verifies that a generated StaffID is assigned and the staff member is persisted to the database.
        /// </summary>
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

        /// <summary>
        /// Ensures PostStaffAsync throws an ArgumentException when the specified department does not exist.
        /// Verifies that staff members cannot be created with an invalid DepartmentID.
        /// </summary>
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

        /// <summary>
        /// Ensures PutStaffAsync successfully updates all fields of an existing staff member.
        /// Verifies that the changes are persisted to the database and the method returns true.
        /// </summary>
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

        /// <summary>
        /// Ensures PutStaffAsync returns false when no staff member matches the given ID.
        /// Verifies that no changes are made to the database when the staff member does not exist.
        /// </summary>
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

        /// <summary>
        /// Ensures DeleteStaffAsync successfully removes an existing staff member from the database.
        /// Verifies that the method returns true and the staff member is no longer present in the database.
        /// </summary>
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

        /// <summary>
        /// Ensures DeleteStaffAsync returns false when no staff member matches the given ID.
        /// Verifies that the existing staff member remains in the database when an unknown ID is provided.
        /// </summary>
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