namespace HashtagIT.Web.ViewModels.HashtagSets
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using HashtagIT.Data.Models;
    using HashtagIT.Services.Mapping;

    public class HashtagSetCreateInputModel : IMapFrom<HashtagSet>, IMapTo<HashtagSet>
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        public IEnumerable<CategoryDropDownViewModel> Categories { get; set; }
    }
}
