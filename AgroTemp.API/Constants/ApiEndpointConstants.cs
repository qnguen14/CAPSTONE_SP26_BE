using AgroTemp.Domain.Entities;

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
        public const string LogoutEndpoint = ApiEndpoint + "/logout";
        public const string VerifyEmailEndpoint = ApiEndpoint + "/verify-email";
        public const string ResendVerificationEndpoint = ApiEndpoint + "/resend-verification";
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
        public const string GetSpecificFarmerProfileEndpoint = FarmerProfileEndpoint + "/{id}";
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
        public const string UploadFarmImageEndpoint = FarmEndpoint + "/{id}/upload-image";
    }

    public static class WorkerProfile
    {
        public const string WorkerProfileEndpoint = ApiEndpoint + "/worker";
        public const string GetSpecificWorkerProfileEndpoint = WorkerProfileEndpoint + "/{id}";
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
        public const string JobDetailEndpoint = JobEndpoint + "/detail";
        public const string GetJobApplicationsByJobPostEndpoint = JobApplicationEndpoint + "/post" + "/{jobPostId}";

        public const string GetAllJobCategoriesEndpoint = JobCategoryEndpoint;
        public const string GetAllJobPostsEndpoint = JobPostEndpoint;
        public const string GetAllJobApplicationsEndpoint = JobApplicationEndpoint;
        public const string GetAllJobDetailsEndpoint = JobDetailEndpoint;

        public const string GetJobCategoryByIdEndpoint = JobCategoryEndpoint + "/{id}";
        public const string GetJobPostByIdEndpoint = JobPostEndpoint + "/{id}";
        public const string GetJobPostsByFarmerEndpoint = JobPostEndpoint + "/farmer";
        public const string GetJobPostsByStatusEndpoint = JobPostEndpoint + "/status" + "/{status}";
        public const string GetFarmerJobHistoryEndpoint = JobPostEndpoint + "/farmer" + "/history";
        public const string GetJobApplicationByIdEndpoint = JobApplicationEndpoint + "/{id}";
        public const string GetJobApplicationsByWorkerEndpoint = JobApplicationEndpoint + "/worker";
        public const string GetWorkerApplicationStatsEndpoint = JobApplicationEndpoint + "/worker/stats";
        public const string GetJobDetailByIdEndpoint = JobDetailEndpoint + "/{id}";

        public const string CreateJobCategoryEndpoint = JobCategoryEndpoint;
        public const string CreateJobPostEndpoint = JobPostEndpoint;
        public const string CreateJobApplicationEndpoint = JobApplicationEndpoint;
        public const string CreateJobDetailEndpoint = JobDetailEndpoint;

        public const string UpdateJobCategoryEndpoint = JobCategoryEndpoint + "/{id}";
        public const string UpdateJobPostEndpoint = JobPostEndpoint + "/{id}";
        public const string UpdateJobApplicationEndpoint = JobApplicationEndpoint + "/{id}";
        public const string UpdateJobDetailEndpoint = JobDetailEndpoint + "/{id}";

        public const string DeleteJobCategoryEndpoint = JobCategoryEndpoint + "/{id}";
        public const string DeleteJobPostEndpoint = JobPostEndpoint + "/{id}";
        public const string DeleteJobApplicationEndpoint = JobApplicationEndpoint + "/{id}";
        public const string DeleteJobDetailEndpoint = JobDetailEndpoint + "/{id}";

        public const string UpdateJobPostStatusEndpoint = JobPostEndpoint + "/update-status" + "/{id}";
        public const string UpdateJobPostUrgencyEndpoint = JobPostEndpoint + "/update-urgency" + "/{id}";
        public const string UpdateJobDetailStatusEndpoint = JobDetailEndpoint + "/update-status" + "/{id}";

        public const string RespondJobApplicationEndpoint = JobApplicationEndpoint + "/respond" + "/{id}";
        public const string AutoAcceptUrgentJobApplicationsEndpoint = JobApplicationEndpoint + "/auto-accept";

        public const string GetFilteredJobPostsEndpoint = JobPostEndpoint + "/filter";
        public const string GetFilteredJobPostsByFarmerEndpoint = JobPostEndpoint + "/filter/farmer";

        public const string GetJobPostsForAdminEndpoint = JobPostEndpoint + "/admin";

        public const string SaveJobPostDraftEndpoint = JobPostEndpoint + "/draft";
        public const string GetFarmerDraftsEndpoint = JobPostEndpoint + "/drafts";

        public const string SearchJobsEndpoint = JobPostEndpoint + "/search";
        public const string GetNearbyJobsEndpoint = JobPostEndpoint + "/nearby";
        public const string GetJobsByDateEndpoint = JobPostEndpoint + "/by-date";
        public const string GetJobsBySkillEndpoint = JobPostEndpoint + "/by-skill";
        public const string GetJobsByWageRangeEndpoint = JobPostEndpoint + "/by-wage";
        public const string GetJobsByTypeEndpoint = JobPostEndpoint + "/by-type";
        public const string GetUrgentJobsEndpoint = JobPostEndpoint + "/urgent";

        public const string CancelJobPostEndpoint = JobPostEndpoint + "/cancel" + "/{id}";
        public const string CancelJobApplicationEndpoint = JobApplicationEndpoint + "/cancel" + "/{id}";
        public const string CancelJobApplicationForFarmerEndpoint = JobApplicationEndpoint + "/cancel/farmer" + "/{id}";

        public const string GetJobDetailByJobPost = JobDetailEndpoint + "/post" + "/{id}";
        public const string GetJobDetailByWorker = JobDetailEndpoint + "/worker" + "/{id}";
        public const string ReportDailyWorkerEndpoint = JobDetailEndpoint + "/report" + "/{id}";
        public const string ApproveJobDetailEndpoint = JobDetailEndpoint + "/approve" + "/{id}";

        public const string GetJobApplicationsByFarmer = JobApplicationEndpoint + "/farmer";

        public const string GetAcceptedWorkersPerDayEndpoint = JobPostEndpoint + "/{id}/workers-per-day";

        public const string ToggleSaveJobPostEndpoint = JobPostEndpoint + "/{id}/save";
        public const string GetSavedJobPostsEndpoint = JobPostEndpoint + "/saved";
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
        public const string GetMyActiveTokensEndpoint = NotificationEndpoint + "/tokens";
        public const string MarkAsReadEndpoint = NotificationEndpoint + "/read";
        public const string MarkAllAsReadEndpoint = NotificationEndpoint + "/read-all";
        public const string DeleteNotificationEndpoint = NotificationEndpoint + "/{id}";
        public const string SendPushNotificationEndpoint = NotificationEndpoint + "/send-push";

        public const string RegisterTokenEndpoint = NotificationEndpoint + "/register-token";
        public const string UnregisterTokenEndpoint = NotificationEndpoint + "/unregister-token";
    }

    public static class Messages
    {
        public const string MessagesEndpoint = ApiEndpoint + "/messages";

        public const string GetMessagesEndpoint = MessagesEndpoint;
        public const string SendMessageEndpoint = MessagesEndpoint;
        public const string GetRecentConversationsEndpoint = MessagesEndpoint + "/conversations";

        // Optional read-receipt endpoint (used later)
        public const string MarkConversationAsReadEndpoint = MessagesEndpoint + "/read";
    }

    public static class Payment
    {
        public const string PaymentEndpoint = ApiEndpoint + "/payment";
        public const string VerifyWebhookEndpoint = PaymentEndpoint + "/verify";
        public const string CallbackEndpoint = PaymentEndpoint + "/callback";

        public const string GetOrderEndpoint = PaymentEndpoint + "/{id}";
        public const string CreateOrderEndpoint = PaymentEndpoint;
        public const string CancelOrderEndpoint = PaymentEndpoint + "/{id}/cancel";
        public const string GetOrderInvoicesEndpoint = PaymentEndpoint + "/{id}/invoices";
        public const string DownloadOrderInvoiceEndpoint = PaymentEndpoint + "/{id}/invoices/{invoiceId}/download";
    }

    public static class Withdraw
    {
        public const string WithdrawEndpoint = ApiEndpoint + "/withdraw";
        public const string WithdrawByIdEndpoint = WithdrawEndpoint + "/{id}";
        public const string WithdrawBalanceEndpoint = WithdrawEndpoint + "/account-balance";
    }

    public static class Skill
    {
        public const string SkillEndpoint = ApiEndpoint + "/skills";
        public const string GetAllSkillsEndpoint = SkillEndpoint;
        public const string GetSkillsByCategoryPagedEndpoint = SkillEndpoint + "/category/{categoryId}";
        public const string GetSkillByIdEndpoint = SkillEndpoint + "/{id}";
        public const string CreateSkillEndpoint = SkillEndpoint;
        public const string UpdateSkillEndpoint = SkillEndpoint + "/{id}";
        public const string DeleteSkillEndpoint = SkillEndpoint + "/{id}";
    }

    public static class Rating
    {
        public const string RatingEndpoint = ApiEndpoint + "/ratings";
        public const string GetAllRatingsEndpoint = RatingEndpoint;
        public const string GetRatingByIdEndpoint = RatingEndpoint + "/{id}";
        public const string CreateRatingEndpoint = RatingEndpoint;
        public const string UpdateRatingEndpoint = RatingEndpoint + "/{id}";
        public const string DeleteRatingEndpoint = RatingEndpoint + "/{id}";

        public const string GetSpecificRatingByUserIdEndpoint = RatingEndpoint + "/user/{userId}";
        public const string GetAllRatingsByUserIdEndpoint = RatingEndpoint + "/user/{userId}/all";
        public const string GetGivenRatingsByUserEndpoint = RatingEndpoint + "/user/given";
        public const string GetReceivedRatingsByUserByPostIdEndpoint = RatingEndpoint + "/user/received/post/{postId}";
        public const string GetAverageRatingByUserIdEndpoint = RatingEndpoint + "/user/{userId}/average";
    }

    public static class Dispute
    {
        public const string DisputeEndpoint = ApiEndpoint + "/disputes";
        public const string GetAllDisputesEndpoint = DisputeEndpoint;
        public const string GetMyDisputesEndpoint = DisputeEndpoint + "/mine";
        public const string GetDisputeByIdEndpoint = DisputeEndpoint + "/{id:guid}";
        public const string CreateDisputeEndpoint = DisputeEndpoint;
        public const string UpdateDisputeEndpoint = DisputeEndpoint + "/{id:guid}";
        public const string DeleteDisputeEndpoint = DisputeEndpoint + "/{id:guid}";
        public const string ReviewDisputeEndpoint = DisputeEndpoint + "/{id:guid}/review";
        public const string ResolveDisputeEndpoint = DisputeEndpoint + "/{id:guid}/resolve";
    }

    public static class Weather
    {
        public const string WeatherEndpoint = ApiEndpoint + "/weather";
        public const string GetWeatherByCoordinatesEndpoint = WeatherEndpoint + "/coordinates";
        public const string GetWeatherByCityEndpoint = WeatherEndpoint + "/city";
        public const string GetWeatherByCurrentUserAddressEndpoint = WeatherEndpoint + "/me";
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

    public static class Wallet
    {
        public const string WalletEndpoint = ApiEndpoint + "/wallet";
        public const string GetAllWalletsEndpoint = WalletEndpoint;
        public const string GetWalletByIdEndpoint = WalletEndpoint + "/{id}";
        public const string GetMyWalletEndpoint = WalletEndpoint + "/me";
    }

    public static class WalletTransaction
    {
        public const string WalletTransactionEndpoint = ApiEndpoint + "/wallet-transaction";
        public const string GetAllWalletTransactionsEndpoint = WalletTransactionEndpoint;
        public const string GetWalletTransactionByIdEndpoint = WalletTransactionEndpoint + "/{id}";
        public const string GetWalletTransactionsByWalletIdEndpoint = WalletTransactionEndpoint + "/wallet/{walletId}";
    }

    public static class Dashboard
    {
        public const string FarmerDashboardEndpoint = FarmerProfile.FarmerProfileEndpoint + "/dashboard";
        public const string WorkerDashboardEndpoint = WorkerProfile.WorkerProfileEndpoint + "/dashboard";
    }

    public static class Admin
    {
        public const string AdminEndpoint = ApiEndpoint + "/admin";
        public const string GetDashboardEndpoint = AdminEndpoint + "/dashboard";
        public const string GetJobpostEndpoint = AdminEndpoint + "/jobpost";
        public const string WalletStatsEndpoint = AdminEndpoint + "/wallet/stats";
        public const string WithdrawalsEndpoint = AdminEndpoint + "/withdrawals";
        public const string WalletsEndpoint = AdminEndpoint + "/wallets";
        public const string WalletTransactionsEndpoint = AdminEndpoint + "/wallet-transactions";
    }
}