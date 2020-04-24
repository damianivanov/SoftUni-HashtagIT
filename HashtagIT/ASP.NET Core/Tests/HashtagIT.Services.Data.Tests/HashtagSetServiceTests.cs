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

    public class HashtagSetServiceTests
    {
        private readonly HashtagSetsService service;
        private readonly EfDeletableEntityRepository<HashtagSet> repository;
        private readonly EfDeletableEntityRepository<Hashtag> hashtagrepository;

        public HashtagSetServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString());
            this.repository = new EfDeletableEntityRepository<HashtagSet>(new ApplicationDbContext(options.Options));
            this.hashtagrepository = new EfDeletableEntityRepository<Hashtag>(new ApplicationDbContext(options.Options));
            this.service = new HashtagSetsService(this.repository, this.hashtagrepository);
        }

        [Fact]
        public async Task ProperCreatingHashtagSet()
        {
            var hashtagSet = new HashtagSet
            {
                UserId = "user",
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
                CategoryId = 1,
                IsPrivate = true,
            };
            var id = await this.service.CreateAsync(hashtagSet.Text, hashtagSet.UserId, hashtagSet.CategoryId, hashtagSet.IsPrivate);
            await this.repository.SaveChangesAsync();
            var set = await this.repository.All().FirstOrDefaultAsync(x => x.UserId == "user");

            Assert.Equal(id, set.Id);
        }

        [Fact]
        public async Task Editing()
        {
            var hashtagSet = new HashtagSet
            {
                UserId = "user",
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
                CategoryId = 1,
                IsPrivate = true,
            };
            var id = await this.service.CreateAsync(hashtagSet.Text, hashtagSet.UserId, hashtagSet.CategoryId, hashtagSet.IsPrivate);
            await this.repository.SaveChangesAsync();

            string text = @" #traveling #earth #agameoftones
                         #nature #gramslayers #traveladdict
                         #vscobulgaria #droneporn #imaginativeuniverse
                         #visualcreative #earthfocus #shotzdelight
                         #mybulgaria #mountains
                         #visualambassadors #droneshots
                         #bestvisualz #igcreative_editz";

            await this.service.EditAsync(text, 2, false, id);
            var set = await this.repository.All().FirstOrDefaultAsync(x => x.UserId == "user");

            Assert.Equal(text, set.Text);
            Assert.Equal(2, set.CategoryId);
            Assert.False(set.IsPrivate);
        }

        [Fact]
        public async Task EditingNotExistingHashtagSet()
        {
            var id = await this.service.EditAsync(string.Empty, 0, false, 2);
            Assert.Equal(0, id);
        }

        [Fact]
        public async Task DeleteById()
        {
            var hashtagSet = new HashtagSet
            {
                UserId = "user",
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
                CategoryId = 1,
                IsPrivate = true,
            };
            var id = await this.service.CreateAsync(hashtagSet.Text, hashtagSet.UserId, hashtagSet.CategoryId, hashtagSet.IsPrivate);
            await this.repository.SaveChangesAsync();

            await this.service.DeleteByIdAsync(id, hashtagSet.UserId);
            await this.repository.SaveChangesAsync();

            var set = await this.repository.All().FirstOrDefaultAsync(x => x.Id == hashtagSet.Id && x.UserId == hashtagSet.UserId);

            Assert.Null(set);
        }

        [Fact]
        public async Task GetPublic()
        {
            await this.service.CreateAsync(string.Empty, "user1", 1, true);
            await this.service.CreateAsync(string.Empty, "user2", 4, true);
            await this.service.CreateAsync(string.Empty, "user2", 2, false);
            await this.service.CreateAsync(string.Empty, "user3", 3, false);

            await this.repository.SaveChangesAsync();

            Assert.Equal(2, this.service.GetCountPublic());
            Assert.Equal(1, this.service.GetCountPrivate("user1"));
            Assert.Equal(2, this.service.GetCountPrivate("user2"));
        }

        [Fact]
        public async Task IsOwner()
        {
            await this.service.CreateAsync(string.Empty, "user1", 1, true);
            await this.service.CreateAsync(string.Empty, "user2", 1, true);
            await this.service.CreateAsync(string.Empty, "user1", 1, true);

            await this.repository.SaveChangesAsync();

            Assert.True(this.service.IsOwner(1, "user1"));
            Assert.True(this.service.IsOwner(3, "user1"));
            Assert.True(this.service.IsOwner(2, "user2"));
            Assert.False(this.service.IsOwner(1, "user2"));
        }
    }
}
