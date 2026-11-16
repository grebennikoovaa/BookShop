
using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Application.Services;
using BookShop.Orders.Infrastructure;
using BookShop.Orders.Infrastructure.Persistence.Seeders;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Application service
builder.Services.AddScoped<IOrderService, OrderService>();

// Infrastructure: DbContext and repositories
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Seed the database only in Development
if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();

    var seeder = scope.ServiceProvider
        .GetRequiredService<IOrderDataSeeder>();

    await seeder.SeedAsync();
}

app.MapControllers();

app.Run();
