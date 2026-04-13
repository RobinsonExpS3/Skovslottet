using Microsoft.AspNetCore.Mvc;
using Slottet.Application.Interfaces;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Repositories;
using Slottet.Shared;

namespace Slottet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResidentController : Controller
    {
        private readonly IBaseRepository<Resident> _baseRepository; 
        private readonly ResidentRepository _residentRepository;

        public ResidentController( IBaseRepository<Resident> baseRepository, ResidentRepository residentRepository) {
            _baseRepository = baseRepository;
            _residentRepository = residentRepository;
        }

        //Get: residents
        [HttpGet("Residents")]
        public async Task<ActionResult<IEnumerable<ResidentViewModel>>> GetAll() {
            var residents = await _baseRepository.GetAllAsync();
            var result = residents.Select(r => new ResidentViewModel {
                ResidentID = r.ResidentID,
                ResidentName = r.ResidentName,
                GroceryDayID = r.GroceryDayID,
                GroceryDayName = r.GroceryDayName ?? string.Empty,
                IsActive = r.IsActive
            });

            return Ok(result);
        }

        //Get: resident by id
        [HttpGet("{id}")]
        public async Task<ActionResult<ResidentDTO>> GetById(Guid id) {
            var resident = await _baseRepository.GetByIdAsync(id);
            
            if (resident == null) {
                return NotFound();
            }

            var dto = new ResidentDTO {
                ResidentID = resident.ResidentID,
                ResidentName = resident.ResidentName,
                GroceryDayID = resident.GroceryDayID,
                IsActive = resident.IsActive,
                PaymentMethodIDs = await _residentRepository.GetPaymentMethodIdsAsync(id),
                MedicineTimes = await _residentRepository.GetMedicineTimesAsync(id)
            };

            return Ok(dto);
        }

        //Post: resident
        [HttpPost]
        public async Task<ActionResult<Resident>> CreateResident([FromBody] ResidentDTO dto) {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ResidentName) || dto.GroceryDayID == Guid.Empty) {
                return BadRequest();
            }

            var resident = new Resident {
                ResidentID = Guid.NewGuid(),
                ResidentName = dto.ResidentName,
                GroceryDayID = dto.GroceryDayID,
                IsActive = dto.IsActive
            };

            await _baseRepository.AddAsync(resident);
            await _residentRepository.ReplacePaymentMethodsAsync(resident.ResidentID, dto.PaymentMethodIDs);
            await _residentRepository.ReplaceMedicineTimesAsync(resident.ResidentID, dto.MedicineTimes);

            return CreatedAtAction(nameof(GetById), new { id = resident.ResidentID }, resident);
        }

        //Put: resident by id
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateResident(Guid id, [FromBody] ResidentDTO dto) {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ResidentName) || dto.GroceryDayID == Guid.Empty) {
                return BadRequest();
            }

            var existingResident = await _baseRepository.GetByIdAsync(id);
            if (existingResident == null) return NotFound();

            existingResident.ResidentName = dto.ResidentName;
            existingResident.GroceryDayID = dto.GroceryDayID;
            existingResident.IsActive = dto.IsActive;

            await _baseRepository.UpdateAsync(existingResident);
            await _residentRepository.ReplacePaymentMethodsAsync(id, dto.PaymentMethodIDs);
            await _residentRepository.ReplaceMedicineTimesAsync(id, dto.MedicineTimes);

            return NoContent();
        }

        //Delete: resident by id
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResident(Guid id) {
            var existingResident = await _baseRepository.GetByIdAsync(id);

            if (existingResident == null) {
                return NotFound();
            }

            await _residentRepository.DeleteRelationsAsync(id);
            await _baseRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
