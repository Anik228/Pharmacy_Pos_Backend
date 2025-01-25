using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using pharmacy_pos_system.module.medicine.model;
using pharmacy_pos_system.module.medicine.service;
using pharmacy_pos_system.module.user.model;

namespace pharmacy_pos_system.module.medicine.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicineController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }


        

        [HttpPost("add-medicine")]
        [Authorize]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Register([FromBody] AddMedicineDto registerDto)
        {
            try
            {
                

                await _medicineService.AddMedicineAsync(registerDto);
                return Ok(new { Message = "medicine added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("medicines")]
        [Authorize]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAllMedicines()
        {
            try
            {
                var Medicine = await _medicineService.GetAllMedicineAsync();
                return Ok(Medicine);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("delete-medicine/{id:int}")]
        [Authorize]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            await _medicineService.DeleteMedicineAsync(id);
            return NoContent();
        }

        [HttpPut("update-medicine/{id:int}")]
        [Authorize]
        [Authorize(Roles = "admin,user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateUser(int id, AddMedicineDto medicine)
        {
            if (medicine == null || id <= 0)
            {
                return BadRequest(new common.ApiErros
                {
                    status = 404,
                    message = "Bad Request"
                });
            }

            bool result = await _medicineService.UpdateMedicineAsync(id, medicine);

            if (!result)
            {
                return NotFound(new common.ApiErros
                {
                    status = 404,
                    message = "Medicine not found."
                });
            }

            return Ok(new common.ApiErros
            {
                status = 200,
                message = "Successfully Updated"
            });
        }

        [HttpGet("find-a-medicine/{id}")]
        [Authorize]
        [Authorize(Roles = "admin,user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public Task<Medicine> GetTask(int id) => _medicineService.GetMedicineByIdAsync(id);

    }
}
