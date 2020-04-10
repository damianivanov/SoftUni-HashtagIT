namespace HashtagIT.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using HashtagIT.Services.Mapping;

    public class HashtagSetsService : IHashtagSetsService
    {
        private readonly IDeletableEntityRepository<HashtagSet> hashtagsetsRepository;
        private readonly IDeletableEntityRepository<Hashtag> hashtagRepository;

        public HashtagSetsService(IDeletableEntityRepository<HashtagSet> hashtagsetsRepository, IDeletableEntityRepository<Hashtag> hashtagRepository)
        {
            this.hashtagsetsRepository = hashtagsetsRepository;
            this.hashtagRepository = hashtagRepository;
        }

        public async Task<int> CreateAsync(string text, string userId, int categoryId, bool isPrivate)
        {
            List<string> hashtags = text.Split(new char[] { '#', '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < hashtags.Count; i++)
            {
                var hashtag = this.hashtagRepository.All().FirstOrDefault(h => h.Name == hashtags[i]);
                if (hashtag != null)
                {
                    hashtag.PostCount++;
                    this.hashtagRepository.Update(hashtag);
                }
                else
                {
                    hashtag = new Hashtag
                    {
                        Name = hashtags[i],
                        PostCount = 1,
                    };
                    await this.hashtagRepository.AddAsync(hashtag);
                }
            }

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

        public IEnumerable<T> GetAllByUser<T>(string id, int? take = null, int skip = 0)
        {
            var hashtagSets = this.hashtagsetsRepository
                   .All()
                   .OrderByDescending(h => h.CreatedOn)
                   .Where(h => h.UserId == id)
                   .Skip(skip);
            if (take.HasValue)
            {
                hashtagSets = hashtagSets.Take(take.Value);
            }

            return hashtagSets.To<T>().ToList();
        }

        public async Task DeleteByIdAsync(int id, string userId, bool isAdmin = false)
        {
            HashtagSet post;
            if (isAdmin)
            {
                 post = this.hashtagsetsRepository.All().Where(h => h.Id == id).FirstOrDefault();
            }
            else
            {
                post = this.hashtagsetsRepository.All().Where(h => h.Id == id && h.UserId == userId).FirstOrDefault();
            }

            this.hashtagsetsRepository.Delete(post);
            await this.hashtagsetsRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetPublic<T>(int? take = null, int skip = 0)
        {
            var hashtagSets = this.hashtagsetsRepository
                .All()
                .Where(h => !h.IsPrivate)
                .OrderByDescending(h => h.CreatedOn)
                .Skip(skip);
            if (take.HasValue)
            {
                hashtagSets = hashtagSets.Take(take.Value);
            }

            return hashtagSets.To<T>().ToList();
        }

        public int GetCountPublic()
        {
            return this.hashtagsetsRepository.All().Where(h => !h.IsPrivate).ToList().Count;
        }

        public int GetCountPrivate(string userId)
        {
            return this.hashtagsetsRepository.All().Where(h => h.UserId == userId).ToList().Count;
        }
    }
}
