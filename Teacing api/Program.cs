using Teacing_api.Models;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Teacing_api.Validation;
using Teacing_api.Validation_Category;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();


builder.Services.AddFluentValidation(); 
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>(); 
builder.Services.AddValidatorsFromAssemblyContaining<UpdateProductDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateProductDtoValidator>();

var app = builder.Build();

app.MapControllers();

app.Run();