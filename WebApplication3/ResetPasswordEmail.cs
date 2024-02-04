using System.Net.Mail;
using System.Net;
using System.Text;

using System.Linq;

namespace WebApplication3
{
	public class ResetPasswordEmail : IEmailSender
	{
		public void SendEmail(string toEmail, string subject, string token)
		{
			// Set up SMTP client
			using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
			{
				client.EnableSsl = true;
				client.UseDefaultCredentials = false;
				client.Credentials = new NetworkCredential("carljdevan@gmail.com", "kwyh vhsi mmde frug\r\n");

				// Create email message
				using (MailMessage mailMessage = new MailMessage())
				{
					mailMessage.From = new MailAddress("carljdevan@gmail.com");
					mailMessage.To.Add(toEmail);
					mailMessage.Subject = subject;
					mailMessage.IsBodyHtml = true;

					StringBuilder mailBody = new StringBuilder();
					mailBody.AppendFormat("<h1>Reset Password</h1>");
					mailBody.AppendFormat("<br />");
					mailBody.AppendFormat($"<p>Your reset token is: {token}</p>");
					mailMessage.Body = mailBody.ToString();

					// Send email
					client.Send(mailMessage);
				}
			}
		}
	}
}
