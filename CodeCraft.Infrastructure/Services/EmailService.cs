using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Services;
public class EmailService: IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var host = _config["Email:Host"] ?? "smtp.gmail.com";
        var port = int.TryParse(_config["Email:Port"], out var p) ? p : 587;
        var username = _config["Email:Username"] ?? string.Empty;
        var password = _config["Email:Password"] ?? string.Empty;

        using var smtp = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        using var message = new MailMessage(username, to, subject, body)
        {
            IsBodyHtml = true
        };

        await smtp.SendMailAsync(message);
    }


    
}
