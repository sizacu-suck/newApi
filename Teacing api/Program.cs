using Teacing_api.Models;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();


builder.Services.AddFluentValidation(); 
builder.Services.AddValidatorsFromAssemblyContaining<ProductDtoValidator>(); 

var app = builder.Build();

app.MapControllers();

app.Run();