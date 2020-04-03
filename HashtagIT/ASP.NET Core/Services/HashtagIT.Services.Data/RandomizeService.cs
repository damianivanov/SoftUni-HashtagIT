namespace HashtagIT.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;

    public class RandomizeService : IRandomizeService
    {
        private readonly IRepository<HashtagSet> repository;

        public RandomizeService(IRepository<HashtagSet> repository)
        {
            this.repository = repository;
        }

        public string Randomize(int hashtagSetId)
        {
            var hashtagSet = this.repository.All().Where(h => h.Id == hashtagSetId).FirstOrDefault();

            var list = hashtagSet.Text
                .Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            Random rnd = new Random();
            List<string> hashtags = new List<string>();
            for (int i = list.Count; i > 0; i--)
            {
                int curr = rnd.Next(0, i);
                if (!hashtags.Any(e => e == list.ElementAt(curr)))
                {
                    hashtags.Add(list.ElementAt(curr));
                }

                list.RemoveAt(curr);
            }

            return string.Join(' ', hashtags);
        }
    }
}
