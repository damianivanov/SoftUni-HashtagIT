namespace HashtagIT.Web.ViewModels.IG
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using InstagramApiSharp.Classes.Models;

    public class NotFollowingViewModel
    {
        public NotFollowingViewModel()
        {
            this.Users = new HashSet<InstaUserShort>();
        }

        public ICollection<InstaUserShort> Users { get; set; }
    }
}
