namespace BssRms.Application.DTOs.Employee;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string Designation { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public decimal? AmountSold { get; set; }
    public UserInfoDto User { get; set; } = null!;
}
