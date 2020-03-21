namespace HashtagIT.Web.ViewModels.HashtagSets
{
    using System.ComponentModel.DataAnnotations;

    public class HashtagSetCreateInputModel
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string CategoryName { get; set; }

        [Required]
        public bool IsPrivate { get; set; }
    }
}
