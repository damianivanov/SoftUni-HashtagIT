namespace HashtagIT.Services.Data
{
    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using InstagramApiSharp;
    using System.Threading.Tasks;

    public class IGService : IIGService
    {
        private readonly IDeletableEntityRepository<IGUser> igUsersRepository;

        public IGService(IDeletableEntityRepository<IGUser> igUsersRepository)
        {
            this.igUsersRepository = igUsersRepository;
        }

        public Task<string> Login(string userId, string username, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}
