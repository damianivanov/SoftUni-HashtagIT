namespace HashtagIT.Services.Data
{
    using System.Threading.Tasks;

    using HashtagIT.Web.ViewModels.IG;

    public interface IIGService
    {
        Task<string> Login(string userId, string username, string password);

        Task<bool> TwoFactor(string username, string password, string code, string userId);

        Task<TopNineViewModel> TopNine(string userId, string username);

        Task<NotFollowingViewModel> NotFollowinBack(string userId, string username);

        Task<FriendshipViewModel> Friendship(string userId, string username);
    }
}
