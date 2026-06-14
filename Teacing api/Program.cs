using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
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


//добавление системы токенов для авторизации и аутенфикации
builder.Services.AddAuthentication(options =>
{
    // Говорим системе, что по умолчанию мы ищем JWT-токен (Bearer)
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // Проверять, не истек ли срок годности токена
        ValidateIssuerSigningKey = true, // Проверять подпись ключом

        ClockSkew = TimeSpan.Zero, // Отключает 5-минутную задержку .NET

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // Разрешает абсолютно любой origin, даже file:///
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Если используешь куки/токены
    });
});


var app = builder.Build();
app.UseCors("AllowAll");
app.UseMiddleware<Teacing_api.Middleware.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//Начинаем использование токенов
app.UseAuthentication(); // КТО ты такой? (Проверяем токен)
app.UseAuthorization();  // ЧТО тебе можно? (Проверяем роли)


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