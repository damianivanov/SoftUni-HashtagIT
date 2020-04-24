namespace HashtagIT.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using HashtagIT.Data;
    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using HashtagIT.Data.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class VotesServiceTests
    {
        private readonly VotesService service;
        private readonly EfRepository<Vote> repository;

        public VotesServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
            this.repository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));
            this.service = new VotesService(this.repository);
        }

        [Fact]
        public async Task EvenVotesShouldntCount()
        {
            await this.service.VoteAsync("user", 1);
            await this.service.VoteAsync("user", 1);

            var votes = this.service.GetVotes(1);

            Assert.Equal(0, votes);
        }

        [Fact]
        public async Task OddVotesShouldCount()
        {
            await this.service.VoteAsync("user", 1);
            await this.service.VoteAsync("user", 1);
            await this.service.VoteAsync("user", 1);

            var votes = this.service.GetVotes(1);

            Assert.Equal(1, votes);
        }

        [Fact]
        public async Task OneUserVotesForTwoSets()
        {
            await this.service.VoteAsync("user", 1);
            await this.service.VoteAsync("user", 2);

            Assert.Equal(1, this.service.GetVotes(2));
            Assert.Equal(1, this.service.GetVotes(1));
        }

        [Fact]
        public async Task TwoUsersVoteForOneSet()
        {
            await this.service.VoteAsync("user", 1);
            await this.service.VoteAsync("user2", 1);

            Assert.Equal(2, this.service.GetVotes(1));
        }

        [Fact]
        public async Task TwoDifferentVotesFromTwoDifferentUsers()
        {
            await this.service.VoteAsync("user", 1);
            await this.service.VoteAsync("user1", 2);

            Assert.Equal(this.service.GetVotes(1), this.service.GetVotes(2));
        }
    }
}
