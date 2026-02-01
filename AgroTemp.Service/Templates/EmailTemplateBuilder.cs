using System.Text;

namespace AgroTemp.Service.Templates;

public static class EmailTemplateBuilder
{
    private const string PrimaryColor = "#2E7D32"; 
    private const string AccentColor = "#4CAF50";
    private const string TextColor = "#333333";
    private const string BackgroundColor = "#f4f4f4";

    public static string BuildBasicTemplate(string title, string content, string buttonText = null, string buttonUrl = null)
    {
        var sb = new StringBuilder();

        sb.Append($@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            margin: 0;
            padding: 0;
            background-color: {BackgroundColor};
            color: {TextColor};
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }}
        .header {{
            background-color: {PrimaryColor};
            color: white;
            padding: 20px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .logo-text {{
            font-size: 28px;
            font-weight: bold;
            letter-spacing: 1px;
        }}
        .content {{
            padding: 30px;
        }}
        .button {{
            display: inline-block;
            background-color: {PrimaryColor};
            color: white;
            padding: 12px 24px;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 20px;
            font-weight: bold;
        }}
        .footer {{
            background-color: #eeeeee;
            padding: 15px;
            text-align: center;
            font-size: 12px;
            color: #666666;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <!-- You can replace this text with an <img> tag if you have a hosted logo URL -->
            <div class=""logo-text"">AgroTemp</div> 
        </div>
        <div class=""content"">
            <h2>{title}</h2>
            {content}
            ");

        if (!string.IsNullOrEmpty(buttonText) && !string.IsNullOrEmpty(buttonUrl))
        {
            sb.Append($@"<div style=""text-align: center;"">
                <a href=""{buttonUrl}"" class=""button"">{buttonText}</a>
            </div>");
        }

        sb.Append($@"
        </div>
        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} AgroTemp. All rights reserved.</p>
            <p>This is an automated message, please do not reply.</p>
        </div>
    </div>
</body>
</html>");

        return sb.ToString();
    }
}
