using Carter;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newsletter.Reporting.Api.Articles;
using Newsletter.Reporting.Api.Database;
using Newsletter.Reporting.Api.Extensions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

builder.Services.AddCarter();

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.AddConsumer<ArticleCreatedConsumer>();
    busConfigurator.AddConsumer<ArticleViewedConsumer>();

    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
        {
            h.Username(builder.Configuration["MessageBroker:Username"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        configurator.ConfigureEndpoints(context);
    });
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Newsletter.Reporting.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation(o => o.SetDbStatementForText = true)
            .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

        tracing.AddOtlpExporter();
    });

builder.Services.AddServiceDiscovery(o => o.UseConsul());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.MapCarter();

app.Run();