using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Implements;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Implements;
using AgroTemp.Service.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
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
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IExpoPushService, ExpoPushService>();
            services.AddScoped<IDashboardService, DashboardService>();

            // Setup static IP proxy for PayOS if running on Heroku
            HttpClient? payOsProxyClient = null;
            var fixieUrl = Environment.GetEnvironmentVariable("FIXIE_URL");
            if (!string.IsNullOrEmpty(fixieUrl))
            {
                var handler = new HttpClientHandler
                {
                    Proxy = new System.Net.WebProxy(fixieUrl, true),
                    UseProxy = true
                };
                payOsProxyClient = new HttpClient(handler);
            }

            // payOS client for payment link flow (deposit/top-up)
            services.AddKeyedSingleton<PayOSClient>("OrderClient", (sp, _) => new PayOSClient(new PayOSOptions
            {
                ClientId = configuration["PayOS:ClientId"]
                    ?? Environment.GetEnvironmentVariable("PAYOS_CLIENT_ID")
                    ?? string.Empty,
                ApiKey = configuration["PayOS:ApiKey"]
                    ?? Environment.GetEnvironmentVariable("PAYOS_API_KEY")
                    ?? string.Empty,
                ChecksumKey = configuration["PayOS:ChecksumKey"]
                    ?? Environment.GetEnvironmentVariable("PAYOS_CHECKSUM_KEY")
                    ?? string.Empty,
                LogLevel = LogLevel.Debug,
                HttpClient = payOsProxyClient
            }));

            // payOS client for payout flow (withdraw)
            services.AddKeyedSingleton<PayOSClient>("TransferClient", (sp, _) => new PayOSClient(new PayOSOptions
            {
                ClientId = configuration["PayOS:PayoutClientId"]
                    ?? Environment.GetEnvironmentVariable("PAYOS_PAYOUT_CLIENT_ID")
                    ?? configuration["PayOS:ClientId"]
                    ?? string.Empty,
                ApiKey = configuration["PayOS:PayoutApiKey"]
                    ?? Environment.GetEnvironmentVariable("PAYOS_PAYOUT_API_KEY")
                    ?? configuration["PayOS:ApiKey"]
                    ?? string.Empty,
                ChecksumKey = configuration["PayOS:PayoutChecksumKey"]
                    ?? Environment.GetEnvironmentVariable("PAYOS_PAYOUT_CHECKSUM_KEY")
                    ?? configuration["PayOS:ChecksumKey"]
                    ?? string.Empty,
                LogLevel = LogLevel.Debug,
                HttpClient = payOsProxyClient
            }));

            services.AddScoped<IPayOSService, PayOSService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddHostedService<JobPostStatusBackgroundService>();
            services.AddScoped<IDisputeReportService, DisputeReportService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IWalletTransactionService, WalletTransactionService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddHttpClient<IWeatherService, WeatherService>();
            
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
            services.AddScoped<IPayOSService, PayOSService>();
            services.AddSingleton<PayOSClient>(_ => new PayOSClient(new PayOSOptions
            {
                ClientId = configuration["PayOS:ClientId"] ?? string.Empty,
                ApiKey = configuration["PayOS:ApiKey"] ?? string.Empty,
                ChecksumKey = configuration["PayOS:ChecksumKey"] ?? string.Empty,
                HttpClient = payOsProxyClient
            }));

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

            //services.Configure<PayOSSetting>(options =>
            //{
            //    options.ClientId = configuration["PayOS:ClientId"];
            //    options.ApiKey = configuration["PayOS:ApiKey"];
            //    options.ChecksumKey = configuration["PayOS:ChecksumKey"];
            //});
            //PayOSSetting.Instance = services.BuildServiceProvider().GetService<IOptions<PayOSSetting>>().Value;
        }
    }
}
