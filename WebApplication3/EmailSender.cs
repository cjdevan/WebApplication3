using System.Net.Mail;
using System.Net;
using System.Text;

namespace WebApplication3
{
    public class EmailSender : IEmailSender
    {
        public void SendEmail(string toEmail, string subject, string token)
        {
            // Set up SMTP client
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("carljdevan@gmail.com", "kwyh vhsi mmde frug\r\n");

            // Create email message
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("carljdevan@gmail.com");
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            StringBuilder mailBody = new StringBuilder();
            mailBody.AppendFormat("<h1>Registration Successful</h1>");
            mailBody.AppendFormat("<br />");
            mailBody.AppendFormat("<p>Thank you for registering an account with Ace Job Agency.</p>");
            mailMessage.Body = mailBody.ToString();

            // Send email
            client.Send(mailMessage);
        }
    }
}
