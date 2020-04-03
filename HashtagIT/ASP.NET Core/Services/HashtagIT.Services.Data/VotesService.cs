namespace HashtagIT.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;

    public class VotesService : IVotesService
    {
        private readonly IRepository<Vote> votesRepository;

        public VotesService(IRepository<Vote> votesRepository)
        {
            this.votesRepository = votesRepository;
        }

        public int GetVotes(int hashtagSetId)
        {
            return this.votesRepository.All().Where(v => v.HashtagSetId == hashtagSetId).Count();
        }

        public async Task VoteAsync(string userId, int hashtagSetId)
        {
            var vote = this.votesRepository
                .All()
                .Where(v => v.HashtagSetId == hashtagSetId && v.UserId == userId)
                .FirstOrDefault();
            if (vote == null)
            {
                vote = new Vote()
                {
                    UserId = userId,
                    HashtagSetId = hashtagSetId,
                };

                await this.votesRepository.AddAsync(vote);
            }
            else
            {
                this.votesRepository.Delete(vote);
            }

            await this.votesRepository.SaveChangesAsync();
        }
    }
}
