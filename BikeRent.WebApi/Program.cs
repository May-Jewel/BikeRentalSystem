using BikeRent.Database.AppDbContextModels;
using BikeRent.Domain.Features.Admin;
using BikeRent.Domain.Features.Bike;
using BikeRent.Domain.Features.Rental;
using BikeRent.Domain.Features.User;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DbConnection")
    ?? "Server=LAPTOP-OI6UJSEI;Database=BikeRent;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
// 2. Register Domain Services for Dependency Injection (DI)
builder.Services.AddScoped<BikeService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RentalService>();
builder.Services.AddScoped<AdminService>();

// 3. Add Controllers & Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();