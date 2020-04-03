namespace HashtagIT.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Services.Data;
    using HashtagIT.Web.ViewModels.Randomize;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class RandomizeController : ControllerBase
    {
        private readonly IRandomizeService randomizeService;

        public RandomizeController(IRandomizeService randomizeService)
        {
            this.randomizeService = randomizeService;
        }

        [Authorize]
        [HttpPost]
        public ActionResult<RandomizeResponseModel> Post([FromBody]int hashtagSetId)
        {
            var result = this.randomizeService.Randomize(hashtagSetId);
            return new RandomizeResponseModel { Text = result };
        }
    }
}
