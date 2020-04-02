namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using HashtagIT.Data.Common.Models;

    public class Vote : BaseModel<int>
    {
        [Required]
        public int HashtagSetId { get; set; }

        public virtual HashtagSet HashtagSet { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
