using MassTransit;
using Ofta.Api.Configurations;
using Ofta.Api.Middlewares;
using Ofta.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true);

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
    .UseSerilogRequestLogging(SerilogConfiguration.SerilogRequestLoggingOption);

app.MapControllers();

app.Run();
