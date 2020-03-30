namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using InstagramApiSharp.API;
    using InstagramApiSharp.API.Builder;
    using InstagramApiSharp.Classes;

    public class API
    {
        private static Dictionary<string, IInstaApi> instagrams;
        private static Dictionary<string, UserSessionData> users;

        public API()
        {
            instagrams = new Dictionary<string, IInstaApi>();
            users = new Dictionary<string, UserSessionData>();
        }

        public IInstaApi GetInstance(string username)
        {
            return instagrams[username];
        }

        public async Task<IInstaApi> Login(string username, string password)
        {
            var user = new UserSessionData
            {
                UserName = username,
                Password = password,
            };
            if (!users.ContainsKey(username))
            {
                users.Add(username, user);
            }

            var api = InstaApiBuilder.CreateBuilder().SetUser(user).Build();
            var logInResult = await api.LoginAsync();
            if (!logInResult.Succeeded)
            {
                var challenge = await api.GetChallengeRequireVerifyMethodAsync();
                if (challenge.Succeeded)
                {
                     await api.RequestVerifyCodeToSMSForChallengeRequireAsync();
                }
            }

            instagrams[username] = api;
            return api;
        }

        public UserSessionData GetByName(string username)
        {
            return users[username];
        }
    }
}
