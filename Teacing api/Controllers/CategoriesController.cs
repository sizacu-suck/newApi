using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teacing_api.Models;
using Teacing_api.Validation_Category;

namespace Teacing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(int id)
        {
            var findCategory = await _db.Category.AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id);


            if (findCategory == null)
            {
                return NotFound();
            }


            return Ok(findCategory);
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var findAll = await _db.Category.AsNoTracking().ToListAsync();

            if (!findAll.Any())
            {
                return NotFound(new { Message = "Категории не найдены" });
            }

            return Ok(findAll);

        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateCategoryDto updateCategoryDto)
        {
            var categoryFind = await _db.Category.FindAsync(updateCategoryDto.Id);

            // 2. Если такой категории нет — возвращаем 404
            if (categoryFind == null)
            {
                return NotFound(new { Message = "Категория не найдена" });
            }

            categoryFind.Name = updateCategoryDto.Name;


            await _db.SaveChangesAsync();
            return Ok(categoryFind);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCategoryDto createCategoryDto)
        {
            var category = new Category { Name = createCategoryDto.Name }; ;



            _db.Category.Add(category);
            await _db.SaveChangesAsync();
            return Ok(category);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete (int id)
        {
            var findItem = await _db.Category.FirstOrDefaultAsync(x => x.Id == id);
            if (findItem == null)
            {
                return NotFound(new { Message = "Категория для удаления не найдена" });
            }

            _db.Category.Remove(findItem);

            await _db.SaveChangesAsync();

            return NoContent();
        }

    }
}
