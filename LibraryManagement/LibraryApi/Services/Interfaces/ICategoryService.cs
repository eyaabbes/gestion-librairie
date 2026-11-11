
using LibraryApi.Models;

namespace LibraryApi.Services.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task AddCategoryAsync(Category category);
    Task<bool> UpdateCategoryAsync(int id, Category updatedCategory);
    Task<bool> DeleteCategoryAsync(int id);
}
