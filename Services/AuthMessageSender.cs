namespace ASC.Web.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.FromResult(0);
        }
        public Task SendMessageAsync(string phone, string message)
        {
            return Task.FromResult(0);
        }
    
    }
}
