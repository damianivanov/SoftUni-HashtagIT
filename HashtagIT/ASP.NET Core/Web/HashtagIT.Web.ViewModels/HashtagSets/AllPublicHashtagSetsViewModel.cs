namespace HashtagIT.Web.ViewModels.HashtagSets
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HashtagIT.Data.Models;
    using HashtagIT.Services.Mapping;

    public class AllPublicHashtagSetsViewModel : IMapFrom<HashtagSet>, IMapTo<HashtagSet>
    {
        public IEnumerable<HashtagSetViewModel> HashtagSets { get; set; }
    }
}
