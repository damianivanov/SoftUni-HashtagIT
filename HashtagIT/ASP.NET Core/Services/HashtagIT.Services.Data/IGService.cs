namespace HashtagIT.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using InstagramApiSharp;
    using InstagramApiSharp.API;
    using InstagramApiSharp.API.Builder;
    using InstagramApiSharp.Classes;

    public class IGService : IIGService
    {
        private readonly IDeletableEntityRepository<IGUser> igUsersRepository;
        private readonly API api;
        private IInstaApi instaApi;

        public IGService(IDeletableEntityRepository<IGUser> igUsersRepository, API api)
        {
            this.igUsersRepository = igUsersRepository;
            this.api = api;
        }

        public async Task<string> Login(string userId, string username, string password)
        {
            this.instaApi = await this.api.Login(username, password);
            if (!this.instaApi.IsUserAuthenticated)
            {
                var challenge = await this.instaApi.GetChallengeRequireVerifyMethodAsync();
                return challenge.Value.StepData.PhoneNumber;
            }

            // succsefull login
            var user = new UserSessionData
            {
                UserName = username,
                Password = password,
            };
            this.Add(userId, user);
            return string.Empty;
        }

        public async Task<bool> TwoFactor(string username, string password, string code, string userId)
        {
            this.instaApi = this.api.GetInstance(username);
            await this.instaApi.VerifyCodeForChallengeRequireAsync(code);
            if (this.instaApi.IsUserAuthenticated)
            {
                var user = new UserSessionData
                {
                    UserName = username,
                    Password = password,
                };
                this.Add(userId, user);
                return true;
            }

            return false;
        }

        private async void Add(string userId, UserSessionData user)
        {
            var followersCount = await this.instaApi.UserProcessor.GetUserFollowersAsync(user.UserName, PaginationParameters.Empty);
            var followingCount = await this.instaApi.UserProcessor.GetUserFollowingAsync(user.UserName, PaginationParameters.Empty);
            IGUser igUser = new IGUser
            {
                UserId = userId,
                IGFullname = user.LoggedInUser.FullName,
                IGProfileId = user.LoggedInUser.Pk,
                IGUserName = user.LoggedInUser.UserName,
                ProfilePicUrl = user.LoggedInUser.ProfilePicUrl,
                FollowersCount = followersCount.Value.Count,
                FollowingCount = followingCount.Value.Count,
            };
            var existingUser = this.igUsersRepository.All().Where(u => u.IGUserName == igUser.IGUserName).FirstOrDefault();
            if (existingUser != null)
            {
                this.igUsersRepository.Update(igUser);
            }
            else
            {
                await this.igUsersRepository.AddAsync(igUser);
            }

            await this.igUsersRepository.SaveChangesAsync();
        }
    }
}
