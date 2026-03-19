using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Implements;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Implements;
using AgroTemp.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.ComponentModel.Design;
using Resend;
using AgroTemp.Service.Config.ApiModels;
using PayOS;

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
            services.AddScoped<IJobCategoryService, JobCategoryService>();
            services.AddScoped<IJobPostService, JobPostService>();
            services.AddScoped<IJobApplicationService, JobApplicationService>();
            services.AddScoped<IJobDetailService, JobDetailService>();
            services.AddScoped<IFarmService, FarmService>();
            services.AddScoped<IWorkerAttendanceService, WorkerAttendanceService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IExpoPushService, ExpoPushService>();
            services.AddSingleton<PayOSClient>(_ => new PayOSClient(new PayOSOptions
            {
                ClientId = configuration["PayOS:ClientId"] ?? string.Empty,
                ApiKey = configuration["PayOS:ApiKey"] ?? string.Empty,
                ChecksumKey = configuration["PayOS:ChecksumKey"] ?? string.Empty
            }));
            services.AddScoped<IPayOSService, PayOSService>();

            // Custom Services
            //services.AddScoped<ICloudinaryService, CloudinaryService>();
            
            // Email Service
            services.AddHttpClient(); // Resend uses HttpClient
            services.AddTransient<IResend, ResendClient>();
            services.AddOptions<ResendClientOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    options.ApiToken = config["Resend:ApiKey"]!;
                });
            services.AddScoped<IEmailService, ResendEmailService>();

            // Custom Services
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            // Third-Party Services
            RegisterThirdPartyServices(services, configuration);

            return services;
        }

        private static void RegisterThirdPartyServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudinarySetting>(options =>
            {
                options.CloudinaryUrl = configuration["Cloudinary:CloudinaryUrl"];
            });
            CloudinarySetting.Instance = services.BuildServiceProvider().GetService<IOptions<CloudinarySetting>>().Value;
        }
    }
}
