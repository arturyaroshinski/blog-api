using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Yaroshinski.Blog.Api
{
    public static class ApiServiceCollectionExtension
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddCors();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Blog api", Version = "v1"
                });

                var securityDefinition = new OpenApiSecurityScheme
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                };
                
                c.AddSecurityDefinition("jwt_auth", securityDefinition);

                var securityScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference()
                    {
                        Id = "jwt_auth",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                
                var securityRequirements = new OpenApiSecurityRequirement()
                {
                    {securityScheme, new string[] { }},
                };
                
                c.AddSecurityRequirement(securityRequirements);
            });

            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
            services.AddRouting(options => options.LowercaseUrls = true);

            return services;
        }
    }
}