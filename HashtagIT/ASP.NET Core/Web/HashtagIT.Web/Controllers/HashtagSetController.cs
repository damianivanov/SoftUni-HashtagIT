namespace HashtagIT.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using HashtagIT.Services.Data;
    using HashtagIT.Web.ViewModels.HashtagSets;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class HashtagSetController : Controller
    {
        private readonly IHashtagSetsService hashtagSetsService;
        private readonly UserManager<ApplicationUser> userManager;

        public HashtagSetController(IHashtagSetsService hashtagSetsService, UserManager<ApplicationUser> userManager)
        {
            this.hashtagSetsService = hashtagSetsService;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Create()
        {
            return this.View();
        }

        public IActionResult ById(int id)
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HashtagSetCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }   

            var user = await this.userManager.GetUserAsync(this.User);
            var hashtagSetId = await this.hashtagSetsService.CreateAsync(inputModel.Text, user.Id, inputModel.CategoryName, inputModel.IsPrivate);
            return this.RedirectToAction("ById", new { id = hashtagSetId });
        }
    }
}
