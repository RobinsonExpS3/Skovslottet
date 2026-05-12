using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Infrastructure.Services;
using Slottet.Shared;

namespace Slottet.Infrastructure.Test {
    [TestClass]
    public class PhoneDTOServiceTests {
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
        /// Creates a valid Phone entity for use in tests.
        /// Ensures the phone is assigned to the shared Department and contains valid default data.
        /// </summary>
        private static Phone ValidPhone() => new Phone {
            PhoneID = Guid.NewGuid(),
            PhoneNumber = "12345678",
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
        /// Ensures GetAllPhonesAsync retrieves all phones from the database.
        /// Verifies that both seeded phones are returned and identifiable by their IDs.
        /// </summary>
        [TestMethod]
        public async Task GetAllPhonesAsync_ReturnsAllPhones() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var phone1 = ValidPhone();

            var phone2 = new Phone {
                PhoneID = Guid.NewGuid(),
                PhoneNumber = "23456789",
                DepartmentID = DepartmentId
            };

            context.Phones.AddRange(phone1, phone2);
            await context.SaveChangesAsync();

            var service = new PhoneDTOService(context);

            var result = await service.GetAllPhonesAsync();

            var items = result.ToList();

            Assert.HasCount(2, items);
            Assert.IsTrue(items.Any(p => p.PhoneID == phone1.PhoneID));
            Assert.IsTrue(items.Any(p => p.PhoneID == phone2.PhoneID));
        }

        /// <summary>
        /// Ensures GetAllPhonesAsync returns phones ordered by phone number.
        /// Verifies that the returned phone collection is sorted in ascending order.
        /// </summary>
        [TestMethod]
        public async Task GetAllPhonesAsync_ReturnsPhonesOrderedByPhoneNumber() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            context.Phones.AddRange(
                new Phone {
                    PhoneID = Guid.NewGuid(),
                    PhoneNumber = "33333333",
                    DepartmentID = DepartmentId
                },
                new Phone {
                    PhoneID = Guid.NewGuid(),
                    PhoneNumber = "11111111",
                    DepartmentID = DepartmentId
                },
                new Phone {
                    PhoneID = Guid.NewGuid(),
                    PhoneNumber = "22222222",
                    DepartmentID = DepartmentId
                }
            );

            await context.SaveChangesAsync();

            var service = new PhoneDTOService(context);

            var result = await service.GetAllPhonesAsync();

            var items = result.ToList();

            // Service returns phone numbers ordered
            Assert.AreEqual("11111111", items[0].PhoneNumber);
            Assert.AreEqual("22222222", items[1].PhoneNumber);
            Assert.AreEqual("33333333", items[2].PhoneNumber);
        }

        /// <summary>
        /// Ensures GetAllPhonesAsync returns phones ordered by phone number.
        /// Verifies that the returned phone collection is sorted in ascending order.
        /// </summary>
        [TestMethod]
        public async Task GetPhoneByIdAsync_ReturnsMappedPhone_WhenFound() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var phone = ValidPhone();
            context.Phones.Add(phone);

            await context.SaveChangesAsync();

            var service = new PhoneDTOService(context);

            var result = await service.GetPhoneByIdAsync(phone.PhoneID);

