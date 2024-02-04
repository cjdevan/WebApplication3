namespace WebApplication3
{
    public interface IEmailSender
    {
        void SendEmail(string email, string subject, string token);
    }
}
