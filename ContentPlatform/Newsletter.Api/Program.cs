using Carter;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newsletter.Api.Articles;
using Newsletter.Api.Database;
using Newsletter.Api.Extensions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Steeltoe.Common.Http.Discovery;
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
    .ConfigureResource(resource => resource.AddService("Newsletter.Api"))
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

builder.Services
    .AddHttpClient<GetReportingArticle.Client>(client => client.BaseAddress = new Uri("http://reporting-service"))
    .AddServiceDiscovery()
    .AddRoundRobinLoadBalancer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.MapCarter();

app.Run();
