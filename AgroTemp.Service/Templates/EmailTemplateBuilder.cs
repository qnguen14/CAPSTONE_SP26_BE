using System.Text;

namespace AgroTemp.Service.Templates;

public static class EmailTemplateBuilder
{
    private const string PrimaryColor = "#2E7D32";
    private const string AccentColor = "#4CAF50";
    private const string TextColor = "#333333";
    private const string SubTextColor = "#6B7280";
    private const string BackgroundColor = "#F0F4F0";

    public static string BuildBasicTemplate(string title, string content, string buttonText = null, string buttonUrl = null)
    {
        var sb = new StringBuilder();

        sb.Append($@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
</head>
<body style=""margin:0;padding:0;background-color:{BackgroundColor};font-family:'Segoe UI',Tahoma,Geneva,Verdana,sans-serif;color:{TextColor};"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:{BackgroundColor};padding:32px 0;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px;width:100%;background:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,0.08);"">

                    <!-- HEADER -->
                    <tr>
                        <td style=""background:linear-gradient(135deg,{PrimaryColor} 0%,{AccentColor} 100%);padding:32px 40px;text-align:center;"">
                            <div style=""display:inline-flex;align-items:center;gap:10px;"">
                                <span style=""font-size:32px;"">🌾</span>
                                <span style=""color:white;font-size:26px;font-weight:700;letter-spacing:1px;"">AgroTemp</span>
                            </div>
                        </td>
                    </tr>

                    <!-- BODY -->
                    <tr>
                        <td style=""padding:40px 48px 32px;"">
                            <h2 style=""margin:0 0 16px;font-size:20px;font-weight:600;color:{PrimaryColor};"">
                                {title}
                            </h2>
                            {content}
                        </td>
                    </tr>");

        if (!string.IsNullOrEmpty(buttonText) && !string.IsNullOrEmpty(buttonUrl))
        {
            sb.Append($@"
                    <tr>
                        <td style=""padding:0 48px 32px;text-align:center;"">
                            <a href=""{buttonUrl}"" style=""display:inline-block;background-color:{PrimaryColor};color:white;padding:14px 32px;text-decoration:none;border-radius:8px;font-weight:600;font-size:15px;"">
                                {buttonText}
                            </a>
                        </td>
                    </tr>");
        }

        sb.Append($@"
                    <!-- FOOTER -->
                    <tr>
                        <td style=""background-color:#F9FAFB;border-top:1px solid #E5E7EB;padding:20px 40px;text-align:center;"">
                            <p style=""margin:0 0 4px;font-size:12px;color:{SubTextColor};"">© {DateTime.Now.Year} AgroTemp. Bảo lưu mọi quyền.</p>
                            <p style=""margin:0;font-size:12px;color:{SubTextColor};"">Đây là email tự động, vui lòng không trả lời email này.</p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>");

        return sb.ToString();
    }

    /// <summary>
    /// Builds a styled OTP block for use inside email content.
    /// </summary>
    public static string BuildOtpBlock(string otp, string expiryMinutes = "15")
    {
        return $@"
            <div style=""text-align:center;margin:28px 0;"">
                <p style=""margin:0 0 16px;font-size:15px;color:#374151;"">Mã xác minh của bạn là:</p>
                <div style=""display:inline-block;background:linear-gradient(135deg,#F0FDF4,#DCFCE7);border:2px solid {AccentColor};border-radius:12px;padding:20px 40px;"">
                    <span style=""font-size:40px;font-weight:700;letter-spacing:12px;color:{PrimaryColor};font-family:'Courier New',monospace;"">{otp}</span>
                </div>
                <p style=""margin:16px 0 0;font-size:13px;color:#6B7280;"">
                    ⏱ Mã này có hiệu lực trong <strong>{expiryMinutes} phút</strong>.
                </p>
            </div>";
    }
}
