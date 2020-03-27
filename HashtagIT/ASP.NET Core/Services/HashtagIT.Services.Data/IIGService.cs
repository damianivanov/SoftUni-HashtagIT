namespace HashtagIT.Services.Data
{
    using System.Threading.Tasks;

    public interface IIGService
    {
        Task<string> Login(string userId, string username, string password);

        Task<bool> TwoFactor(string code, string phoneNumber);
    }
}
