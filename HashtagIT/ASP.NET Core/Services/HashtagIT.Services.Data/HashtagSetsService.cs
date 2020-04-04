﻿namespace HashtagIT.Services.Data
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
