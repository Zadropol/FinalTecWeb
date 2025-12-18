using FluentValidation;
using HotelManager.Core.Interfaces;
using HotelManager.Core.Services;
using HotelManager.Infrastructure.Data;
using HotelManager.Infrastructure.Filters;
using HotelManager.Infrastructure.Mappings;
using HotelManager.Infrastructure.Repositories;
using HotelManager.Infrastructure.Services;
using HotelManager.Infrastructure.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MySql.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);
// ⭐ Registrar Unit of Work (reemplaza los repositorios individuales)
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.AddTransient<IReservaService, ReservaService>();
builder.Services.AddTransient<ICheckInOutService, CheckInOutService>();
builder.Services.AddTransient<IReporteService, ReporteService>();
// ⭐ Registrar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ⭐ Registrar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ReservaDtoValidator>();
builder.Services.AddScoped<IValidationService, ValidationService>();


builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<IReservaQueryService, ReservaQueryService>();


// Configurar la conexión a MySQL
var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
builder.Services.AddDbContext<HotelContext>(options =>
    options.UseMySQL(connectionString));

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); // 👈 Agregar filtro de excepciones
    options.Filters.Add<ValidationFilter>();
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Management API",
        Version = "v1",
        Description = "API RESTful para la gestión integral de un hotel",
        Contact = new OpenApiContact
        {
            Name = "Adrian Rodriguez Montecinos",
            Email = "adrian.rodriguez@hotel.com"
        }
    });

    // Habilitar comentarios XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Management API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
