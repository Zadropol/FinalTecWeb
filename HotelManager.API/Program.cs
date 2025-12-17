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
using MySql.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// ⭐ Registrar Unit of Work (reemplaza los repositorios individuales)
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.AddTransient<IReservaService, ReservaService>();

// ⭐ Registrar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ⭐ Registrar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ReservaDtoValidator>();
builder.Services.AddScoped<IValidationService, ValidationService>();


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
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
