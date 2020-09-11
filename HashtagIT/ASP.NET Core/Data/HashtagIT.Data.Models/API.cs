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
    using Microsoft.Extensions.Configuration;

    public class API
    {
        private static Dictionary<string, Dictionary<string, IInstaApi>> instagrams;
        private static Dictionary<string, UserSessionData> users;
        private readonly IConfiguration configuration;

        public API(IConfiguration configuration)
        {
            this.configuration = configuration;
            instagrams = new Dictionary<string, Dictionary<string, IInstaApi>>();
            users = new Dictionary<string, UserSessionData>();

            // Make multiple system accounts and manage the traffic
            // _ = this.Login("system", this.configuration["Instagram:SystemUser"], this.configuration["Instagram:SystemPass"]).Result;
        }

        public IInstaApi GetInstance(string userId, string username = null)
        {
            if (username != null && instagrams.ContainsKey(userId) && instagrams[userId].ContainsKey(username))
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

            var api = InstaApiBuilder.CreateBuilder().SetUser(user).SetRequestDelay(RequestDelay.FromSeconds(0, 1)).Build();
            var logInResult = await api.LoginAsync();

            if (!users.ContainsKey(username))
            {
                users.Add(username, user);
            }

            if (instagrams.ContainsKey(userId))
            {
                instagrams[userId][username] = api;
            }
            else
            {
                // instagrams[userId] = new Dictionary<string, IInstaApi>();
                // instagrams[userId][username] = api;
                instagrams[userId] = new Dictionary<string, IInstaApi>
                {
                    [username] = api,
                };
            }

            return api;
        }

        public UserSessionData GetByName(string username)
        {
            return users[username];
        }
    }
}
