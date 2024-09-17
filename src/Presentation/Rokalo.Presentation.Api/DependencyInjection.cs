namespace Rokalo.Presentation.Api
{
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Rokalo.Blocks.Common.Exceptions;
    using System;
    using System.Linq;
    using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

    public static class DependencyInjection
    {
        private const string Bearer = "Bearer";
        public static IServiceCollection AddPresentationConfiguration(this IServiceCollection services, IHostEnvironment environment)
        {
            Action<RouteOptions> routeOptions = options => options.LowercaseUrls = true;
            Action<ProblemDetailsOptions> problemDetailsOptions = options => SetProblemDetailsOptions(options, environment);
            Action<MvcNewtonsoftJsonOptions> newtonsoftOptions = options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            };

            services
                .AddRouting(routeOptions)
                .AddProblemDetails(problemDetailsOptions)
                .AddControllers()
                .AddNewtonsoftJson(newtonsoftOptions);

            services.AddSwaggerGen(c =>
            {
                //needed to include jwt token in request that needs authorization
                // TODO maybe move this config to some extension class, look at grd ServiceCollecionExtensions
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                // Add JWT Authentication
                c.AddSecurityDefinition(Bearer, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer abc123\"",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = Bearer
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        private static void SetProblemDetailsOptions(ProblemDetailsOptions options, IHostEnvironment env)
        {
            Type[] knownExceptionTypes = new Type[] { typeof(ServiceValidationException) };

            options.IncludeExceptionDetails = (_, exception) => 
                env.IsDevelopment() &&
                !knownExceptionTypes.Contains(exception.GetType());

            options.Map<ServiceValidationException>(exception =>
            new ValidationProblemDetails(exception.Errors)
            {
                Title = exception.Title,
                Detail = exception.Detail,
                Status = StatusCodes.Status400BadRequest
            });
            }
        }
    }
