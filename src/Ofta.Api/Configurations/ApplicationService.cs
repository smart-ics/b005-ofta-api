using System.Reflection;
using FluentValidation;
using MassTransit;
using MediatR;
using Ofta.Application;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.Helpers.DocNumberGenerator;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;
using Scrutor;

namespace Ofta.Api.Configurations;

public static class ApplicationService
{
    private const string APPLICATION_ASSEMBLY = "Ofta.Application";

    public static IServiceCollection AddApplication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddMediatR(typeof(ApplicationAssemblyAnchor))
            .AddValidatorsFromAssembly(Assembly.Load(APPLICATION_ASSEMBLY))
            .AddScoped<INunaCounterBL, NunaCounterBL>()
            .AddScoped<DateTimeProvider, DateTimeProvider>()
            .AddScoped<IDocNumberGenerator, DocNumberGenerator>();

        services
            .Scan(selector => selector
                .FromAssemblyOf<ApplicationAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(INunaWriter<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<ApplicationAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(INunaWriterWithReturn<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<ApplicationAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(INunaBuilder<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<ApplicationAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IUnitOfWorkSave<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<ApplicationAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IReffIdFinderAction)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<ApplicationAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IFactoryPattern<,,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
            );
        
        services
            .AddMassTransit(x =>
            {
                x.AddConsumers(typeof(ApplicationAssemblyAnchor).Assembly);
                x.SetKebabCaseEndpointNameFormatter();

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

        return services;
    }
}