using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using ServiceContracts;
using Services;

namespace Vessels.Extensions
{
    public static class ServiceExtensions
    {
        // Configure CORS
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }

        // Configure IIS Deployment
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        // Configure SQL Context
        public static void ConfigureSQLContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:SqlConnection"];

            services.AddDbContext<ShipWatchDataContext>(options => options.UseSqlServer(connectionString, SqlTypeOption => SqlTypeOption.UseNetTopologySuite()));
        }

        // Configure Services
        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
        }

        // Configure Repository Wrapper
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }

    }
}
