namespace HashtagIT.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using HashtagIT.Services.Mapping;

    public class HashtagSetsService : IHashtagSetsService
    {
        private readonly IDeletableEntityRepository<HashtagSet> hashtagsetsRepository;

        public HashtagSetsService(IDeletableEntityRepository<HashtagSet> hashtagsetsRepository)
        {
            this.hashtagsetsRepository = hashtagsetsRepository;
        }

        public async Task<int> CreateAsync(string text, string userId, int categoryId, bool isPrivate)
        {
            var hashtagSet = new HashtagSet
            {
                Text = text,
                IsPrivate = isPrivate,
                UserId = userId,
                CategoryId = categoryId,
            };
            await this.hashtagsetsRepository.AddAsync(hashtagSet);
            await this.hashtagsetsRepository.SaveChangesAsync();
            return hashtagSet.Id;
        }

        public T GetById<T>(int id)
        {
            return this.hashtagsetsRepository.All()
                    .Where(h => h.Id == id)
                    .To<T>()
                    .FirstOrDefault();

        }
    }
}
