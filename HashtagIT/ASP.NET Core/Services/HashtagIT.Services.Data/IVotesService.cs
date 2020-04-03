namespace HashtagIT.Services.Data
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IVotesService
    {
        Task VoteAsync(string userId, int hashtagSetId);

        int GetVotes(int hashtagSetId);
    }
}
