using Microsoft.AspNetCore.Mvc;
using BookMyDoctor.Server.Services.Interfaces;
using BookMyDoctor.Server.DTOs;

namespace BookMyDoctor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicsController : ControllerBase
    {
        private readonly IClinicService _clinicService;

        public ClinicsController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetClinics()
        {
            var clinics = await _clinicService.GetClinicsAsync();
            return Ok(new ApiResponse<object> { Success = true, Data = clinics });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClinic(int id)
        {
            var clinic = await _clinicService.GetClinicByIdAsync(id);
            return Ok(new ApiResponse<object> { Success = true, Data = clinic });
        }

        [HttpPost]
        public async Task<IActionResult> CreateClinic([FromBody] ClinicCreateDto request)
        {
            var clinic = await _clinicService.CreateClinicAsync(request);
            return Ok(new ApiResponse<object> { Success = true, Data = clinic });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinic(int id, [FromBody] ClinicUpdateDto request)
        {
            await _clinicService.UpdateClinicAsync(id, request);
            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            await _clinicService.DeleteClinicAsync(id);
            return Ok(new ApiResponse<object> { Success = true });
        }
    }
}