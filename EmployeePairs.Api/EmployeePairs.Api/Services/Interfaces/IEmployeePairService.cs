using EmployeePairs.Api.DTOs;

namespace EmployeePairs.Api.Services.Interfaces
{
    public interface IEmployeePairService
    {
        public Task<EmployeePairAnalysisResponseDto> ProcessCsvAsync(IFormFile file);
    }
}