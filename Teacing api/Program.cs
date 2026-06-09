using Teacing_api.Models;
using Microsoft.EntityFrameworkCore;  // ← добавить эту строку

var builder = WebApplication.CreateBuilder(args);  // ← CreateBuilder, а не CreateHostBuilder

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("hgx8")));

var app = builder.Build();

app.MapControllers();

app.Run();