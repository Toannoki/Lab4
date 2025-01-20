namespace ASC.Web.Services
{
    public interface ISmsSender
    {
        Task SendMessageAsync(string phone, string message);
    }
}
