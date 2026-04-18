namespace EmployeePairs.Api.DTOs;
public class CsvRowErrorDto
{
    public int RowNumber { get; set; }

    public string RowContent { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
}
