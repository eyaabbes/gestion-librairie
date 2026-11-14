
using LibraryApi.DTOs;
using LibraryApi.Models;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employees
    [Authorize(Policy = "Employees.View")]
    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Ok(employees.Select(ToResponse));
    }

    // GET: api/employees/1
    [Authorize(Policy = "Employees.View")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        if (employee == null)
            return NotFound("Employee not found.");

        return Ok(ToResponse(employee));
    }

    // POST: api/employees
    [Authorize(Policy = "Employees.Create")]
    [HttpPost]
    public async Task<IActionResult> AddEmployee(Employee employee)
    {
        try
        {
            await _employeeService.AddEmployeeAsync(employee);

            return CreatedAtAction(
                nameof(GetEmployeeById),
                new { id = employee.Id },
                ToResponse(employee)
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/employees/1
    [Authorize(Policy = "Employees.Edit")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEmployee(
        int id, Employee updatedEmployee)
    {
        try
        {
            var result = await _employeeService.UpdateEmployeeAsync(
                id, updatedEmployee);

            if (!result)
                return NotFound("Employee not found.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/employees/1
    [Authorize(Policy = "Employees.Delete")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);

            if (!result)
                return NotFound("Employee not found.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST: api/employees/1/account
    [Authorize(Policy = "Employees.Create")]
    [HttpPost("{employeeId:int}/account")]
    public async Task<IActionResult> CreateEmployeeAccount(
        int employeeId,
        CreateEmployeeAccountDto dto)
    {
        try
        {
            var result = await _employeeService.CreateEmployeeAccountAsync(
                employeeId, dto);

            if (!result)
                return BadRequest("Impossible de créer le compte.");

            return Ok(new
            {
                message = "Compte employé créé avec succès."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private static object ToResponse(Employee employee) => new
    {
        employee.Id,
        employee.FullName,
        employee.Email,
        employee.JobTitle,
        employee.IsActive,
        employee.ApplicationUserId
    };
}
