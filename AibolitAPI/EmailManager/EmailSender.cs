using AibolitAPI.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AibolitAPI.EmailManager;

public class EmailSender(string senderEmail, string senderPassword) : INotificationSender
{
    private readonly string _senderEmail = senderEmail ?? throw new ArgumentNullException(nameof(senderEmail));
    private readonly string _senderPassword = senderPassword ?? throw new ArgumentNullException(nameof(senderPassword));

    public async Task SendAsync(string recipient, IEmailTemplate template, object model)
    {
        if (string.IsNullOrEmpty(recipient))
            throw new ArgumentException("Recipient email cannot be null or empty.", nameof(recipient));

        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(model);

        var subject = template.Subject;
        string body;

        body = template.GetBody(model);
        var inlinedBody = PreMailer.Net.PreMailer.MoveCssInline(body);

        var message = CreateMessage(recipient, subject, inlinedBody.Html);
        Console.WriteLine(string.Join(" ", inlinedBody.Warnings));
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
        message.From.Add(new MailboxAddress("AibolIT", _senderEmail));
        message.To.Add(MailboxAddress.Parse(recipient));
        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = body
        };

        return message;
    }
}