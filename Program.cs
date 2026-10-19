using Microsoft.EntityFrameworkCore;
using StockFlowAPI.Data;
using StockFlowAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı (SQLite)
builder.Services.AddDbContext<StockFlowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Servisler (JSON Döngü Hatasını Önlemek İçin ReferenceHandler.IgnoreCycles Eklendi)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Global Hata Yönetimi
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 4. wwwroot (Statik dosyalar ve arayüz)
app.UseStaticFiles();

app.UseAuthorization();

app.MapFallbackToFile("index.html");

app.MapControllers();

app.Run();