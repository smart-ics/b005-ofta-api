using Ofta.Application.ParamContext.ConnectionAgg.Contracts;
using Ofta.Infrastructure;
using Ofta.Infrastructure.Helpers;
using Ofta.Infrastructure.ParamContext.ConnectionAgg;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataAccessHelper;
using Ofta.Infrastructure.ParamContext;
using Ofta.Infrastructure.UserContext.Services;
using Scrutor;

namespace Ofta.Api.Configurations;

public static class InfrastructureService
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddScoped<INunaCounterDal, ParamNoDal>();
        services.AddScoped<IDbConnectionDal, DbConnectionDal>();
        services.AddTransient<IUsmanGetTokenService, UsmanGetTokenService>();
        services.AddMemoryCache();
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SECTION_NAME));

        services
            .Scan(selector => selector
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IInsert<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IUpdate<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IDelete<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IGetData<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IListData<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IListData<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IListData<,,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(INunaService<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
                .FromAssemblyOf<InfrastructureAssemblyAnchor>()
                    .AddClasses(c => c.AssignableTo(typeof(IRequestResponseService<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime()
            );
        return services;
    }

}