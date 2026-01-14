using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Implements;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Implements;
using AgroTemp.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.ComponentModel.Design;

namespace AgroTemp.API.Configuration
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            // Mapper
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IMapperlyMapper, MapperlyMapper>();


            // Repositories
            services.AddScoped<IUnitOfWork<AgroTempDbContext>, UnitOfWork<AgroTempDbContext>>();

            // Services
            services.AddScoped<AuthService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            // Custom Services
            //services.AddScoped<ICloudinaryService, CloudinaryService>();

            // Third-Party Services
            //RegisterThirdPartyServices(services, configuration);

            return services;
        }

        //private static void RegisterThirdPartyServices(IServiceCollection services, IConfiguration configuration)
        //{
        //    services.Configure<CloudinarySetting>(options =>
        //    {
        //        options.CloudinaryUrl = configuration["Cloudinary:CloudinaryUrl"];
        //    });
        //    CloudinarySetting.Instance = services.BuildServiceProvider().GetService<IOptions<CloudinarySetting>>().Value;
        //}
    }
}
