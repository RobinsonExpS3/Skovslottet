using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Infrastructure.Services;
using Slottet.Shared;

namespace Slottet.Infrastructure.Test
{
    [TestClass]
    public class ResidentDTOServiceTests
    {
        private static readonly Guid GroceryDayId = Guid.NewGuid();

        private static GroceryDay ValidGroceryDay() => new GroceryDay
        {
            GroceryDayID = GroceryDayId,
            GroceryDayName = "Mandag"
        };

        private static Resident ValidResident() => new Resident
        {
            ResidentID = Guid.NewGuid(),
            ResidentName = "Anna Andersen",
            IsActive = true,
            GroceryDayID = GroceryDayId
        };

        private SlottetDBContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SlottetDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SlottetDBContext(options);
        }

        // ---------------------------------------------------------------------
        // GetAllResidentsAsync
        // ---------------------------------------------------------------------

        [TestMethod]
        public async Task GetAllResidentsAsync_ReturnsAllActiveResidents()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());

            var resident1 = ValidResident();
            var resident2 = new Resident
            {
                ResidentID = Guid.NewGuid(),
                ResidentName = "Bo Berg",
                IsActive = true,
                GroceryDayID = GroceryDayId
            };

            context.Residents.AddRange(resident1, resident2);
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);
            var result = await service.GetAllResidentsAsync();
            var items = result.ToList();

            Assert.HasCount(2, items);
            Assert.IsTrue(items.Any(r => r.ResidentID == resident1.ResidentID));
            Assert.IsTrue(items.Any(r => r.ResidentID == resident2.ResidentID));
        }

        [TestMethod]
        public async Task GetAllResidentsAsync_DoesNotReturn_InactiveResidents()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());

            var activeResident = ValidResident();
            var inactiveResident = new Resident
            {
                ResidentID = Guid.NewGuid(),
                ResidentName = "Inactive Person",
                IsActive = false,
                GroceryDayID = GroceryDayId
            };

            context.Residents.AddRange(activeResident, inactiveResident);
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);
            var result = await service.GetAllResidentsAsync();
            var items = result.ToList();

            Assert.HasCount(1, items);
            Assert.IsTrue(items.All(r => r.IsActive));
        }

        // ---------------------------------------------------------------------
        // GetResidentByIdAsync
        // ---------------------------------------------------------------------

        [TestMethod]
        public async Task GetResidentByIdAsync_ReturnsMappedResident_WhenFound()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());

            var resident = ValidResident();
            context.Residents.Add(resident);
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);
            var result = await service.GetResidentByIdAsync(resident.ResidentID);

            Assert.IsNotNull(result);
            Assert.AreEqual(resident.ResidentID, result.ResidentID);
            Assert.AreEqual("Anna Andersen", result.ResidentName);
            Assert.AreEqual(GroceryDayId, result.GroceryDayID);
            Assert.IsTrue(result.IsActive);
        }

        [TestMethod]
        public async Task GetResidentByIdAsync_ReturnsNull_WhenMissing()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());

            var resident = ValidResident();
            context.Residents.Add(resident);
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);
            var result = await service.GetResidentByIdAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        // ---------------------------------------------------------------------
        // PostResidentAsync
        // ---------------------------------------------------------------------

        [TestMethod]
        public async Task PostResidentAsync_CreatesResident_WithNewID_WhenIDIsEmpty()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);

            var dto = new EditResidentDto
            {
                ResidentID = Guid.Empty,
                ResidentName = "Bo Berg",
                IsActive = true,
                GroceryDayID = GroceryDayId
            };

            var result = await service.PostResidentAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(Guid.Empty, result.ResidentID);
            Assert.AreEqual("Bo Berg", result.ResidentName);
            Assert.AreEqual(1, context.Residents.Count());
        }

        // ---------------------------------------------------------------------
        // PutResidentAsync
        // ---------------------------------------------------------------------
        [TestMethod]
        public async Task PutResidentAsync_ReturnsTrue_AndUpdatesResident_WhenFound()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());

            var resident = ValidResident();
            context.Residents.Add(resident);
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);

            var dto = new EditResidentDto
            {
                ResidentID = resident.ResidentID,
                ResidentName = "Updated Name",
                IsActive = false,
                GroceryDayID = GroceryDayId
            };

            var result = await service.PutResidentAsync(resident.ResidentID, dto);

            var updatedResident = await context.Residents
                .FirstAsync(r => r.ResidentID == resident.ResidentID);

            Assert.IsTrue(result);
            Assert.AreEqual("Updated Name", updatedResident.ResidentName);
            Assert.IsFalse(updatedResident.IsActive);
            Assert.AreEqual(GroceryDayId, updatedResident.GroceryDayID);
        }

        [TestMethod]
        public async Task PutResidentAsync_ReturnsFalse_WhenResidentMissing()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);

            var dto = new EditResidentDto
            {
                ResidentID = Guid.NewGuid(),
                ResidentName = "Missing Name",
                IsActive = true,
                GroceryDayID = GroceryDayId
            };

            var result = await service.PutResidentAsync(Guid.NewGuid(), dto);

            Assert.IsFalse(result);
            Assert.AreEqual(0, context.Residents.Count());
        }

        // ---------------------------------------------------------------------
        // DeleteResidentAsync
        // ---------------------------------------------------------------------

        [TestMethod]
        public async Task DeleteResidentAsync_ReturnsTrue_AndDeletesResident_WhenFound()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());

            var resident = ValidResident();
            context.Residents.Add(resident);
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);
            var result = await service.DeleteResidentAsync(resident.ResidentID);

            Assert.IsTrue(result);
            Assert.IsFalse(context.Residents.Any(r => r.ResidentID == resident.ResidentID));
        }

        [TestMethod]
        public async Task DeleteResidentAsync_ReturnsFalse_WhenResidentMissing()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());

            var resident = ValidResident();
            context.Residents.Add(resident);
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);
            var result = await service.DeleteResidentAsync(Guid.NewGuid());

            Assert.IsFalse(result);
            Assert.AreEqual(1, context.Residents.Count());
        }
    }
}