            Assert.IsNotNull(result);
            Assert.AreEqual(phone.PhoneID, result.PhoneID);
            Assert.AreEqual("12345678", result.PhoneNumber);
            Assert.AreEqual(DepartmentId, result.DepartmentID);
        }

        /// <summary>
        /// Ensures GetPhoneByIdAsync returns null when no phone matches the given ID.
        /// Verifies that querying with a random ID that does not exist in the database returns null.
        /// </summary>
        [TestMethod]
        public async Task GetPhonesByIdAsync_ReturnsNull_WhenMissing() {
            using var context = CreateContext();

            var service = new PhoneDTOService(context);

            var result = await service.GetPhoneByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        /// <summary>
        /// Ensures PostPhoneAsync successfully creates a new phone in the database.
        /// Verifies that all PhoneDTO fields are persisted correctly and the phone count increases.
        /// </summary>
        [TestMethod]
        public async Task PostPhoneAsync_CreatesPhone() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());
            await context.SaveChangesAsync();

            var service = new PhoneDTOService(context);

            var phoneId = Guid.NewGuid();

            var dto = new PhoneDTO {
                PhoneID = phoneId,
                PhoneNumber = "45678912",
                DepartmentID = DepartmentId
            };

            var result = await service.PostPhoneAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(phoneId, result.PhoneID);
            Assert.AreEqual("45678912", result.PhoneNumber);
            Assert.AreEqual(DepartmentId, result.DepartmentID);
            Assert.AreEqual(1, context.Phones.Count());
        }

        /// <summary>
        /// Ensures PostPhoneAsync assigns a new PhoneID when Guid.Empty is provided.
        /// Verifies that the phone is created with a non-empty generated ID and persisted to the database.
        /// </summary>
        [TestMethod]
        public async Task PostPhoneAsync_GeneratesId_WhenPhoneIdIsEmpty() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());
            await context.SaveChangesAsync();

            var service = new PhoneDTOService(context);

            var dto = new PhoneDTO {
                PhoneID= Guid.Empty,
                PhoneNumber = "67891234",
                DepartmentID = DepartmentId
            };

            var result = await service.PostPhoneAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.PhoneID);
            Assert.AreEqual("67891234", result.PhoneNumber);
            Assert.AreEqual(1, context.Phones.Count());
        }

        /// <summary>
        /// Ensures PutPhoneAsync successfully updates all fields of an existing phone.
        /// Verifies that the changes are persisted to the database and the method returns true.
        /// </summary>
        [TestMethod]
        public async Task PutPhoneAsync_ReturnsTrue_AndUpdatesPhone_WhenFound() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var phone = ValidPhone();
            context.Phones.Add(phone);

            await context.SaveChangesAsync();

            var service = new PhoneDTOService(context);

            var dto = new PhoneDTO {
                PhoneID = phone.PhoneID,
                PhoneNumber = "99999999",
                DepartmentID = DepartmentId
            };

            var result = await service.PutPhoneAsync(phone.PhoneID, dto);

            var updatedPhone = await context.Phones
                .FirstAsync(p => p.PhoneID == phone.PhoneID);

            Assert.IsTrue(result);
            Assert.AreEqual("99999999", updatedPhone.PhoneNumber);
            Assert.AreEqual(DepartmentId, updatedPhone.DepartmentID);
        }

        /// <summary>
        /// Ensures PutPhoneAsync returns false when no phone matches the given ID.
        /// Verifies that no changes are made to the database when the phone does not exist.
        /// </summary>
        [TestMethod]
        public async Task PutPhoneAsync_ReturnsFalse_WhenPhoneMissing() {
            using var context = CreateContext();

            var service = new PhoneDTOService(context);

            var dto = new PhoneDTO {
                PhoneID = Guid.NewGuid(),
                PhoneNumber = "99999999",
                DepartmentID = DepartmentId
            };

            var result = await service.PutPhoneAsync(Guid.NewGuid(), dto);

            Assert.IsFalse(result);
            Assert.AreEqual(0, context.Phones.Count());
        }

        /// <summary>
        /// Ensures DeletePhoneAsync successfully removes an existing phone from the database.
        /// Verifies that the method returns true and the phone is no longer present in the database.
        /// </summary>
        [TestMethod]
        public async Task DeletePhoneAsync_ReturnsTrue_AndDeletesPhone_WhenFound() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var phone = ValidPhone();
            context.Phones.Add(phone);

            await context.SaveChangesAsync();

            var service = new PhoneDTOService(context);

            var result = await service.DeletePhoneAsync(phone.PhoneID);

            Assert.IsTrue(result);
            Assert.IsFalse(context.Phones.Any(p => p.PhoneID == phone.PhoneID));
        }

        /// <summary>
        /// Ensures DeletePhoneAsync returns false when no phone matches the given ID.
        /// Verifies that the existing phone remains in the database when an unknown ID is provided.
        /// </summary>
        [TestMethod]
        public async Task DeletePhoneAsync_ReturnsFalse_WhenPhoneMissing() {
            using var context = CreateContext();

            context.Departments.Add(ValidDepartment());

            var phone = ValidPhone();
            context.Phones.Add(phone);

            await context.SaveChangesAsync();

            var service = new PhoneDTOService(context);

            var result = await service.DeletePhoneAsync(Guid.NewGuid());

            Assert.IsFalse(result);
            Assert.AreEqual(1, context.Phones.Count());
        }
    }
}
