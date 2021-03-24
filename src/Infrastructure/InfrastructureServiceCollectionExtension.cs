using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Infrastructure.Persistence;
using Yaroshinski.Blog.Infrastructure.Services;

namespace Yaroshinski.Blog.Infrastructure
{
    public static class InfrastructureServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IEmailService, EmailService>();
            
            return services;
        }
    }
}