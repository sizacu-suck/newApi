using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teacing_api.Models;
using Teacing_api.Validation;
using Teacing_api.Validation_product;

namespace Teacing_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    //[HttpGet]
    //public async Task<IActionResult> GetAll()
    //{
    //    var products = await _db.Products
    //.Include(p => p.category)
    //.AsNoTracking()
    //.Select(p => new ProductResponseDto
    //{
    //    Id = p.Id,
    //    Name = p.Name,
    //    Price = p.Price,
    //    CategoryId = p.CategoryId,
    //    CategoryName = p.category != null ? p.category.Name : "Без категории"
    //})
    //.ToListAsync();

    //    if (!products.Any())
    //        return NotFound(new { Message = "Товары не найдены" });

    //    return Ok(products);


    //}


    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] decimal? minPrice = null,
    [FromQuery] decimal? maxPrice = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1)
            pageSize = 10;
        if (pageSize > 50)
            pageSize = 50;

        IQueryable<Product> query = _db.Products.AsNoTracking();

        if (minPrice.HasValue)
        {
            query = query.Where(x => x.Price >= minPrice);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(x => x.Price <= maxPrice);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
        }

        var productsDto = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.category != null ? p.category.Name : "Без категории"
            })
            .ToListAsync();

        if (!productsDto.Any())
        {
            return NotFound(new { Message = "Товары по заданным фильтрам не найдены" });
        }

        return Ok(productsDto);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto productVal)
    {
        bool categoryExists = await _db.Category.AnyAsync(x => x.Id == productVal.CategoryId);

        if (!categoryExists)
        {
            return BadRequest(new { Message = $"Категории с Id = {productVal.CategoryId} не существует!" });
        }
        var product = new Product
        {
            Price = productVal.Price,
            Name = productVal.Name,
            CategoryId = productVal.CategoryId
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return Ok(product);
    }
    [Authorize(Roles = "User,Admin")]
    [HttpPut()]
    public async Task<IActionResult> Put([FromBody] UpdateProductDto productVal)
    {
        var itemfind = await _db.Products.FindAsync(productVal.Id);

        if (itemfind == null)
        {
            return NotFound();
        }
        bool categoryExists = await _db.Category.AnyAsync(x => x.Id == productVal.CategoryId);

        if (!categoryExists)
        {
            return BadRequest(new { Message = $"Категории с Id = {productVal.CategoryId} не существует!" });
        }
        itemfind.Name = productVal.Name;
        itemfind.Price = productVal.Price;
        itemfind.CategoryId = productVal.CategoryId;

        await _db.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var itemfind = await _db.Products.FindAsync(id);

        if (itemfind == null)
        {
            return NotFound();
        }
        _db.Products.Remove(itemfind);

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByID(int id)
    {
        var findItem = await _db.Products
        .AsNoTracking()
        .Where(p => p.Id == id)
        .Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryId = p.CategoryId,
            CategoryName = p.category != null ? p.category.Name : "Без категории"
        })
        .FirstOrDefaultAsync();

        if (findItem == null)
        {
            return NotFound();
        }



        return Ok(findItem);
    }
}