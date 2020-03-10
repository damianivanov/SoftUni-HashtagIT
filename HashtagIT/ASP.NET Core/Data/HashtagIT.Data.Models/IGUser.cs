namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HashtagIT.Data.Common.Models;
    using InstagramApiSharp.Classes.Models;

    public class IGUser : BaseDeletableModel<string>
    {
        public IGUser()
        {
            this.Posts = new HashSet<Post>();
        }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string IGFullname { get; set; }

        public string IGUserName { get; set; }

        public long IGProfileId { get; set; }

        public string ProfilePicUrl { get; set; }

        public long FollowersCount { get; set; }

        public long FollowingCount { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
