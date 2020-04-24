namespace HashtagIT.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using HashtagIT.Data;
    using HashtagIT.Data.Models;
    using HashtagIT.Data.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class RandomizeServiceTests
    {
        private readonly RandomizeService service;
        private readonly EfDeletableEntityRepository<HashtagSet> repository;

        public RandomizeServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
            this.repository = new EfDeletableEntityRepository<HashtagSet>(new ApplicationDbContext(options.Options));
            this.service = new RandomizeService(this.repository);
        }

        [Fact]
        public async Task NormalRandomize()
        {
            var hashtagSet = new HashtagSet
            {
                Id = 1,
                Text = @"#createcommune #earth_shotz #vscobalkan #doomshots
                         #wanderlust #creartmood #mountainaddict #agameofdrones
                         #beatifuldestinations #drones #vscosofia
                         #traveling #earth #agameoftones
                         #nature #gramslayers #traveladdict
                         #vscobulgaria #droneporn #imaginativeuniverse
                         #visualcreative #earthfocus #shotzdelight
                         #mybulgaria #mountains
                         #visualambassadors #droneshots
                         #bestvisualz #igcreative_editz",
            };
            await this.repository.AddAsync(hashtagSet);
            await this.repository.SaveChangesAsync();

            var hashtags = this.repository.All().FirstOrDefault(x => x.Id == 1);
            var randomized = this.service.Randomize(1);

            Assert.NotEqual(hashtagSet.Text, randomized);
        }

        [Fact]
        public async Task EmptyString()
        {
            var hashtagSet = new HashtagSet
            {
                UserId = "user1",
                Text = string.Empty,
            };
            await this.repository.AddAsync(hashtagSet);
            await this.repository.SaveChangesAsync();

            var set = this.repository.All().FirstOrDefault(x => x.UserId == "user1");
            var randomized = this.service.Randomize(set.Id);

            Assert.Equal(hashtagSet.Text, randomized);
        }

        [Fact]
        public async Task OneHashtag()
        {
            var hashtagSet = new HashtagSet
            {
                UserId = "user2",
                Text = "#droneshots",
            };
            await this.repository.AddAsync(hashtagSet);
            await this.repository.SaveChangesAsync();

            var hashtags = this.repository.All().FirstOrDefault(x => x.UserId == "user2");
            var randomized = this.service.Randomize(1);

            Assert.Equal(hashtagSet.Text, randomized);
        }

        [Fact]
        public async Task RemoveSameTags()
        {
            var hashtagSet = new HashtagSet
            {
                Id = 1,
                Text = @"#earth_shotz #earth_shotz #vscobalkan #doomshots
                         #wanderlust #creartmood #mountainaddict #agameofdrones
                         #beatifuldestinations #drones #vscosofia
                         #traveling #earth #agameoftones
                         #nature #gramslayers #traveladdict
                         #vscobulgaria #droneporn #imaginativeuniverse
                         #visualcreative #earthfocus #shotzdelight
                         #mybulgaria #mountains
                         #visualambassadors #droneshots
                         #bestvisualz #igcreative_editz",
            };
            await this.repository.AddAsync(hashtagSet);
            await this.repository.SaveChangesAsync();

            var hashtags = this.repository.All().FirstOrDefault(x => x.Id == 1);
            var randomized = this.service.Randomize(1);
            var tags = hashtagSet.Text.Split('#').ToList().Count;
            var randomizedTags = randomized.Split('#').ToList().Count;

            Assert.Equal(tags, randomizedTags + 1);
        }

        [Fact]
        public async Task DoesntRemoveTags()
        {
            var hashtagSet = new HashtagSet
            {
                Id = 1,
                Text = @"#earth_shotz #vscobalkan #doomshots
                         #wanderlust #creartmood #mountainaddict #agameofdrones
                         #beatifuldestinations #drones #vscosofia
                         #traveling #earth #agameoftones
                         #nature #gramslayers #traveladdict
                         #vscobulgaria #droneporn #imaginativeuniverse
                         #visualcreative #earthfocus #shotzdelight
                         #mybulgaria #mountains
                         #visualambassadors #droneshots
                         #bestvisualz #igcreative_editz",
            };
            await this.repository.AddAsync(hashtagSet);
            await this.repository.SaveChangesAsync();
            var tagsCount = hashtagSet.Text.Split('#').ToList().Count;
            string randomized = string.Empty;

            for (int i = 0; i < 30; i++)
            {
                var hashtags = this.repository.All().FirstOrDefault(x => x.Id == 1);
                randomized = this.service.Randomize(1);
                hashtags.Text = randomized;
                this.repository.Update(hashtags);
                await this.repository.SaveChangesAsync();
            }

            var afterCount = randomized.Split('#').ToList().Count;

            Assert.Equal(tagsCount, afterCount);
        }
    }
}
