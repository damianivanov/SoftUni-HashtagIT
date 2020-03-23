namespace HashtagIT.Web.ViewModels.HashtagSets
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class HashtagSetCreateInputModel
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        public IEnumerable<CategoryDropDownViewModel> Categories { get; set; }
    }
}
