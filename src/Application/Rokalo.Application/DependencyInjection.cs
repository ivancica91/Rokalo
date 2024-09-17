﻿namespace Rokalo.Application
{
    using FluentValidation;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Rokalo.Application.Internal.Behaviors;
    using System.Reflection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddApplicationConfiguration(Assembly.GetExecutingAssembly());

            return services;
        }
        public static IServiceCollection AddApplicationConfiguration(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            return services;
        }
    }
}
