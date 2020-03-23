namespace HashtagIT.Web.ViewModels.HashtagSets
{
    using HashtagIT.Data.Models;
    using HashtagIT.Services.Mapping;

    public class CategoryDropDownViewModel : IMapFrom<Category>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}