
using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services.Interfaces;

namespace LibraryApi.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<List<Student>> GetAllStudentsAsync()
    {
        return await _studentRepository.GetAllAsync();
    }

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        return await _studentRepository.GetByIdAsync(id);
    }

    public async Task AddStudentAsync(Student student)
    {
        var existingStudent =
            await _studentRepository.GetByEmailAsync(student.Email);

        if (existingStudent != null)
            throw new InvalidOperationException(
                "Cette adresse email est déjà utilisée."
            );

        student.Loans = new List<Loan>();
        await _studentRepository.AddAsync(student);
    }

    public async Task<bool> UpdateStudentAsync(
        int id, Student updatedStudent)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
            return false;

        var existingStudent =
            await _studentRepository.GetByEmailAsync(updatedStudent.Email);

        if (existingStudent != null && existingStudent.Id != id)
            throw new InvalidOperationException(
                "Cette adresse email est déjà utilisée."
            );

        student.FullName = updatedStudent.FullName;
        student.Email = updatedStudent.Email;

        await _studentRepository.UpdateAsync(student);

        return true;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
            return false;

        if (await _studentRepository.HasLoansAsync(id))
            throw new InvalidOperationException(
                "Impossible de supprimer cet étudiant : il possède des emprunts."
            );

        await _studentRepository.DeleteAsync(student);

        return true;
    }
}
