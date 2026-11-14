
using LibraryApi.Models;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // GET: api/students
    [Authorize(Policy = "Students.View")]
    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students.Select(ToResponse));
    }

    // GET: api/students/1
    [Authorize(Policy = "Students.View")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStudentById(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);

        if (student == null)
            return NotFound("Student not found.");

        return Ok(ToResponse(student));
    }

    // POST: api/students
    [Authorize(Policy = "Students.Create")]
    [HttpPost]
    public async Task<IActionResult> AddStudent(Student student)
    {
        try
        {
            await _studentService.AddStudentAsync(student);

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = student.Id },
                ToResponse(student)
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/students/1
    [Authorize(Policy = "Students.Edit")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStudent(
        int id, Student updatedStudent)
    {
        try
        {
            var result = await _studentService.UpdateStudentAsync(
                id, updatedStudent);

            if (!result)
                return NotFound("Student not found.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/students/1
    [Authorize(Policy = "Students.Delete")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        try
        {
            var result = await _studentService.DeleteStudentAsync(id);

            if (!result)
                return NotFound("Student not found.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private static object ToResponse(Student student) => new
    {
        student.Id,
        student.FullName,
        student.Email
    };
}
