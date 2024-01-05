using Ofta.Api;
using Ofta.Api.Configurations;
using Ofta.Api.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true);

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var startup = new Startup();
startup.ConfigureServices(builder.Services);
var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("corsapp");

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
