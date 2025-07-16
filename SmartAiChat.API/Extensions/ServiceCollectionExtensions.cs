using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using SmartAiChat.Application.Mappings;
using SmartAiChat.Application.Validators;
using SmartAiChat.Domain.Interfaces;
using SmartAiChat.Infrastructure.Repositories;
using SmartAiChat.Infrastructure.Services;
using SmartAiChat.Infrastructure.Services.SentimentAnalysis;
using SmartAiChat.Infrastructure.Services.Translation;
using SmartAiChat.Persistence;
using SmartAiChat.Shared.Constants;
using System.Reflection;
using System.Text;

namespace SmartAiChat.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile));

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(StartChatSessionValidator).Assembly));

        // Add FluentValidation
        services.AddValidatorsFromAssembly(typeof(StartChatSessionValidator).Assembly);

        // Add custom services
        services.AddScoped<ITenantContext, TenantContextService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddHttpClient("OpenAI");
        services.AddSingleton<AiServiceFactory>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<SentimentAnalysisService>();
        services.AddSingleton<TranslationService>();
        services.AddSingleton<IFileProcessingService, FileProcessingService>();
        services.AddScoped<IAiService, OpenAiService>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!123456789";
        var key = Encoding.ASCII.GetBytes(jwtKey);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            // Super Admin only policy
            options.AddPolicy(SystemConstants.Policies.SuperAdminOnly, policy =>
                policy.RequireClaim(SystemConstants.ClaimTypes.Role, SystemConstants.Roles.SuperAdmin));

            // Tenant Admin or above policy
            options.AddPolicy(SystemConstants.Policies.TenantAdminOrAbove, policy =>
                policy.RequireClaim(SystemConstants.ClaimTypes.Role,
                    SystemConstants.Roles.SuperAdmin,
                    SystemConstants.Roles.TenantAdmin));

            // Operator or above policy
            options.AddPolicy(SystemConstants.Policies.OperatorOrAbove, policy =>
                policy.RequireClaim(SystemConstants.ClaimTypes.Role,
                    SystemConstants.Roles.SuperAdmin,
                    SystemConstants.Roles.TenantAdmin,
                    SystemConstants.Roles.Operator));

            // Customer or above policy (all authenticated users)
            options.AddPolicy(SystemConstants.Policies.CustomerOrAbove, policy =>
                policy.RequireClaim(SystemConstants.ClaimTypes.Role,
                    SystemConstants.Roles.SuperAdmin,
                    SystemConstants.Roles.TenantAdmin,
                    SystemConstants.Roles.Operator,
                    SystemConstants.Roles.Customer));
        });

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SmartAiChat API",
                Version = "v1",
                Description = "Multi-tenant AI-powered live chat platform"
            });

         

            // More robust custom operation ID generation
            c.CustomOperationIds(apiDesc =>
            {
                try
                {
                    // Get route template and clean it up
                    var routeTemplate = apiDesc.RelativePath ?? "unknown";
                    var cleanRoute = routeTemplate.Replace("/", "_")
                                                 .Replace("{", "")
                                                 .Replace("}", "")
                                                 .Replace("-", "_")
                                                 .Replace(":", "_")
                                                 .Trim('_');

                    // Get HTTP method
                    var httpMethod = apiDesc.HttpMethod?.ToLower() ?? "unknown";

                    // Try to get a meaningful name from metadata
                    var endpointName = apiDesc.ActionDescriptor.EndpointMetadata?
                        .OfType<Microsoft.AspNetCore.Routing.EndpointNameMetadata>()
                        .FirstOrDefault()?.EndpointName;

                    if (!string.IsNullOrEmpty(endpointName))
                    {
                        return $"{httpMethod}_{endpointName}";
                    }

                    // Create a unique but predictable operation ID
                    var operationId = $"{httpMethod}_{cleanRoute}";
                    
                    // If still empty or just underscore, use a fallback
                    if (string.IsNullOrEmpty(operationId) || operationId == "_" || operationId == "unknown_")
                    {
                        operationId = $"{httpMethod}_operation_{Math.Abs(apiDesc.GetHashCode())}";
                    }

                    return operationId;
                }
                catch (Exception)
                {
                    // Fallback in case of any error
                    return $"{apiDesc.HttpMethod?.ToLower() ?? "unknown"}_{Math.Abs(apiDesc.GetHashCode())}";
                }
            });

            // Custom schema IDs to prevent type conflicts
            c.CustomSchemaIds(type => 
            {
                // Handle nested types
                if (type.DeclaringType != null)
                {
                    return $"{type.DeclaringType.Name}_{type.Name}";
                }
                
                // Handle generic types
                if (type.IsGenericType)
                {
                    var genericTypeName = type.Name.Split('`')[0];
                    var genericArgs = string.Join("_", type.GetGenericArguments().Select(x => x.Name));
                    return $"{genericTypeName}_{genericArgs}";
                }

                return type.Name;
            });

            // Include XML comments if available
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Add JWT security definition
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });

            // Additional configuration for minimal APIs
            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                // Only include each unique combination once
                return apiDesc.RelativePath != null;
            });

            // Configure for better Carter integration
            c.SupportNonNullableReferenceTypes();
        });

        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                if (allowedOrigins != null && allowedOrigins.Length > 0)
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                }
                else
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Default fixed window limiter
            options.AddFixedWindowLimiter("default", opt =>
            {
                opt.PermitLimit = 100;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 10;
            });

            // Chat-specific limiter
            options.AddFixedWindowLimiter("ChatPolicy", opt =>
            {
                opt.PermitLimit = 50;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 5;
            });

            // API-specific limiter
            options.AddFixedWindowLimiter("ApiPolicy", opt =>
            {
                opt.PermitLimit = 200;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 20;
            });

            options.RejectionStatusCode = 429;
        });

        return services;
    }

    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("database", tags: new[] { "db", "sql" })
            .AddCheck("openai-api", () =>
            {
                // Basic health check for OpenAI service availability
                return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("OpenAI service is configured");
            }, tags: new[] { "external", "ai" })
            .AddCheck("application", () =>
            {
                // Basic application health check
                return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Application is running");
            }, tags: new[] { "application" });

        return services;
    }
}
