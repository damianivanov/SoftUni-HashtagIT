namespace HashtagIT.Services.Data
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using HashtagIT.Data.Models;

    public interface IHashtagSetsService
    {
        Task<int> CreateAsync(string text, string userId, int categoryId, bool isPrivate);

        T GetById<T>(int id);

        IEnumerable<T> GetAll<T>(string id);

        Task DeleteById(int id, string userId);

        IEnumerable<T> GetPublic<T>(int? take = null, int skip = 0);

        int GetCountPublic();
    }
}
