
using LibraryApi.Models;

namespace LibraryApi.Services.Interfaces;

public interface IStudentService
{
    Task<List<Student>> GetAllStudentsAsync();
    Task<Student?> GetStudentByIdAsync(int id);
    Task AddStudentAsync(Student student);
    Task<bool> UpdateStudentAsync(int id, Student updatedStudent);
    Task<bool> DeleteStudentAsync(int id);
}
