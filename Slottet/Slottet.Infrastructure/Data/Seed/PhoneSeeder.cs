using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
{
    public static class PhoneSeeder
    {
        public static async Task<List<Phone>> SeedAsync(SlottetDBContext context)
        {
            if (await context.Phones.AnyAsync())
                return await context.Phones.ToListAsync();

            var skoven = await context.Departments.FirstOrDefaultAsync(d => d.DepartmentName == "Skoven");

            if (skoven is null)
                throw new InvalidOperationException("Department 'Skoven' was not found.");

            var phones = new List<Phone>
            {
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "00112233", DepartmentID = skoven.DepartmentID },
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "11223344", DepartmentID = skoven.DepartmentID },
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "22334455", DepartmentID = skoven.DepartmentID },
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "33445566", DepartmentID = skoven.DepartmentID },
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "44556677", DepartmentID = skoven.DepartmentID },
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "55667788", DepartmentID = skoven.DepartmentID },
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "66778899", DepartmentID = skoven.DepartmentID },
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "77889900", DepartmentID = skoven.DepartmentID },
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "88990011", DepartmentID = skoven.DepartmentID },
                new Phone { PhoneID = Guid.NewGuid(), PhoneNumber = "99001122", DepartmentID = skoven.DepartmentID }
            };
            context.Phones.AddRange(phones);
            await context.SaveChangesAsync();
            return phones;
        }
    }
}
