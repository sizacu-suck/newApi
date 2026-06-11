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

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _db.Products
    .Include(p => p.category)
    .AsNoTracking()
    .Select(p => new ProductResponseDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        CategoryId = p.CategoryId,
        CategoryName = p.category != null ? p.category.Name : "Без категории"
    })
    .ToListAsync();

        if (!products.Any())
            return NotFound(new { Message = "Товары не найдены" });

        return Ok(products);


    }

    // POST: api/products
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto productVal)
    {
        var product = new Product
        {
            Price = productVal.Price,
            Name = productVal.Name,
            CategoryId = productVal.CategoryId
        };


        var ChrckCategory = await _db.Category.FirstOrDefaultAsync(x => x.Id == productVal.CategoryId);

        if (ChrckCategory == null)
        {
            return BadRequest(new { Message = $"Категории с Id = {productVal.CategoryId} не существует!" });
        }



        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return Ok(product);
    }
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        var products = await _db.Products
            .Where(p => p.Name.Contains(name))
            .AsNoTracking()
            .Include(p => p.category)
            .ToListAsync();

        return Ok(products);
    }
    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice)
    {
        var query = _db.Products.AsQueryable();

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        var products = await query
            .AsNoTracking()
            .ToListAsync();
        return Ok(products);
    }

    [HttpGet("top")]
    public async Task<IActionResult> Top([FromQuery] int count = 5)
    {
        var products = await _db.Products
            .AsNoTracking()
            .Take(count)
            .ToListAsync();

        return Ok(products);
    }

    [HttpPut()]
    public async Task<IActionResult> Put([FromBody] UpdateProductDto productVal)
    {
        var itemfind = await _db.Products.FindAsync(productVal.Id);

        if (itemfind == null)
        {
            return NotFound();
        }

        itemfind.Name = productVal.Name;
        itemfind.Price = productVal.Price;
        itemfind.CategoryId = productVal.CategoryId;

        await _db.SaveChangesAsync();


        return Ok();
    }


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
        .Include(p => p.category)
        .FirstOrDefaultAsync(p => p.Id == id);

        if (findItem == null)
        {
            return NotFound();
        }
        ProductResponseDto res = new ProductResponseDto
        {
            Id = findItem.Id,
            Name = findItem.Name,
            Price = findItem.Price,
            CategoryId = findItem.CategoryId,
            CategoryName = findItem.category != null ? findItem.category.Name : "Без категории"
        };

        

        return Ok(res);

    }

}