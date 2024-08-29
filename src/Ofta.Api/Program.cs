using MassTransit;
using Microsoft.OpenApi.Models;
using Ofta.Api.Configurations;
using Ofta.Api.Middlewares;
using Ofta.Application;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true);

var assembly = Assembly.GetEntryAssembly();
var version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OFTA Api",
        Version = $"v{version}",
        Description = "OFTAApi",
        Contact = new OpenApiContact
        {
            Name = "Intersolusi Cipta Softindo",
            Email = "support@smart-ics.com",
            Url = new Uri("https://smart-ics.com"),
        },
    });
});

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration)
    .AddMassTransit(x =>
{
    x.AddConsumers(typeof(ApplicationAssemblyAnchor).Assembly);
    x.SetKebabCaseEndpointNameFormatter();

    var configuration = builder.Configuration;

    var rabbitMqOption = configuration.GetSection("RabbitMqOption");
    var server = rabbitMqOption["Server"];
    var userName = rabbitMqOption["UserName"];
    var password = rabbitMqOption["Password"];


    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(server, "/", h =>
        {
            h.Username(userName ?? string.Empty);
            h.Password(password ?? string.Empty);
        }); ;
        cfg.ConfigureEndpoints(context);

    });
});

builder.Host
    .UseSerilog(SerilogConfiguration.ContextConfiguration);

var app = builder.Build();

app
    .UseSwagger()
    .UseSwaggerUI()
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization()
    .UseCors("corsapp")
    .UseMiddleware<ErrorHandlerMiddleware>()
    .UseMiddleware<SpaceTrimmerMiddleware>()
    .UseSerilogRequestLogging(SerilogConfiguration.SerilogRequestLoggingOption);

app.MapControllers();

app.Run();
