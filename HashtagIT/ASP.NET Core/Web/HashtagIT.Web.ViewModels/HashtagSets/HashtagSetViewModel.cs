namespace HashtagIT.Web.ViewModels.HashtagSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using HashtagIT.Data.Models;
    using HashtagIT.Services.Mapping;

    public class HashtagSetViewModel : IMapFrom<HashtagSet>, IMapTo<HashtagSet>
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Text { get; set; }

        public bool IsPrivate { get; set; }

        public string UserUserName { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public int HashtagCount => this.Text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList().Count();

        public int VotesCount { get; set; }

    }
}
