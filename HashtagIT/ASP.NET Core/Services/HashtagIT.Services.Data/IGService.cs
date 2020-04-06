namespace HashtagIT.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using HashtagIT.Web.ViewModels.IG;
    using InstagramApiSharp;
    using InstagramApiSharp.API;
    using InstagramApiSharp.API.Builder;
    using InstagramApiSharp.Classes;
    using InstagramApiSharp.Classes.Models;

    public class IGService : IIGService
    {
        private readonly IDeletableEntityRepository<IGUser> igUsersRepository;
        private readonly IDeletableEntityRepository<Post> postRepository;
        private readonly API api;
        private IInstaApi instaApi;

        public IGService(IDeletableEntityRepository<IGUser> igUsersRepository, API api, IDeletableEntityRepository<Post> postRepository)
        {
            this.postRepository = postRepository;
            this.igUsersRepository = igUsersRepository;
            this.api = api;
        }

        public async Task<string> Login(string userId, string username, string password)
        {
            this.instaApi = await this.api.Login(userId, username, password);

            // challange needed (Two Factor Authentication)
            if (!this.instaApi.IsUserAuthenticated)
            {
                var challenge = await this.instaApi.GetChallengeRequireVerifyMethodAsync();
                return challenge.Value.StepData.PhoneNumber;
            }

            // succsefull login
            var user = this.api.GetByName(username);
            await this.Add(userId, user);
            return string.Empty;
        }

        public async Task<TopNineViewModel> TopNine(string userId, string username)
        {
            this.instaApi = this.api.GetInstance(userId, username);
            var user = await this.instaApi.UserProcessor.GetUserInfoByUsernameAsync(username);
            var allMedia = await this.instaApi.UserProcessor.GetUserMediaAsync(username, PaginationParameters.MaxPagesToLoad(5));
            var mediaFromThisYear = allMedia.Value
                .Where(m => m.TakenAt.Year == DateTime.Now.Year)
                .OrderByDescending(m => m.LikesCount)
                .ToList();
            var likesTotal = mediaFromThisYear.Sum(l => l.LikesCount);
            var topNine = mediaFromThisYear.Take(9).ToList();
            List<Post> posts = new List<Post>();

            // InstaMedia Post to Model Post
            foreach (var post in topNine)
            {
                var postModel = new Post
                {
                    IGUserName = username,
                    InstaMedia = post,
                    PostPhoto = post.Images[1].Uri,
                    Likes = post.LikesCount,
                    Comments = post.CommentsCount,
                    HashtagSet = await this.FindHashtagSet(post),
                    PostUrl = "https://www.instagram.com/p/" + post.Code,
                };

                if (!this.postRepository.All().Any(p => p.PostUrl == postModel.PostUrl))
                {
                    await this.postRepository.AddAsync(postModel);
                    await this.postRepository.SaveChangesAsync();
                }

                posts.Add(postModel);
            }

            var viewModel = new TopNineViewModel
            {
                IsPrivate = user.Value.IsPrivate,
                ProfilePicUrl = user.Value.ProfilePicUrl,
                IGFullName = user.Value.FullName,
                AllPosts = user.Value.MediaCount,
                Followers = user.Value.FollowerCount,
                Following = user.Value.FollowingCount,
                Caption = user.Value.Biography,
                IGUserName = user.Value.Username,
                LikesTotal = likesTotal,
                ThisYearsPostsCount = mediaFromThisYear.Count,
                LikesAvrg = likesTotal / mediaFromThisYear.Count,
                Posts = posts,
            };

            return viewModel;
        }

        public async Task<bool> TwoFactor(string username, string password, string code, string userId)
        {
            this.instaApi = this.api.GetInstance(userId, username);
            await this.instaApi.VerifyCodeForChallengeRequireAsync(code);
            if (this.instaApi.IsUserAuthenticated)
            {
                var user = this.api.GetByName(username);
                await this.Add(userId, user);
                return true;
            }

            return false;
        }

        private async Task<string> Add(string userId, UserSessionData user)
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
                existingUser.UserId = igUser.UserId;
                this.igUsersRepository.Update(existingUser);
            }
            else
            {
                await this.igUsersRepository.AddAsync(igUser);
            }

            await this.igUsersRepository.SaveChangesAsync();

            return igUser.Id;
        }

        private async Task<string> FindHashtagSet(InstaMedia media)
        {
            StringBuilder sb = new StringBuilder();
            if (media.Caption.Text.Contains("#"))
            {
                var matches = Regex.Matches(media.Caption.Text, @"#+\w+");
                foreach (var match in matches)
                {
                    sb.Append(match.ToString() + " ");
                }
            }

            var comments = await this.instaApi.CommentProcessor.GetMediaCommentsAsync(media.InstaIdentifier, PaginationParameters.Empty);

            foreach (var comment in comments.Value.Comments)
            {
                var matches = Regex.Matches(comment.Text, @"#+\w+");
                foreach (var match in matches)
                {
                    sb.Append(match.ToString() + " ");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}
