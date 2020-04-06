namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    using HashtagIT.Data.Common.Models;
    using InstagramApiSharp.Classes.Models;

    public class Post : BaseDeletableModel<int>
    {
        public string IGUserName { get; set; }

        public string HashtagSet { get; set; }

        public string PostUrl { get; set; }

        public string PostPhoto { get; set; }

        public long Likes { get; set; }

        public string Comments { get; set; }

        [NotMapped]
        public InstaMedia InstaMedia { get; set; }
    }
}
