using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Infrastructure.Services;
using Slottet.Shared;

namespace Slottet.Infrastructure.Test {
    [TestClass]
    public class PhoneDTOServiceTests {
        private static readonly Guid DepartmentId = Guid.NewGuid();

        private static Department ValidDepartment() => new Department {
            DepartmentID = DepartmentId,
            DepartmentName = "Slottet"
        };

        private static Phone ValidPhone() => new Phone {
            PhoneID = Guid.NewGuid(),
            PhoneNumber = "12345678",
            DepartmentID = DepartmentId
        };

        private SlottetDBContext CreateContext() {
            var options = new DbContextOptionsBuilder<SlottetDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SlottetDBContext(options);
        }

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

            Assert.AreEqual("11111111", items[0].PhoneNumber);
            Assert.AreEqual("22222222", items[1].PhoneNumber);
            Assert.AreEqual("33333333", items[2].PhoneNumber);
        }

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

        [TestMethod]
        public async Task GetPhonesByIdAsync_ReturnsNull_WhenMissing() {
            using var context = CreateContext();

            var service = new PhoneDTOService(context);

            var result = await service.GetPhoneByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

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
