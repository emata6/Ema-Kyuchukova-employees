using EmployeePairs.Models.DTOs;

namespace EmployeePairs.Api.DTOs
{
    public class EmployeePairAnalysisResponseDto
    {
        public List<EmployeePairProjectResultDto> CommonProjects { get; set; } = new List<EmployeePairProjectResultDto>();

        public int? TopEmployeeId1 { get; set; }

        public int? TopEmployeeId2 { get; set; }

        public int TotalDaysWorkedTogether { get; set; }

        public List<CsvRowErrorDto> Errors { get; set; } = new List<CsvRowErrorDto>();
    }
}