using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TaskManagement.Services
{
    public static class MailService
    {
        public static void SendMessage(string email, string subject, string body, string code = null)
        {
            try
            {
                var message = new MimeMessage();

                var from = new MailboxAddress("TaskManagement", "TaskManagement@gmail.com");
                message.From.Add(from);

                var to = new MailboxAddress("User", email);
                message.To.Add(to);

                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body // Directly using the HTML body provided as an argument
                };

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate("osamaalaayahoocom@gmail.com", "wwihcrbiformsxcp");
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {

                 Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
