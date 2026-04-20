using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResidentController : Controller
    {
        private readonly SlottetDBContext _context;

        public ResidentController(SlottetDBContext context)
        {
            _context = context;
        }

        //Get: residents
        [HttpGet("Residents")]
        public async Task<ActionResult<IEnumerable<ResidentViewModel>>> GetAll()
        {
            var result = await _context.Residents
                .AsNoTracking()
                .Select(r => new ResidentViewModel
                {
                    ResidentID = r.ResidentID,
                    ResidentName = r.ResidentName,
                    GroceryDayID = r.GroceryDayID,
                    IsActive = r.IsActive,
                })
                .ToListAsync();

            return Ok(result);
        }

        //Get: resident by id
        [HttpGet("{id}")]
        public async Task<ActionResult<ResidentDTO>> GetById(Guid id)
        {
            var resident = await _context.Residents
                .Include(r => r.ResidentPaymentMethods.Where(r => r.ResidentID == id))
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ResidentID == id);

            if (resident == null)
            {
                return NotFound();
            }

            var dto = new ResidentDTO
            {
                ResidentID = resident.ResidentID,
                ResidentName = resident.ResidentName,
                GroceryDayID = resident.GroceryDayID,
                IsActive = resident.IsActive,
                PaymentMethodIDs = resident.ResidentPaymentMethods.Select(r => r.PaymentMethodID).ToList(),
                MedicineTimes = await _context.Medicines
                    .AsNoTracking()
                    .Where(m => m.ResidentID == id)
                    .Select(m => m.MedicineTime)
                    .ToListAsync()
            };

            return Ok(dto);
        }

        //Post: resident
        [HttpPost]
        public async Task<ActionResult<Resident>> CreateResident([FromBody] ResidentDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ResidentName) || dto.GroceryDayID == Guid.Empty)
            {
                return BadRequest();
            }

            var resident = new Resident
            {
                ResidentID = Guid.NewGuid(),
                ResidentName = dto.ResidentName,
                GroceryDayID = dto.GroceryDayID,
                IsActive = dto.IsActive
            };

            _context.Residents.Add(resident);

            AddPaymentMethods(resident.ResidentID, dto.PaymentMethodIDs);
            AddMedicines(resident.ResidentID, dto.MedicineTimes);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = resident.ResidentID }, resident);
        }

        //Put: resident by id
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateResident(Guid id, [FromBody] ResidentDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ResidentName) || dto.GroceryDayID == Guid.Empty)
            {
                return BadRequest();
            }

            var existingResident = await _context.Residents.FirstOrDefaultAsync(r => r.ResidentID == id);
            if (existingResident == null)
            {
                return NotFound();
            }

            existingResident.ResidentName = dto.ResidentName;
            existingResident.GroceryDayID = dto.GroceryDayID;
            existingResident.IsActive = dto.IsActive;

            var existingPaymentMethods = await _context.ResidentPaymentMethods
                .Where(rpm => rpm.ResidentID == id).ToListAsync();
            _context.ResidentPaymentMethods.RemoveRange(existingPaymentMethods);
            AddPaymentMethods(id, dto.PaymentMethodIDs);

            var existingMedicines = await _context.Medicines
                .Where(m => m.ResidentID == id).ToListAsync();
            _context.Medicines.RemoveRange(existingMedicines);
            AddMedicines(id, dto.MedicineTimes);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Delete: resident by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResident(Guid id)
        {
            var existingResident = await _context.Residents.FirstOrDefaultAsync(r => r.ResidentID == id);

            if (existingResident == null)
            {
                return NotFound();
            }

            var residentStatusIDs = await _context.ResidentStatuses
                .Where(rs => rs.ResidentID == id)
                .Select(rs => rs.ResidentStatusID)
                .ToListAsync();

            if (residentStatusIDs.Count > 0)
            {
                var staffResidentStatuses = await _context.StaffResidentStatuses
                    .Where(srs => residentStatusIDs.Contains(srs.ResidentStatusID))
                    .ToListAsync();
                _context.StaffResidentStatuses.RemoveRange(staffResidentStatuses);
            }

            var residentStatuses = await _context.ResidentStatuses
                .Where(rs => rs.ResidentID == id)
                .ToListAsync();
            _context.ResidentStatuses.RemoveRange(residentStatuses);

            var residentPaymentMethods = await _context.ResidentPaymentMethods
                .Where(rpm => rpm.ResidentID == id)
                .ToListAsync();
            _context.ResidentPaymentMethods.RemoveRange(residentPaymentMethods);

            var medicines = await _context.Medicines
                .Where(m => m.ResidentID == id)
                .ToListAsync();
            _context.Medicines.RemoveRange(medicines);

            _context.Residents.Remove(existingResident);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void AddPaymentMethods(Guid residentID, List<Guid>? paymentMethodsIDs)
        {
            if (paymentMethodsIDs == null || paymentMethodsIDs.Count == 0)
            {
                return;
            }

            var relationRows = paymentMethodsIDs
                .Distinct()
                .Select(paymentMethodID => new ResidentPaymentMethod
                {
                    ResidentID = residentID,
                    PaymentMethodID = paymentMethodID,
                });

            _context.ResidentPaymentMethods.AddRange(relationRows);
        }

        private void AddMedicines(Guid residentID, List<DateTime>? medicineTimes)
        {
            if (medicineTimes == null || medicineTimes.Count == 0)
            {
                return;
            }

            var medicineRows = medicineTimes.Select(medicineTime => new Medicine
            {
                MedicineID = Guid.NewGuid(),
                ResidentID = residentID,
                MedicineTime = medicineTime,
                MedicineGivenTime = DateTime.Now,
                MedicineRegisteredTime = DateTime.Now
            });

            _context.Medicines.AddRange(medicineRows);
        }
    }
}