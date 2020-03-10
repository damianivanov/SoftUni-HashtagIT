namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HashtagIT.Data.Common.Models;

    public class Category : BaseDeletableModel<int>
    {
        public Category()
        {
            this.HashtagSets = new HashSet<HashtagSet>();
        }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<HashtagSet> HashtagSets { get; set; }
    }
}
