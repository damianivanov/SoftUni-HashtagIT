namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    using HashtagIT.Data.Common.Models;

    public class Hashtag : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public long PostCount { get; set; }

        [NotMapped]
        public string Tag => "#" + this.Name;
    }
}
