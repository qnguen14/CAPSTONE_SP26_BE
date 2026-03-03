namespace AgroTemp.API.Constants;

public class ApiEndpointConstants
{
    static ApiEndpointConstants() { }

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
        public const string GetAllUsersEndpoint = UserEndpoint;
        public const string GetUserByIdEndpoint = UserEndpoint + "/{id}";
        public const string CreateUserEndpoint = UserEndpoint;
        public const string UpdateUserEndpoint = UserEndpoint + "/{id}";
        public const string DeleteUserEndpoint = UserEndpoint + "/{id}";
    }

    public static class FarmerProfile
    {
        public const string FarmerProfileEndpoint = ApiEndpoint + "/farmer-profile";
        public const string GetFarmerProfileEndpoint = FarmerProfileEndpoint + "/{userId}";
        public const string UpdateFarmerProfileEndpoint = FarmerProfileEndpoint + "/{userId}";
    }

    public static class Farm 
    {
        public const string FarmEndpoint = ApiEndpoint + "/farm";
        public const string GetFarmsEndpoint = FarmEndpoint;
        public const string GetFarmByIdEndpoint = FarmEndpoint + "/{id}";
        public const string CreateFarmEndpoint = FarmEndpoint;
        public const string UpdateFarmEndpoint = FarmEndpoint + "/{id}";
        public const string DeleteFarmEndpoint = FarmEndpoint + "/{id}";
    }

    public static class WorkerProfile
    {
        public const string WorkerProfileEndpoint = ApiEndpoint + "/worker-profile";
        public const string GetWorkerProfileEndpoint = WorkerProfileEndpoint + "/{userId}";
        public const string UpdateWorkerProfileEndpoint = WorkerProfileEndpoint + "/{userId}";
    }
    public static class Job
    {
        public const string JobEndpoint = ApiEndpoint + "/job";
        public const string JobCategoryEndpoint = JobEndpoint + "/category";
        public const string JobPostEndpoint = JobEndpoint + "/post";
        public const string JobApplicationEndpoint = JobEndpoint + "/application";

        public const string GetAllJobCategoriesEndpoint = JobCategoryEndpoint;
        public const string GetAllJobPostsEndpoint = JobPostEndpoint;
        public const string GetAllJobApplicationsEndpoint = JobApplicationEndpoint;

        public const string GetJobCategoryByIdEndpoint = JobCategoryEndpoint + "/{Id}";
        public const string GetJobPostByIdEndpoint = JobPostEndpoint + "/{Id}";
        public const string GetJobApplicationsByPostIdEndpoint = JobApplicationEndpoint + "/{Id}";

        public const string CreateJobCategoryEndpoint = JobCategoryEndpoint + "/create";
        public const string CreateJobPostEndpoint = JobPostEndpoint + "/create";
        public const string CreateJobApplicationEndpoint = JobApplicationEndpoint + "/create";

        public const string UpdateJobCategoryEndpoint = JobCategoryEndpoint + "/update";
        public const string UpdateJobPostEndpoint = JobPostEndpoint + "/update";
        public const string UpdateJobApplicationEndpoint = JobApplicationEndpoint + "/update";

        public const string DeleteJobCategoryEndpoint = JobCategoryEndpoint + "/delete";
        public const string DeleteJobPostEndpoint = JobPostEndpoint + "/delete";
        public const string DeleteJobApplicationEndpoint = JobApplicationEndpoint + "/delete";

        public const string UpdateJobPostStatusEndpoint = JobPostEndpoint + "/update-status";
    }

    public static class WorkerAttendance
    {
        public const string WorkerAttendanceEndpoint = ApiEndpoint + "/attendance";
        
        // Worker endpoints
        public const string CheckInEndpoint = WorkerAttendanceEndpoint + "/check-in";
        public const string CheckOutEndpoint = WorkerAttendanceEndpoint + "/check-out";
        public const string GetAttendanceByIdEndpoint = WorkerAttendanceEndpoint + "/{id}";
        public const string GetWorkerAttendanceHistoryEndpoint = WorkerAttendanceEndpoint + "/worker/{workerProfileId}";
        
        // Farmer endpoints
        public const string ApproveAttendanceEndpoint = WorkerAttendanceEndpoint + "/approve";
        public const string GetFarmAttendanceRecordsEndpoint = WorkerAttendanceEndpoint + "/farm/{farmerProfileId}";
        public const string GetWorkerAttendanceByFarmerEndpoint = WorkerAttendanceEndpoint + "/farm/{farmerProfileId}/worker/{workerProfileId}";
    }
}