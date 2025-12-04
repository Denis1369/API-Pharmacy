using API_Pharmacy.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger; // <--- Добавьте эту строку
using Swashbuckle.AspNetCore.SwaggerUI; // <--- И эту

internal class Program
{
    public static PharmacyDbContext _context { get; } = new PharmacyDbContext();

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Добавляем конфигурацию из appsettings.json
        builder.Configuration.AddJsonFile("appsettings.json");

        // Добавляем сервисы контроллеров
        builder.Services.AddControllers();

        // Добавляем Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });

        var app = builder.Build();

        // Настраиваем конвейер запросов
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pharmacy API v1");
            });
        }

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}