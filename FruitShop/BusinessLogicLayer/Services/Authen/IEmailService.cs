namespace BusinessLogicLayer.Services.Authen
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}
