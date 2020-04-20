namespace HashtagIT.Web.ViewModels.IG
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class IndexPageViewModel
    {
        public IndexPageViewModel()
        {
            this.Hashsets = new HashSet<HashSetInfoModel>();
        }

        public ICollection<HashSetInfoModel> Hashsets { get; set; }
    }
}
