namespace HashtagIT.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using HashtagIT.Services.Mapping;

    public class CategoriesService : ICategoriesService
    {
        private readonly IDeletableEntityRepository<Category> categoriesRepository;

        public CategoriesService(IDeletableEntityRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public async Task<int> AddCategory(string name)
        {
            if (!this.categoriesRepository.All().Any(x => x.Name == name))
            {
                var category = new Category
                {
                    Name = name,
                };
                await this.categoriesRepository.AddAsync(category);
                await this.categoriesRepository.SaveChangesAsync();
                return category.Id;
            }

            return this.categoriesRepository.All().FirstOrDefault(x => x.Name == name).Id;
        }

        public IEnumerable<T> GetAll<T>(int? count = null)
        {
            IQueryable<Category> query = this.categoriesRepository.All().OrderBy(x => x.Name);
            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.To<T>().ToList();
        }

        public Category GetById(int categoryId)
        {
            var category = this.categoriesRepository.All().Where(c => c.Id == categoryId).FirstOrDefault();
            return category;
        }

        public T GetByName<T>(string name)
        {
            var category = this.categoriesRepository.All().Where(x => x.Name == name)
                .To<T>().FirstOrDefault();
            return category;
        }
    }
}
