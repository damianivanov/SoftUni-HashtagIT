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
        private IInstaApi instaApi;

        public IGService(IDeletableEntityRepository<IGUser> igUsersRepository)
        {
            this.igUsersRepository = igUsersRepository;
        }

        public async Task<string> Login(string userId, string username, string password)
        {
            var user = new UserSessionData
            {
                UserName = username,
                Password = password,
            };
            this.instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(user).Build();

                // login
            var logInResult = await this.instaApi.LoginAsync();
            if (!logInResult.Succeeded)
            {
                var challenge = await this.instaApi.GetChallengeRequireVerifyMethodAsync();
                if (challenge.Succeeded)
                {
                    var phoneNumber = await this.instaApi.RequestVerifyCodeToSMSForChallengeRequireAsync();
                    return phoneNumber.Value.StepData.PhoneNumberPreview;
                }
            }

            var followersCount = await this.instaApi.UserProcessor.GetUserFollowersAsync(username, PaginationParameters.Empty);
            var followingCount = await this.instaApi.UserProcessor.GetUserFollowingAsync(username, PaginationParameters.Empty);
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
            return string.Empty;
        }

        public async Task<bool> TwoFactor(string code, string phoneNumber)
        {
                var verifyLogin = await this.instaApi.VerifyCodeForChallengeRequireAsync(code);
                var info = await this.instaApi.GetLoggedInChallengeDataInfoAsync();

                return verifyLogin.Succeeded;
        }
    }
}
