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
    using HashtagIT.Services.Mapping;
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
            if (username == "system")
            {
                this.instaApi = this.api.GetInstance("system");
                var guest_username = this.instaApi.GetLoggedUser().UserName;
                var guest = this.api.GetByName(guest_username);
                this.instaApi = await this.api.Login(userId, guest.UserName, guest.Password);
                await this.Add(userId, guest);
                return string.Empty;
            }

            this.instaApi = await this.api.Login(userId, username, password);

            // challange needed (Two Factor Authentication)
            if (!this.instaApi.IsUserAuthenticated)
            {
                var challenge = await this.instaApi.GetChallengeRequireVerifyMethodAsync();
                await this.instaApi.SendTwoFactorLoginSMSAsync();
                return challenge.Value.StepData.PhoneNumber;
            }

            // succsefull login
            var user = this.api.GetByName(username);
            await this.Add(userId, user);
            return string.Empty;
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

        // TODO Refactor
        public async Task<TopNineViewModel> TopNine(string userId, string username)
        {
            this.instaApi = this.api.GetInstance(userId, username);
            var user = await this.instaApi.UserProcessor.GetUserInfoByUsernameAsync(username);
            var allMedia = await this.instaApi.UserProcessor.GetUserMediaAsync(username, PaginationParameters.Empty);
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
                string photoUrl;

                if (post.Carousel != null)
                {
                     photoUrl = post.Carousel[0].Images[1].Uri;
                }
                else if (post.Videos.Count != 0)
                {
                     photoUrl = post.Videos[0].Uri;
                }
                else
                {
                    photoUrl = post.Images[1].Uri;
                }

                var postModel = new Post
                {
                    IGUserName = username,
                    InstaMedia = post,
                    PostPhoto = photoUrl,
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

            double likesAvrg;
            if (likesTotal != 0 && mediaFromThisYear.Count != 0)
            {
                likesAvrg = mediaFromThisYear.Average(x => x.LikesCount);
            }
            else
            {
                likesAvrg = 0;
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
                LikesAvrg = likesAvrg,
                Posts = posts,
            };

            return viewModel;
        }

        public async Task<NotFollowingViewModel> NotFollowinBack(string userId, string username)
        {
            this.instaApi = this.api.GetInstance(userId, username);

            var followers = await this.instaApi.UserProcessor.GetUserFollowersAsync(username, PaginationParameters.Empty);
            var following = await this.instaApi.UserProcessor.GetUserFollowingAsync(username, PaginationParameters.Empty);

            var notFollowing = new List<InstaUserShort>();

            foreach (var user in following.Value)
            {
                if (!followers.Value.Contains(user))
                {
                    notFollowing.Add(user);
                }
            }

            var viewModel = new NotFollowingViewModel
            {
                Users = notFollowing,
            };

            return viewModel;
        }

        public async Task<FriendshipViewModel> Friendship(string userId, string username)
        {
            this.instaApi = this.api.GetInstance(userId);

            var profile = await this.instaApi.UserProcessor.GetUserInfoByUsernameAsync(username);

            var status = await this.instaApi.UserProcessor.GetFriendshipStatusAsync(profile.Value.Pk);

            FriendshipViewModel model = new FriendshipViewModel
            {
                IGUserName = username,
                IGFullName = profile.Value.FullName,
                ProfilePicUrl = profile.Value.ProfilePicUrl,
                Friends = status.Value.FollowedBy && status.Value.Following,
                Caption = profile.Value.Biography,
            };
            return model;
        }

        public IndexPageViewModel TopHashtags()
        {
            var hashtagSets = this.postRepository
                .All()
                .OrderByDescending(x => x.Likes)
                .Select(x => new { x.Likes, x.HashtagSet, x.IGUserName })
                .Take(5)
                .ToList();
            HashSet<HashSetInfoModel> hashSets = new HashSet<HashSetInfoModel>();
            foreach (var item in hashtagSets)
            {
                hashSets.Add(new HashSetInfoModel
                {
                    Text = item.HashtagSet,
                    TotalLikes = item.Likes,
                    Username = item.IGUserName,
                });
            }

            IndexPageViewModel model = new IndexPageViewModel
            {
                Hashsets = hashSets,
            };

            return model;
        }

        public async Task<SetFromWordViewModel> GenerateSet(string userId, string word)
        {
            this.instaApi = this.api.GetInstance(userId);
            var hashtag = Regex.Match(word, @"#\w+[_]\w+$|#\w+$|\w+$|\w+[_]+\w$");
            var sets = await this.Hashtags(hashtag.Name);
            var model = new SetFromWordViewModel
            {
                Sets = sets,
                Word = word,
            };

            return model;
        }

        private async Task<string> Add(string userId, UserSessionData user)
        {
            var followersCount = await this.instaApi.UserProcessor.GetUserFollowersAsync(user.UserName, PaginationParameters.Empty);
            var followingCount = await this.instaApi.UserProcessor.GetUserFollowingAsync(user.UserName, PaginationParameters.Empty);

            long followers = 0;
            long following = 0;

            if (followersCount.Value != null)
            {
                followers = followersCount.Value.Count;
            }

            if (followingCount.Value != null)
            {
                following = followingCount.Value.Count;
            }

            IGUser igUser = new IGUser
            {
                UserId = userId,
                IGFullname = user.LoggedInUser.FullName,
                IGProfileId = user.LoggedInUser.Pk,
                IGUserName = user.LoggedInUser.UserName,
                ProfilePicUrl = user.LoggedInUser.ProfilePicUrl,
                FollowersCount = followers,
                FollowingCount = following,
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

            var comments = await this.instaApi.CommentProcessor.GetMediaCommentsAsync(media.InstaIdentifier, PaginationParameters.MaxPagesToLoad(1));

            foreach (var comment in comments.Value.Comments)
            {
                try
                {
                    var matches = Regex.Matches(comment.Text, @"#+\w+");
                    foreach (var match in matches)
                    {
                        sb.Append(match.ToString() + " ");
                    }

                    if (matches.Count >= 10)
                    {
                        return sb.ToString().TrimEnd();
                    }
                }
                catch (Exception)
                {
                }
            }

            return sb.ToString().TrimEnd();
        }

        private async Task<ICollection<string>> Hashtags(string hash)
        {
            var hashtagSetsFromPosts = await this.HashtagsFromHashtagsTopPosts(hash);
            List<string> hashtags = new List<string>();
            foreach (var set in hashtagSetsFromPosts)
            {
                List<string> tags = set.Split(' ').ToList();
                foreach (var tag in tags)
                {
                    hashtags.Add(tag);
                }

                hashtags.AddRange(tags.ToList());
            }

            var randomized = this.Random(hashtags);
            var relatedhashtags = await this.RelatedHashtags(hash);
            hashtags.AddRange(relatedhashtags);
            List<string> result = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                var tmp = randomized.Distinct().Skip(20 * i).Take(20).ToList();
                tmp.AddRange(relatedhashtags);
                tmp = this.Random(tmp).ToList();
                result.Add(string.Join(' ', tmp));
            }

            return result;
        }

        private async Task<ICollection<string>> HashtagsFromHashtagsTopPosts(string hash)
        {
            var topPosts = await this.instaApi.HashtagProcessor.GetTopHashtagMediaListAsync(hash, PaginationParameters.MaxPagesToLoad(1));
            var mediaList = topPosts.Value.Medias;
            HashSet<string> hashtags = new HashSet<string>();
            foreach (var media in mediaList.Take(5).ToList())
            {
                var hashtagSet = await this.FindHashtagSet(media);
                hashtags.Add(hashtagSet);
            }

            return hashtags.Distinct().ToList();
        }

        private async Task<ICollection<string>> RelatedHashtags(string hash)
        {
            var medialist = await this.instaApi.HashtagProcessor.GetTopHashtagMediaListAsync(hash, PaginationParameters.MaxPagesToLoad(1));
            var listOfHashtags = medialist.Value.RelatedHashtags.Select(x => "#" + x.Name).ToList();
            return listOfHashtags;
        }

        private ICollection<string> Random(List<string> tags)
        {
            Random rnd = new Random();
            List<string> randomized = new List<string>();
            for (int i = tags.Count; i > 0; i--)
            {
                int curr = rnd.Next(0, i);
                if (!randomized.Any(e => e == tags.ElementAt(curr)))
                {
                    randomized.Add(tags.ElementAt(curr));
                }

                tags.RemoveAt(curr);
            }

            return randomized;
        }
    }
}
