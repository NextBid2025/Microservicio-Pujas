using DotNetEnv;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Puja.Application.Handlers;
using Puja.Domain.Repositories;

using Puja.Domain.Events;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Productos.Infrastructure.Configurations;
using Pruja.Infrastructure.Configurations;

using Puja.Application.Handlers.EventHandlers;
using Puja.Application.Interfaces;
using Puja.Infraestructura.Consumer;
using Puja.Infraestructura.Interfaces;
using Puja.Infraestructura.Persistence.Repository.MongoRead;
using Puja.Infraestructura.Persistence.Repository.MongoWrite;
using Puja.Infraestructura.Services;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Configuración de controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de MongoDB
builder.Services.AddSingleton<MongoWriteDbConfig>();
builder.Services.AddSingleton<MongoReadDbConfig>();


builder.Services.AddHttpClient<ISubastaService, SubastaService>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SUBASTA_SERVICE_URL"));
});
// Repositorios
builder.Services.AddScoped<IPujaRepository, MongoWritePujaRepository>();
builder.Services.AddScoped<IPujaReadRepository, MongoReadPujaRepository>();

// MediatR para los handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreatePujaCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PujaCreatedEventHandler).Assembly));

// Configuración de MassTransit y RabbitMQ
builder.Services.AddMassTransit(busConfigurator =>
{
    
    busConfigurator.AddConsumer<ConsumerPuja>();

    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(Environment.GetEnvironmentVariable("RABBIT_URL")), h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBIT_USERNAME"));
            h.Password(Environment.GetEnvironmentVariable("RABBIT_PASSWORD"));
        });

        configurator.ReceiveEndpoint(Environment.GetEnvironmentVariable("RABBIT_QUEUE"), e => {
             e.ConfigureConsumer<ConsumerPuja>(context);
        });

        configurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        configurator.ConfigureEndpoints(context);
    });
});

 EndpointConvention.Map<PujaCreatedEvent>(new Uri("queue:" + Environment.GetEnvironmentVariable("RABBIT_QUEUE")));

 var app = builder.Build();

// Redirect root URL to Swagger
 app.MapGet("/", context =>
 {
     context.Response.Redirect("/swagger");
     return Task.CompletedTask;
 });

// Configure the HTTP request pipeline.
 if (app.Environment.IsDevelopment())
 {
     app.UseSwagger();
     app.UseSwaggerUI();
 }

 
 app.UseExceptionHandler(errorApp =>
 {
     errorApp.Run(async context =>
     {
         var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
         context.Response.StatusCode = 400;
         await context.Response.WriteAsync(exceptionHandlerPathFeature?.Error.Message ?? "Error");
     });
 });
 app.UseAuthorization();

 app.MapControllers();

 app.Run();