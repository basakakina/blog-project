using Microsoft.AspNetCore.Identity.UI.Services;

namespace WEB.Infrastructure
{
    public class NullOrEmailSender
    {
        public class NullEmailSender : IEmailSender
        {
            public Task SendEmailAsync(string email, string subject, string htmlMessage)
                => Task.CompletedTask;
        }
    }
}
