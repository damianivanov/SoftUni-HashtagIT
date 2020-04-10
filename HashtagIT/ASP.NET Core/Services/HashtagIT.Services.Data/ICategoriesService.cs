namespace HashtagIT.Services.Data
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using HashtagIT.Data.Models;

    public interface ICategoriesService
    {
        IEnumerable<T> GetAll<T>(int? count = null);

        T GetByName<T>(string name);

        Category GetById(int categoryId);

        Task<int> AddCategory(string name);
    }
}
