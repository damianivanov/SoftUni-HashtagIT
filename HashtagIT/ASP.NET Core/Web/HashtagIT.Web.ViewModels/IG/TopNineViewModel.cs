namespace HashtagIT.Web.ViewModels.IG
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HashtagIT.Data.Models;

    public class TopNineViewModel
    {
        public TopNineViewModel()
        {
            this.Posts = new HashSet<Post>();
        }

        public bool IsPrivate { get; set; }

        public string Caption { get; set; }

        public string ProfilePicUrl { get; set; }

        public string IGFullName { get; set; }

        public long AllPosts { get; set; }

        public long Followers { get; set; }

        public long Following { get; set; }

        public string IGUserName { get; set; }

        public double LikesAvrg { get; set; }

        public double LikesTotal { get; set; }

        public int ThisYearsPostsCount { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
