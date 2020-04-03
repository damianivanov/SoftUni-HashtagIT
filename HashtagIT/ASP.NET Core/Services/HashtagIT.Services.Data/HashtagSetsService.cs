namespace HashtagIT.Services.Data
{
    using System.Collections.Generic;
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

        public IEnumerable<T> GetAll<T>(string id)
        {
            return this.hashtagsetsRepository.All()
                   .Where(h => h.UserId == id).To<T>().ToList();
        }

        public async Task DeleteById(int id, string userId)
        {
            var toRemove = this.hashtagsetsRepository.All().Where(h => h.Id == id && userId == h.UserId).FirstOrDefault();
            this.hashtagsetsRepository.Delete(toRemove);
            await this.hashtagsetsRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetPublic<T>()
        {
            var hashtagSets = this.hashtagsetsRepository
                .All()
                .Where(h => h.IsPrivate == false)
                .OrderByDescending(h => h.CreatedOn)
                .To<T>()
                .ToList();
            return hashtagSets;
        }
    }
}
