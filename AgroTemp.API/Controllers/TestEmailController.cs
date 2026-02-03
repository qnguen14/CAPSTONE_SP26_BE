using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[Route("api/v1/test-email")]
public class TestEmailController : Controller
{
    private readonly IEmailService _emailService;

    public TestEmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> SendTestEmail([FromBody] TestEmailRequest request)
    {
        await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);
        return Ok(new { Message = "Email sent successfully" });
    }
}

public class TestEmailRequest
{
    public string To { get; set; }
    public string Subject { get; set; } = "Test Email";
    public string Body { get; set; } = "<p>This is a test email from AgroTemp.</p>";
}
