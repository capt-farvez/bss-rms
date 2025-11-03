namespace BssRms.Application.DTOs.Employee;

public class CreateEmployeeDto
{
    // Employee fields
    public string Designation { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }

    // User fields
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public string SpouseName { get; set; } = string.Empty;
    public DateTime Dob { get; set; }
    public string Nid { get; set; } = string.Empty;
    public int GenderId { get; set; }
    public string Image { get; set; } = string.Empty;
    public string Base64 { get; set; } = string.Empty;
}
