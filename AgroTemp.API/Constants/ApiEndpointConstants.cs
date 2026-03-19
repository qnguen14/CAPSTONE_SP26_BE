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
        public const string ResetPasswordEndpoint = ApiEndpoint + "/reset";
        public const string ForgetPasswordEndpoint = ApiEndpoint + "/forget";
        // public const string DisableAccountEndpoint = ApiEndpoint + "/disable";
        // public const string VerifyDisableCodeEndpoint = ApiEndpoint + "/verify/disable";
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
        public const string FarmerProfileEndpoint = ApiEndpoint + "/farmer";
        public const string GetFarmerProfileEndpoint = FarmerProfileEndpoint;
        public const string UpdateFarmerProfileEndpoint = FarmerProfileEndpoint;
        public const string UploadAvatarEndpoint = FarmerProfileEndpoint + "/upload-avatar";
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
        public const string WorkerProfileEndpoint = ApiEndpoint + "/worker";
        public const string GetWorkerProfileEndpoint = WorkerProfileEndpoint;
        public const string UpdateWorkerProfileEndpoint = WorkerProfileEndpoint;
        public const string UploadAvatarEndpoint = WorkerProfileEndpoint + "/upload-avatar";
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

        public const string GetJobCategoryByIdEndpoint = JobCategoryEndpoint + "/{id}";
        public const string GetJobPostByIdEndpoint = JobPostEndpoint + "/{id}";
        public const string GetJobApplicationByIdEndpoint = JobApplicationEndpoint + "/{id}";

        public const string CreateJobCategoryEndpoint = JobCategoryEndpoint;
        public const string CreateJobPostEndpoint = JobPostEndpoint;
        public const string CreateJobApplicationEndpoint = JobApplicationEndpoint;

        public const string UpdateJobCategoryEndpoint = JobCategoryEndpoint + "/{id}";
        public const string UpdateJobPostEndpoint = JobPostEndpoint + "/{id}";
        public const string UpdateJobApplicationEndpoint = JobApplicationEndpoint + "/{id}";

        public const string DeleteJobCategoryEndpoint = JobCategoryEndpoint + "/{id}";
        public const string DeleteJobPostEndpoint = JobPostEndpoint + "/{id}";
        public const string DeleteJobApplicationEndpoint = JobApplicationEndpoint + "/{id}";
        public const string UpdateJobPostStatusEndpoint = JobPostEndpoint + "/update-status" + "/{id}";
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

    public static class Notification
    {
        public const string NotificationEndpoint = ApiEndpoint + "/notification";
        public const string GetNotificationsEndpoint = NotificationEndpoint;
        public const string GetUnreadNotificationsEndpoint = NotificationEndpoint + "/unread";
        public const string MarkAsReadEndpoint = NotificationEndpoint + "/read";
        public const string MarkAllAsReadEndpoint = NotificationEndpoint + "/read-all";
        public const string DeleteNotificationEndpoint = NotificationEndpoint + "/{id}";

        public const string RegisterTokenEndpoint = NotificationEndpoint + "/register-token";
        public const string UnregisterTokenEndpoint = NotificationEndpoint + "/unregister-token";
    }

    public static class Media
    {
        public const string MediaEndpoint = ApiEndpoint + "/media";
        public const string MediaUploadEndpoint = MediaEndpoint + "/upload";
        public const string MediaDeleteEndpoint = MediaEndpoint + "/delete";

        // Image endpoints
        public const string UploadImageEndpoint = MediaUploadEndpoint + "/image";
        public const string UploadImagesEndpoint = MediaUploadEndpoint + "/images";

        // Video endpoints
        public const string UploadVideoEndpoint = MediaUploadEndpoint + "/video";
        public const string UploadVideosEndpoint = MediaUploadEndpoint + "/videos";

        // Raw file endpoints
        public const string UploadRawFileEndpoint = MediaUploadEndpoint + "/raw-file";
        public const string UploadRawFilesEndpoint = MediaUploadEndpoint + "/raw-files";

        // Media deletion endpoints
        public const string DeleteResourceEndpoint = MediaDeleteEndpoint + "/resource";
        public const string DeleteResourcesEndpoint = MediaDeleteEndpoint + "/resources";
    }
}