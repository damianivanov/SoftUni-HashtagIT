namespace HashtagIT.Services.Data
{
    using System.Threading.Tasks;

    public interface IHashtagSetsService
    {
        Task<int> CreateAsync(string text, string userId, int categoryId, bool isPrivate);

        T GetById<T>(int id);
    }
}
