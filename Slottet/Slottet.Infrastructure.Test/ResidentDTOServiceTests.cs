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

        /// <summary>
        /// Creates a valid GroceryDay entity for use in tests.
        /// Ensures all required GroceryDay fields are populated with consistent test data.
        /// </summary>
        private static GroceryDay ValidGroceryDay() => new GroceryDay
        {
            GroceryDayID = GroceryDayId,
            GroceryDayName = "Mandag"
        };

        /// <summary>
        /// Creates a valid active Resident entity for use in tests.
        /// Ensures the resident is assigned to the shared GroceryDay and contains valid default data.
        /// </summary>
        private static Resident ValidResident() => new Resident
        {
            ResidentID = Guid.NewGuid(),
            ResidentName = "Anna Andersen",
            IsActive = true,
            GroceryDayID = GroceryDayId
        };

        /// <summary>
        /// Creates a new in-memory SlottetDBContext instance for isolated unit testing.
        /// Ensures each test uses a unique database instance to prevent shared state between tests.
        /// </summary>
        private SlottetDBContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<SlottetDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SlottetDBContext(options);
        }

        /// <summary>
        /// Ensures GetAllResidentsAsync retrieves all active residents from the database.
        /// Verifies that both seeded active residents are returned and identifiable by their IDs.
        /// </summary>
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

        /// <summary>
        /// Ensures GetAllResidentsAsync filters out inactive residents and only returns active ones.
        /// Verifies that when both active and inactive residents exist, only the active resident is returned.
        /// </summary>
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

        //// <summary>
        /// Ensures GetResidentByIdAsync correctly maps all Resident entity fields to the EditResidentDTO.
        /// Verifies that ResidentID, ResidentName, GroceryDayID, and IsActive are all mapped accurately.
        /// </summary>
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

        /// <summary>
        /// Ensures GetResidentByIdAsync returns null when no resident matches the given ID.
        /// Verifies that querying with a random ID that does not exist in the database returns null.
        /// </summary>
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

        /// <summary>
        /// Ensures PostResidentAsync assigns a new ResidentID when Guid.Empty is provided.
        /// Verifies that the resident is created with a non-empty generated ID and persisted to the database.
        /// </summary>
        [TestMethod]
        public async Task PostResidentAsync_CreatesResident_WithNewID_WhenIDIsEmpty()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);

            var dto = new EditResidentDTO
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

        /// <summary>
        /// Ensures PutResidentAsync successfully updates all fields of an existing resident.
        /// Verifies that the changes are persisted to the database and the method returns true.
        /// </summary>
        [TestMethod]
        public async Task PutResidentAsync_ReturnsTrue_AndUpdatesResident_WhenFound()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());

            var resident = ValidResident();
            context.Residents.Add(resident);
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);

            var dto = new EditResidentDTO
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

        /// <summary>
        /// Ensures PutResidentAsync returns false when no resident matches the given ID.
        /// Verifies that no changes are made to the database when the resident does not exist.
        /// </summary>
        [TestMethod]
        public async Task PutResidentAsync_ReturnsFalse_WhenResidentMissing()
        {
            using var context = CreateContext();
            context.GroceryDays.Add(ValidGroceryDay());
            await context.SaveChangesAsync();

            var service = new ResidentDTOService(context);

            var dto = new EditResidentDTO
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

        /// <summary>
        /// Ensures DeleteResidentAsync successfully removes an existing resident from the database.
        /// Verifies that the method returns true and the resident is no longer present in the database.
        /// </summary>
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
        /// <summary>
        /// Ensures DeleteResidentAsync returns false when no resident matches the given ID.
        /// Verifies that the existing resident remains in the database when an unknown ID is provided.
        /// </summary>
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