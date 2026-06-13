using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Teacing_api.Models;
using Teacing_api.Validation;
using Teacing_api.Validation_Category;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("--> Начинаем настройку DbContext...");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
Log.Information("--> DbContext успешно настроен!");

Log.Information("--> Начинаем настройку контроллеров...");
builder.Services.AddControllers();
Log.Information("--> контроллеры успешно настроен!");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidation(); 
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>(); 
builder.Services.AddValidatorsFromAssemblyContaining<UpdateProductDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UodateCategoryDtoValidator>();



var app = builder.Build();
app.UseMiddleware<Teacing_api.Middleware.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

try
{
    Log.Information("Приложение Teaching_api успешно запускается...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение неожиданно упало при запуске!");
}
finally
{
    Log.CloseAndFlush(); // Гарантирует, что все логи запишутся на диск до закрытия процесса
}