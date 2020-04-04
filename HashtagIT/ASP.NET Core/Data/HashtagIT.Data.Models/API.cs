namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using InstagramApiSharp.API;
    using InstagramApiSharp.API.Builder;
    using InstagramApiSharp.Classes;

    public class API
    {
        private static Dictionary<string, Dictionary<string, IInstaApi>> instagrams;
        private static Dictionary<string, UserSessionData> users;

        public API()
        {
            instagrams = new Dictionary<string, Dictionary<string, IInstaApi>>();
            users = new Dictionary<string, UserSessionData>();
        }

        public IInstaApi GetInstance(string userId, string username = null)
        {
            if (username != null && instagrams.ContainsKey(userId))
            {
                return instagrams[userId][username];
            }
            else if (instagrams.ContainsKey(userId))
            {
                return instagrams[userId].FirstOrDefault().Value;
            }
            else
            {
                return null;
            }
        }

        public async Task<IInstaApi> Login(string userId, string username, string password)
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

            if (instagrams.ContainsKey(userId))
            {
                instagrams[userId][username] = api;
            }
            else
            {
                instagrams[userId] = new Dictionary<string, IInstaApi>();
                instagrams[userId][username] = api;
            }

            return api;
        }

        public UserSessionData GetByName(string username)
        {
            return users[username];
        }
    }
}
