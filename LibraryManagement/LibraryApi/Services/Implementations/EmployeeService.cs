
using LibraryApi.DTOs;
using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LibraryApi.Services.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        UserManager<ApplicationUser> userManager)
    {
        _employeeRepository = employeeRepository;
        _userManager = userManager;
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        return await _employeeRepository.GetAllAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _employeeRepository.GetByIdAsync(id);
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        var existingEmployee =
            await _employeeRepository.GetByEmailAsync(employee.Email);

        if (existingEmployee != null)
            throw new InvalidOperationException(
                "Cette adresse email existe déjà."
            );

        employee.ApplicationUserId = null;

        await _employeeRepository.AddAsync(employee);
    }

    public async Task<bool> UpdateEmployeeAsync(
        int id, Employee updatedEmployee)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
            return false;

        var existingEmployee =
            await _employeeRepository.GetByEmailAsync(updatedEmployee.Email);

        if (existingEmployee != null && existingEmployee.Id != id)
            throw new InvalidOperationException(
                "Cette adresse email est déjà utilisée."
            );

        employee.FullName = updatedEmployee.FullName;
        employee.Email = updatedEmployee.Email;
        employee.JobTitle = updatedEmployee.JobTitle;
        employee.IsActive = updatedEmployee.IsActive;

        await _employeeRepository.UpdateAsync(employee);

        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null)
            return false;

        if (!string.IsNullOrEmpty(employee.ApplicationUserId))
            throw new InvalidOperationException(
                "Cet employé possède un compte. Désactivez-le plutôt que de supprimer son profil."
            );

        await _employeeRepository.DeleteAsync(employee);

        return true;
    }

    public async Task<bool> CreateEmployeeAccountAsync(
        int employeeId,
        CreateEmployeeAccountDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);

        if (employee == null)
            throw new KeyNotFoundException("Employé introuvable.");

        if (!employee.IsActive)
            throw new InvalidOperationException("Cet employé est inactif.");

        if (!string.IsNullOrEmpty(employee.ApplicationUserId))
            throw new InvalidOperationException(
                "Cet employé possède déjà un compte."
            );

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
            throw new InvalidOperationException(
                "Cette adresse email est déjà utilisée."
            );

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = employee.FullName,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(
            user, dto.Password);

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(" | ", createResult.Errors.Select(e => e.Description))
            );
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user, "Employee");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            throw new InvalidOperationException(
                string.Join(" | ", roleResult.Errors.Select(e => e.Description))
            );
        }

        employee.ApplicationUserId = user.Id;

        try
        {
            await _employeeRepository.UpdateAsync(employee);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        return true;
    }
}
