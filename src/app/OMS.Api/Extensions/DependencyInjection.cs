using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OMS.Api.Constants;
using OMS.Api.Identity;
using OMS.Application.Common.Abstractions;
using OMS.Domain.Constants;
using OMS.Domain.Entities;
using OMS.Infrastructure.Persistence;

namespace OMS.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGenWithAuth();
        services.AddIdentity(configuration);
        services.AddCorsHandler(configuration);
        services.AddScoped<ICurrentUser, CurrentUser>();
        
        return services;
    }

    private static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter your JWT token in this field",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };
            
            opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },[]
                }
            };
            
            opt.AddSecurityRequirement(securityRequirement);
        });
        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<OmsUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<OmsDbContext>();
        
        services.AddAuthorizationBuilder()
            .AddPolicy(OmsPolicies.StaffOnly, p 
                => p.RequireRole(UserRoles.Staff))
            .AddPolicy(OmsPolicies.AdminOnly, p 
                => p.RequireRole(UserRoles.Admin))
            .AddPolicy(OmsPolicies.DeliveryStaff, p 
                => p.RequireRole(UserRoles.DeliveryStaff))
            .AddPolicy(OmsPolicies.CustomerOnly, p 
                => p.RequireRole(UserRoles.Customer));
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        return services;
    }
    
    private static IServiceCollection AddCorsHandler(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration
            .GetSection("AllowCors").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });
        
        return services;
    }
}