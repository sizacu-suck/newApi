using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teacing_api.Models;

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

        //try
        //{
        //    var products = await _db.Products.ToListAsync();

        //    return Ok();
        //}
        //catch
        //{
        //    return NotFound();
        //}

        var products = await _db.Products.ToListAsync();

        if (products == null || !products.Any())
        {
            return NotFound(new { Message = "Товары не найдены" });
        }

        return Ok(products);


    }

    // POST: api/products
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto productVal)
    {
        var product = new Product
        {
            Price = productVal.Price,
            Name = productVal.Name
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return Ok(product);
    }
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        var products = await _db.Products
            .Where(p => p.Name.Contains(name))
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

        var products = await query.ToListAsync();
        return Ok(products);
    }

    [HttpGet("top")]
    public async Task<IActionResult> Top([FromQuery] int count = 5)
    {
        var products = await _db.Products
            .Take(count)
            .ToListAsync();

        return Ok(products);
    }

    [HttpPut()]
    public async Task<IActionResult> Put( [FromBody] UpdateProductDto productVal)
    {
        var itemfind = await _db.Products.FindAsync(productVal.Id);

        if (itemfind == null)
        {
            return NotFound();
        }

        var product = new Product
        {
            Name = productVal.Name,
            Price = productVal.Price
        };


        itemfind.Name = product.Name;
        itemfind.Price = product.Price;

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
        // найти продукт по id
        // если нет → NotFound()

        _db.Products.Remove(itemfind);

        await _db.SaveChangesAsync();

        return NoContent();  // 204 No Content — удалено, тело пустое
        // удалить → _db.Products.Remove(...)
        // сохранить
        // вернуть Ok() или NoContent()
    }

    [HttpGet ("{id}")]
    public async Task<IActionResult>GetByID(int id)
    {
        var findItem = await _db.Products.FindAsync(id);

        if (findItem == null)
        {
            return NotFound();
        }    

        return Ok(findItem);

    }

}