
using LibraryApi.DTOs;
using LibraryApi.Models;

namespace LibraryApi.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllEmployeesAsync();

    Task<Employee?> GetEmployeeByIdAsync(int id);

    Task AddEmployeeAsync(Employee employee);

    Task<bool> UpdateEmployeeAsync(int id, Employee updatedEmployee);

    Task<bool> DeleteEmployeeAsync(int id);

    Task<bool> CreateEmployeeAccountAsync(
        int employeeId,
        CreateEmployeeAccountDto dto);
}
