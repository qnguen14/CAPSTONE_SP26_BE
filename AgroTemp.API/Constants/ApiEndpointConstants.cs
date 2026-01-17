namespace AgroTemp.API.Constants;

public class ApiEndpointConstants
{
    static ApiEndpointConstants() {}
    
    public const string RootEndpoint = "/api";
    public const string ApiVersion = "/v1";
    public const string ApiEndpoint = RootEndpoint + ApiVersion;

    public static class Auth
    {
        public const string AuthEndpoint = ApiEndpoint + "/auth";
        public const string LoginEndpoint = ApiEndpoint + "/login";
        public const string RegisterEndpoint = ApiEndpoint + "/register";
        public const string GoogleLoginEndpoint = ApiEndpoint + "/google-login";
        public const string SendResetCodeEndpoint = ApiEndpoint + "/reset";
        public const string VerifyResetCodeEndpoint = ApiEndpoint + "/verify/reset";
        public const string DisableAccountEndpoint = ApiEndpoint + "/disable";
        public const string VerifyDisableCodeEndpoint = ApiEndpoint + "/verify/disable";
        public const string LogoutEndpoint = ApiEndpoint + "/logout";
    }

    public static class User
    { 
        public const string UserEndpoint = ApiEndpoint + "/user";
        public const string GetProfileEndpoint = UserEndpoint + "/profile";
        public const string UpdateProfileEndpoint = UserEndpoint + "/update";
        public const string GetAllUsersEndpoint = UserEndpoint;
    }
    
    public static class FarmerProfile
    {
        public const string FarmerProfileEndpoint = ApiEndpoint + "/farmer-profile";
        public const string GetFarmerProfileEndpoint = FarmerProfileEndpoint + "/{userId}";
        public const string UpdateFarmerProfileEndpoint = FarmerProfileEndpoint + "/{userId}";
    }
    
    public static class WorkerProfile
    {
        public const string WorkerProfileEndpoint = ApiEndpoint + "/worker-profile";
        public const string GetWorkerProfileEndpoint = WorkerProfileEndpoint + "/{userId}";
        public const string UpdateWorkerProfileEndpoint = WorkerProfileEndpoint + "/{userId}";
    }
}