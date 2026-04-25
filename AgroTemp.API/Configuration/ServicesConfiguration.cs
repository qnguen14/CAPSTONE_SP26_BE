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

            // Build a dedicated proxy client for payout flows only.
            // This keeps deposit/create-payment traffic direct (no proxy),
            // while still supporting static IP for withdraw/payout calls.
            HttpClient? payoutProxyClient = null;
            var payoutFixieUrl = Environment.GetEnvironmentVariable("FIXIE_URL_PAYOUT")
                                ?? Environment.GetEnvironmentVariable("FIXIE_URL");
            if (!string.IsNullOrWhiteSpace(payoutFixieUrl))
            {
                var proxyUri = new Uri(payoutFixieUrl);
                var proxy = new System.Net.WebProxy(proxyUri.Host, proxyUri.Port);

                if (!string.IsNullOrEmpty(proxyUri.UserInfo))
                {
                    var authParts = proxyUri.UserInfo.Split(':');
                    if (authParts.Length == 2)
                    {
                        proxy.Credentials = new System.Net.NetworkCredential(authParts[0], authParts[1]);
                    }
                }

                var handler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true,
                    PreAuthenticate = true
                };

                var loggingHandler = new PayOSLoggingHandler(handler);
                payoutProxyClient = new HttpClient(loggingHandler);
            }

            // Helper function to get config with fallbacks and handle empty strings
            string GetConfig(string key, string envKey, string? fallback = null)
            {
                var val = configuration[key];
                if (string.IsNullOrWhiteSpace(val))
                {
                    val = Environment.GetEnvironmentVariable(envKey);
                }
                return !string.IsNullOrWhiteSpace(val) ? val : (fallback ?? string.Empty);
            }

            // payOS client for payment link flow (deposit/top-up)
            services.AddKeyedSingleton<PayOSClient>("OrderClient", (sp, _) => new PayOSClient(new PayOSOptions
            {
                ClientId = GetConfig("PayOS:ClientId", "PAYOS_CLIENT_ID"),
                ApiKey = GetConfig("PayOS:ApiKey", "PAYOS_API_KEY"),
                ChecksumKey = GetConfig("PayOS:ChecksumKey", "PAYOS_CHECKSUM_KEY"),
                LogLevel = LogLevel.Debug,
                HttpClient = null
            }));

            // payOS client for payout flow (withdraw)
            services.AddKeyedSingleton<PayOSClient>("TransferClient", (sp, _) => new PayOSClient(new PayOSOptions
            {
                ClientId = GetConfig("PayOS:PayoutClientId", "PAYOS_PAYOUT_CLIENT_ID", GetConfig("PayOS:ClientId", "PAYOS_CLIENT_ID")),
                ApiKey = GetConfig("PayOS:PayoutApiKey", "PAYOS_PAYOUT_API_KEY", GetConfig("PayOS:ApiKey", "PAYOS_API_KEY")),
                ChecksumKey = GetConfig("PayOS:PayoutChecksumKey", "PAYOS_PAYOUT_CHECKSUM_KEY", GetConfig("PayOS:ChecksumKey", "PAYOS_CHECKSUM_KEY")),
                LogLevel = LogLevel.Debug,
                HttpClient = payoutProxyClient
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
            
            // Register a default PayOSClient (unkeyed) that uses Order keys as a fallback
            services.AddSingleton<PayOSClient>(_ => new PayOSClient(new PayOSOptions
            {
                ClientId = GetConfig("PayOS:ClientId", "PAYOS_CLIENT_ID"),
                ApiKey = GetConfig("PayOS:ApiKey", "PAYOS_API_KEY"),
                ChecksumKey = GetConfig("PayOS:ChecksumKey", "PAYOS_CHECKSUM_KEY"),
                HttpClient = null
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

    public class PayOSLoggingHandler : DelegatingHandler
    {
        public PayOSLoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[PayOS Request] {request.Method} {request.RequestUri}");
            if (request.Content != null)
            {
                var requestContent = await request.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"[PayOS Request Content] {requestContent}");
            }

            var response = await base.SendAsync(request, cancellationToken);

            Console.WriteLine($"[PayOS Response] {response.StatusCode}");
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"[PayOS Response Content] {responseContent}");
            }

            return response;
        }
    }
}
