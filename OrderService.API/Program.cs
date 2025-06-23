using Confluent.Kafka;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Features.Orders.Commands.CreateOrderCommand;
using OrderService.Core.Interfaces;
using OrderService.Infrastructure.Kafka;
using OrderService.Infrastructure.Option;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Services;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("OrdersDb")); // or UseSqlServer(...)

// Redis
var redisConfig = builder.Configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost";

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));

// Kafka
var kafkaServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers") ?? "localhost:9092";

builder.Services.AddSingleton<IKafkaProducer>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<KafkaProducer>>();
    var config = new ProducerConfig { BootstrapServers = kafkaServers };
    return new KafkaProducer(new ProducerBuilder<Null, string>(config).Build(), logger);
});

// HTTP Client + Polly
builder.Services.AddHttpClient<INotificationService, NotificationService>()
    .ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
});

builder.Services.Configure<NotificationServiceOptions>(
    builder.Configuration.GetSection("NotificationService")
);

// Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderHandler).Assembly));


var app = builder.Build();

// Global error handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exceptionHandlerPathFeature?.Error, "Unhandled exception");

        await context.Response.WriteAsJsonAsync(new
        {
            Message = "An unexpected error occurred."
        });
    });
});

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
