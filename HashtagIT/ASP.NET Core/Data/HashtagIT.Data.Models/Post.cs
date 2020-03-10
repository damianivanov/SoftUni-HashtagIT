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
        public int HashtagSetId { get; set; }

        public HashtagSet HashtagSet { get; set; }

        public string PostUrl { get; set; }

        [NotMapped]
        public InstaMedia InstaMedia { get; set; }
    }
}
