namespace AgroTemp.API.Configuration
{
    public static class AuthorizationConfiguration
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //Examples of policies

                //// Administrator: full access
                //options.AddPolicy("RequireAdminRole", policy =>
                //    policy.RequireRole("Admin"));

                //// Staff: customer service, management control
                //options.AddPolicy("RequireStaffRole", policy =>
                //    policy.RequireRole("Staff"));

                //// Customer: basic access to pet-related services
                //options.AddPolicy("RequireStudentRole", policy =>
                //    policy.RequireRole("Student"));

                //// Example of a combined policy (optional)
                //options.AddPolicy("RequireStaffOrAdmin", policy =>
                //    policy.RequireRole("Staff", "Admin"));
            });

            return services;
        }
    }
}
