
using LibraryApi.Models;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: api/categories
    [Authorize(Policy = "Categories.View")]
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    // GET: api/categories/1
    [Authorize(Policy = "Categories.View")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null)
            return NotFound("Category not found.");

        return Ok(category);
    }

    // POST: api/categories
    [Authorize(Policy = "Categories.Create")]
    [HttpPost]
    public async Task<IActionResult> AddCategory(Category category)
    {
        await _categoryService.AddCategoryAsync(category);

        return CreatedAtAction(
            nameof(GetCategoryById),
            new { id = category.Id },
            category
        );
    }

    // PUT: api/categories/1
    [Authorize(Policy = "Categories.Edit")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(
        int id, Category updatedCategory)
    {
        var result = await _categoryService.UpdateCategoryAsync(
            id, updatedCategory);

        if (!result)
            return NotFound("Category not found.");

        return NoContent();
    }

    // DELETE: api/categories/1
    [Authorize(Policy = "Categories.Delete")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result)
                return NotFound("Category not found.");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
