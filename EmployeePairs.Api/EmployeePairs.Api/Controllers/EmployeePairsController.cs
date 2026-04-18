using EmployeePairs.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePairs.Api.Controllers
{
    [ApiController]
    [Route("api/employee-pairs")]
    public class EmployeePairsController : ControllerBase
    {
        private readonly IEmployeePairService _employeePairService;

        public EmployeePairsController(IEmployeePairService employeePairService)
        {
            _employeePairService = employeePairService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCsv([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Please upload a valid CSV file." });
                }

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "Only CSV files are allowed." });
                }

                var result = await _employeePairService.ProcessCsvAsync(file);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}