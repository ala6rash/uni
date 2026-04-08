using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Uni_Connect.Services
{
    /// <summary>
    /// Sends real emails using Gmail SMTP via MailKit.
    /// Configured via appsettings.json → EmailSettings section.
    /// </summary>
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Sends the 6-digit verification code to the student's university email.
        /// </summary>
        public async Task SendVerificationEmailAsync(string toEmail, string studentName, string code)
        {
            var subject = "UniConnect — Verify your email address";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'/>
  <style>
    body {{ font-family: 'Segoe UI', Arial, sans-serif; background:#F8F9FF; margin:0; padding:0; }}
    .wrap {{ max-width:520px; margin:40px auto; background:#fff; border-radius:16px; overflow:hidden; box-shadow:0 4px 24px rgba(61,82,160,.1); }}
    .header {{ background:linear-gradient(135deg,#3D52A0,#7091E6); padding:36px 32px; text-align:center; }}
    .header-icon {{ font-size:48px; display:block; margin-bottom:8px; }}
    .header-title {{ color:#fff; font-size:24px; font-weight:800; margin:0; }}
    .body {{ padding:32px; }}
    .greeting {{ font-size:16px; color:#0F172A; margin-bottom:16px; }}
    .msg {{ font-size:14px; color:#475569; line-height:1.7; margin-bottom:24px; }}
    .code-box {{ background:#EEF2FF; border:2px dashed #3D52A0; border-radius:12px; padding:20px; text-align:center; margin:0 auto 24px; max-width:260px; }}
    .code {{ font-size:42px; font-weight:900; letter-spacing:10px; color:#3D52A0; font-family:monospace; }}
    .code-label {{ font-size:12px; color:#64748B; margin-top:6px; font-weight:600; }}
    .warn {{ font-size:13px; color:#F59E0B; background:#FEF3C7; border-radius:8px; padding:10px 14px; margin-bottom:24px; }}
    .footer {{ background:#F8F9FF; padding:20px 32px; text-align:center; font-size:12px; color:#94A3B8; border-top:1px solid #E2E8F0; }}
  </style>
</head>
<body>
<div class='wrap'>
  <div class='header'>
    <span class='header-icon'>🎓</span>
    <h1 class='header-title'>UniConnect</h1>
  </div>
  <div class='body'>
    <div class='greeting'>Hello, <strong>{studentName}</strong> 👋</div>
    <div class='msg'>
      Welcome to UniConnect — Philadelphia University's academic Q&amp;A platform!
      <br><br>
      Please enter the verification code below to activate your account:
    </div>
    <div class='code-box'>
      <div class='code'>{code}</div>
      <div class='code-label'>This code expires in <strong>15 minutes</strong></div>
    </div>
    <div class='warn'>⚠️ If you didn't create a UniConnect account, you can safely ignore this email.</div>
    <div style='font-size:13px;color:#475569'>Once verified, you'll start with <strong style='color:#F59E0B'>🪙 50 welcome points</strong>!</div>
  </div>
  <div class='footer'>
    &copy; {DateTime.Now.Year} UniConnect · Philadelphia University · Jordan<br>
    This is an automated message, please do not reply.
  </div>
</div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        /// <summary>
        /// Core method that connects to Gmail SMTP and sends the message.
        /// </summary>
        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var smtpServer  = _config["EmailSettings:SmtpServer"]  ?? "smtp.gmail.com";
                var smtpPort    = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
                var senderEmail = _config["EmailSettings:SenderEmail"] ?? "";
                var senderName  = _config["EmailSettings:SenderName"]  ?? "UniConnect";
                var appPassword = _config["EmailSettings:AppPassword"] ?? "";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, appPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Verification email sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw; // Let the controller handle the error
            }
        }
    }
}
