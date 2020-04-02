namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    using HashtagIT.Data.Common.Models;

    public class HashtagSet : BaseDeletableModel<int>
    {
        public HashtagSet()
        {
            this.Hashtags = new HashSet<Hashtag>();
            this.Insights = new Dictionary<string, int>();
            this.Votes = new HashSet<Vote>();
        }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public bool IsPrivate { get; set; }

        [Required]
        public string Text { get; set; }

        public virtual ICollection<Hashtag> Hashtags { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }

        [NotMapped]
        public virtual IDictionary<string, int> Insights { get; set; }
    }
}
