namespace HashtagIT.Services.Data
{
    using System.Threading.Tasks;

    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;

    public class HashtagSetsService : IHashtagSetsService
    {
        private readonly IDeletableEntityRepository<HashtagSet> hashtagsetsRepository;

        public HashtagSetsService(IDeletableEntityRepository<HashtagSet> hashtagsetsRepository)
        {
            this.hashtagsetsRepository = hashtagsetsRepository;
        }

        public async Task<int> CreateAsync(string text, string userId, string categoryName, bool isPrivate)
        {
            var hashtagSet = new HashtagSet
            {
                Text = text,
                IsPrivate = isPrivate,
                CategoryName = categoryName,
                UserId = userId,
            };
            await this.hashtagsetsRepository.AddAsync(hashtagSet);
            await this.hashtagsetsRepository.SaveChangesAsync();
            return hashtagSet.Id;
        }
    }
}
