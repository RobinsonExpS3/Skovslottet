﻿using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Data.Seed
    {
        public static class StaffSeeder
        {
            public static async Task<List<Staff>> SeedAsync(SlottetDBContext context)
            {
                if (await context.Staffs.AnyAsync())
                {
                    return await context.Staffs.ToListAsync();
                }

                var departments = await context.Departments.ToListAsync();
                if (departments.Count == 0)
                {
                    throw new InvalidOperationException("Departments must be seeded before seeding staff.");
                }

                var skovenDepartmentId = departments[0].DepartmentID;
                var slottetDepartmentId = departments.Count > 1
                    ? departments[1].DepartmentID
                    : skovenDepartmentId;

                var staffs = new List<Staff>
            {
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Hestemand Hestesen", DepartmentID = skovenDepartmentId, Initials = "HH", Role = "Pædagog" },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Søren Skovfis", DepartmentID = skovenDepartmentId, Initials = "SS", Role = "Pædagog" },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Lise Lægemand", DepartmentID = slottetDepartmentId, Initials = "LL", Role = "Sygeplejerske" },
                new Staff { StaffID = Guid.NewGuid(), StaffName = "Peter Pedalskid", DepartmentID = slottetDepartmentId, Initials = "PP", Role = "Pædagog" },
            };

                context.Staffs.AddRange(staffs);
                await context.SaveChangesAsync();

                return staffs;
            }
        }
    }


