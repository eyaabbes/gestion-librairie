
using LibraryApi.Models;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services.Interfaces;

namespace LibraryApi.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task AddCategoryAsync(Category category)
    {
        category.Books = new List<Book>();
        await _categoryRepository.AddAsync(category);
    }

    public async Task<bool> UpdateCategoryAsync(
        int id, Category updatedCategory)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return false;

        category.Name = updatedCategory.Name;

        await _categoryRepository.UpdateAsync(category);

        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return false;

        if (await _categoryRepository.HasBooksAsync(id))
            throw new InvalidOperationException(
                "Impossible de supprimer cette catégorie : elle contient des livres."
            );

        await _categoryRepository.DeleteAsync(category);

        return true;
    }
}
