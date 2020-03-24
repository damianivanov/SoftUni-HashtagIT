namespace HashtagIT.Web.ViewModels.HashtagSets
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HashtagIT.Data.Models;

    public class AllHashtagSetsByUserViewModel
    {
        public string UserName { get; set; }

        public IEnumerable<HashtagSetViewModel> HashtagSets { get; set; }
    }
}
