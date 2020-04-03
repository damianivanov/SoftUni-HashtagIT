namespace HashtagIT.Web.Controllers
{
    using System.Threading.Tasks;

    using HashtagIT.Data.Models;
    using HashtagIT.Services.Data;
    using HashtagIT.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class VotesController : ControllerBase
    {
        private readonly IVotesService votesService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;

        public VotesController(IVotesService votesService, UserManager<ApplicationUser> userManager)
        {
            this.votesService = votesService;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<VoteResponseModel>> Post([FromBody]int inputModel)
        {
            var userId = this.userManager.GetUserId(this.User);
            await this.votesService.VoteAsync(userId, inputModel);
            var votes = this.votesService.GetVotes(inputModel);
            return new VoteResponseModel { VotesCount = votes };
        }
    }
}
