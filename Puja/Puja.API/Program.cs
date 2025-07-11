using DotNetEnv;
using DotNetEnv.Configuration;
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
using Puja.Infraestructura.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno desde .env
builder.Configuration.AddDotNetEnv();

// SignalR y notificaciones
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificacionPujaService, NotificacionPujaService>();

// Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de MongoDB
builder.Services.AddSingleton<MongoWriteDbConfig>();
builder.Services.AddSingleton<MongoReadDbConfig>();

// --- Configuración de HttpClient para microservicios ---

// UsuarioService
var usuarioServiceUrl = builder.Configuration["USUARIO_SERVICE_URL"];
if (string.IsNullOrEmpty(usuarioServiceUrl))
    throw new InvalidOperationException("La URL del servicio de usuario (USUARIO_SERVICE_URL) no está configurada.");
builder.Services.AddHttpClient<IUsuarioService, UsuarioService>(client =>
{
    client.BaseAddress = new Uri(usuarioServiceUrl);
});

// SubastaService
var subastaServiceUrl = builder.Configuration["SUBASTA_SERVICE_URL"];
if (string.IsNullOrEmpty(subastaServiceUrl))
    throw new InvalidOperationException("La URL del servicio de subasta (SUBASTA_SERVICE_URL) no está configurada.");
builder.Services.AddHttpClient<ISubastaService, SubastaService>(client =>
{
    client.BaseAddress = new Uri(subastaServiceUrl);
});

// --- Fin configuración HttpClient ---

// Repositorios
builder.Services.AddScoped<IPujaRepository, MongoWritePujaRepository>();
builder.Services.AddScoped<IPujaReadRepository, MongoReadPujaRepository>();
builder.Services.AddScoped<IPujaAutomaticaRepository, MongoWritePujaAutomaticaRepository>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreatePujaCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PujaCreatedEventHandler).Assembly));

builder.Services.AddScoped<IPujaService, PujaService>();

// MassTransit y RabbitMQ
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<ConsumerPuja>();

    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(builder.Configuration["RABBIT_URL"]), h =>
        {
            h.Username(builder.Configuration["RABBIT_USERNAME"]);
            h.Password(builder.Configuration["RABBIT_PASSWORD"]);
        });

        configurator.ReceiveEndpoint(builder.Configuration["RABBIT_QUEUE"], e =>
        {
            e.ConfigureConsumer<ConsumerPuja>(context);
        });

        configurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        configurator.ConfigureEndpoints(context);
    });
});

EndpointConvention.Map<PujaCreatedEvent>(new Uri("queue:" + builder.Configuration["RABBIT_QUEUE"]));

// CORS para React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// Redirección a Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// Pipeline de la app
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

app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();
app.MapHub<SubastaHub>("/subastaHub");

app.Run();