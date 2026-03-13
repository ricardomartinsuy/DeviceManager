using DevicesApi.Application.Interfaces;
using DevicesApi.Application.Services;
using DevicesApi.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure: EF Core + PostgreSQL + Repository
builder.Services.AddInfrastructure(builder.Configuration);

// Application services
builder.Services.AddScoped<IDeviceService, DeviceService>();

// Controllers + OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Expose for integration tests
public partial class Program { }
