using Lummo.Application.Common.Notifications.Brokers.Interfaces;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Lummo.Infrastructure.Common.Notifications.Brokers;

public class SmtpEmailSenderBroker(IOptions<SmtpEmailSenderSettings> emailSenderSettings) : IEmailSenderBroker
{
    private readonly SmtpEmailSenderSettings _smtpEmailSenderSettings = emailSenderSettings.Value;

    public ValueTask<bool> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        // create new mail message
        var mail = new MailMessage(emailMessage.SenderEmailAddress, emailMessage.ReceiverEmailAddress);
        mail.Subject = emailMessage.Subject;
        mail.Body = emailMessage.Body;
        mail.IsBodyHtml = true;

        //create smtpClient and send mail message to email
        var smtpClient = new SmtpClient(_smtpEmailSenderSettings.Host, _smtpEmailSenderSettings.Port);
        smtpClient.Credentials =
            new NetworkCredential(_smtpEmailSenderSettings.CredentialAddress, _smtpEmailSenderSettings.Password);
        smtpClient.EnableSsl = true;

        smtpClient.Send(mail);

        return new ValueTask<bool>(true);
    }
}
