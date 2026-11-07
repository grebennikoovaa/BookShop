using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Application.Services;
using BookShop.Orders.Presentation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();

var app = builder.Build();

app.MapControllers();

app.Run();