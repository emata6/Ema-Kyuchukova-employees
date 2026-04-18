using System.Globalization;
using EmployeePairs.Api.DTOs;
using EmployeePairs.Api.Services.Interfaces;
using EmployeePairs.Models.DTOs;
using EmployeePairs.Models;

namespace EmployeePairs.Api.Services.Implementations
{
    public class EmployeePairService : IEmployeePairService
    {
        private static readonly string[] SupportedDateFormats =
        {
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            "yyyyMMdd",
            "dd/MM/yyyy",
            "dd-MM-yyyy",
            "MM/dd/yyyy",
            "MM-dd-yyyy"
        };

        public async Task<EmployeePairAnalysisResponseDto> ProcessCsvAsync(IFormFile file)
        {
            List<CsvRowErrorDto> errors = new List<CsvRowErrorDto>();

            List<EmployeeProjectRecord> records = await ReadCsvAsync(file, errors);

            Dictionary<(int EmployeeId1, int EmployeeId2), int> pairTotals =
                new Dictionary<(int EmployeeId1, int EmployeeId2), int>();

            List<EmployeePairProjectResultDto> results = CalculateCommonProjects(records, pairTotals);

            var topPair = pairTotals
                .OrderByDescending(x => x.Value)
                .FirstOrDefault();

            EmployeePairAnalysisResponseDto response = new EmployeePairAnalysisResponseDto
            {
                CommonProjects = results
                    .OrderByDescending(x => x.DaysWorked)
                    .ToList(),
                TopEmployeeId1 = pairTotals.Count > 0 ? topPair.Key.EmployeeId1 : null,
                TopEmployeeId2 = pairTotals.Count > 0 ? topPair.Key.EmployeeId2 : null,
                TotalDaysWorkedTogether = pairTotals.Count > 0 ? topPair.Value : 0,
                Errors = errors
            };

            return response;
        }

        private async Task<List<EmployeeProjectRecord>> ReadCsvAsync(
            IFormFile file,
            List<CsvRowErrorDto> errors)
        {
            List<EmployeeProjectRecord> records = new List<EmployeeProjectRecord>();

            using StreamReader reader = new StreamReader(file.OpenReadStream());

            string? line;
            bool isFirstLine = true;
            int rowNumber = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                rowNumber++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (isFirstLine)
                {
                    isFirstLine = false;

                    if (line.Contains("EmpID", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                try
                {
                    string[] parts = line.Split(',');

                    if (parts.Length != 4)
                    {
                        throw new ArgumentException("Invalid CSV row format.");
                    }

                    EmployeeProjectRecord record = new EmployeeProjectRecord
                    {
                        EmpId = ParseInt(parts[0], "EmpID", rowNumber, line),
                        ProjectId = ParseInt(parts[1], "ProjectID", rowNumber, line),
                        DateFrom = ParseDate(parts[2], "DateFrom", rowNumber, line),
                        DateTo = ParseDateTo(parts[3], rowNumber, line)
                    };

                    if (record.DateFrom > record.DateTo)
                    {
                        throw new ArgumentException("DateFrom cannot be after DateTo.");
                    }

                    records.Add(record);
                }
                catch (ArgumentException ex)
                {
                    errors.Add(new CsvRowErrorDto
                    {
                        RowNumber = rowNumber,
                        RowContent = line,
                        ErrorMessage = ex.Message
                    });
                }
            }

            return records;
        }

        private List<EmployeePairProjectResultDto> CalculateCommonProjects(
            List<EmployeeProjectRecord> records,
            Dictionary<(int EmployeeId1, int EmployeeId2), int> pairTotals)
        {
            List<EmployeePairProjectResultDto> results = new List<EmployeePairProjectResultDto>();

            var groupedByProject = records.GroupBy(x => x.ProjectId);

            foreach (var projectGroup in groupedByProject)
            {
                List<EmployeeProjectRecord> employees = projectGroup
                    .OrderBy(x => x.DateFrom)
                    .ToList();

                for (int i = 0; i < employees.Count; i++)
                {
                    EmployeeProjectRecord firstEmployee = employees[i];

                    for (int j = i + 1; j < employees.Count; j++)
                    {
                        EmployeeProjectRecord secondEmployee = employees[j];

                        if (secondEmployee.DateFrom > firstEmployee.DateTo)
                        {
                            break;
                        }

                        DateTime overlapStart = firstEmployee.DateFrom > secondEmployee.DateFrom
                            ? firstEmployee.DateFrom
                            : secondEmployee.DateFrom;

                        DateTime overlapEnd = firstEmployee.DateTo < secondEmployee.DateTo
                            ? firstEmployee.DateTo
                            : secondEmployee.DateTo;

                        if (overlapStart <= overlapEnd)
                        {
                            int daysWorked = (overlapEnd - overlapStart).Days + 1;

                            int employeeId1 = Math.Min(firstEmployee.EmpId, secondEmployee.EmpId);
                            int employeeId2 = Math.Max(firstEmployee.EmpId, secondEmployee.EmpId);

                            EmployeePairProjectResultDto result = new EmployeePairProjectResultDto
                            {
                                EmployeeId1 = employeeId1,
                                EmployeeId2 = employeeId2,
                                ProjectId = projectGroup.Key,
                                DaysWorked = daysWorked
                            };

                            results.Add(result);

                            var pairKey = (employeeId1, employeeId2);

                            if (!pairTotals.ContainsKey(pairKey))
                            {
                                pairTotals[pairKey] = 0;
                            }

                            pairTotals[pairKey] += daysWorked;
                        }
                    }
                }
            }

            return results;
        }

        private int ParseInt(string input, string fieldName, int rowNumber, string line)
        {
            if (!int.TryParse(input.Trim(), out int parsedValue))
            {
                throw new ArgumentException(
                    $"Invalid {fieldName} value '{input}' at row {rowNumber}: {line}"
                );
            }

            return parsedValue;
        }

        private DateTime ParseDate(string input, string fieldName, int rowNumber, string line)
        {
            string trimmedInput = input.Trim();

            List<DateTime> parsedDates = new List<DateTime>();

            foreach (string format in SupportedDateFormats)
            {
                if (DateTime.TryParseExact(
                        trimmedInput,
                        format,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime parsedDate))
                {
                    if (!parsedDates.Contains(parsedDate))
                    {
                        parsedDates.Add(parsedDate);
                    }
                }
            }

            if (parsedDates.Count == 1)
            {
                return parsedDates[0];
            }

            if (parsedDates.Count > 1)
            {
                throw new ArgumentException(
                    $"Ambiguous {fieldName} value '{input}' at row {rowNumber}: {line}. Please use an unambiguous format like yyyy-MM-dd."
                );
            }

            throw new ArgumentException(
                $"Invalid {fieldName} date value '{input}' at row {rowNumber}: {line}"
            );
        }

        private DateTime ParseDateTo(string input, int rowNumber, string line)
        {
            string trimmedInput = input.Trim();

            if (string.IsNullOrWhiteSpace(trimmedInput) ||
                trimmedInput.Equals("NULL", StringComparison.OrdinalIgnoreCase))
            {
                return DateTime.Today;
            }

            return ParseDate(trimmedInput, "DateTo", rowNumber, line);
        }
    }
}