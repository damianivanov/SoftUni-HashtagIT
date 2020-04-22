namespace HashtagIT.Web.ViewModels.IG
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class SetFromWordViewModel
    {
        public SetFromWordViewModel()
        {
            this.Sets = new List<string>();
        }

        public string Word { get; set; }

        public ICollection<string> Sets { get; set; }
    }
}
