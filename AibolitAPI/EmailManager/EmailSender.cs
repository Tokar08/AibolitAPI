using AibolitAPI.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AibolitAPI.EmailManager;

public class EmailSender : INotificationSender
{
    private readonly string _senderEmail;
    private readonly string _senderPassword;

    public EmailSender(string senderEmail, string senderPassword)
    {
        _senderEmail = senderEmail;
        _senderPassword = senderPassword;
    }

    public async Task SendAsync(string recipient, object model)
    {
        if (model is not IEmailTemplate template)
            throw new ArgumentException("Model must implement IEmailTemplate");

        var message = CreateMessage(recipient, template.Subject, template.GetBody(model));

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_senderEmail, _senderPassword);
            await client.SendAsync(message);
        }
        finally
        {
            if (client.IsConnected)
                await client.DisconnectAsync(true);
        }
    }

    private MimeMessage CreateMessage(string recipient, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Aibolit", _senderEmail));
        message.To.Add(new MailboxAddress("", recipient));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };
        return message;
    }
}