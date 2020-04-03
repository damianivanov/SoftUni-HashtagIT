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

    [Authorize]
    public class HashtagSetController : Controller
    {
        private readonly IHashtagSetsService hashtagSetsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICategoriesService categoriesService;

        public HashtagSetController(IHashtagSetsService hashtagSetsService, UserManager<ApplicationUser> userManager, ICategoriesService categoriesService)
        {
            this.hashtagSetsService = hashtagSetsService;
            this.userManager = userManager;
            this.categoriesService = categoriesService;
        }

        public IActionResult Create()
        {
            var categories = this.categoriesService.GetAll<CategoryDropDownViewModel>();
            var viewModel = new HashtagSetCreateInputModel
            {
                Categories = categories,
            };
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HashtagSetCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            await this.hashtagSetsService.CreateAsync(inputModel.Text, user.Id, inputModel.CategoryId, inputModel.IsPrivate);
            return this.RedirectToAction("MySets");
        }

        public IActionResult ById(int id)
        {
            var hashtagViewModel = this.hashtagSetsService.GetById<HashtagSetViewModel>(id);
            return this.View(hashtagViewModel);
        }

        public async Task<IActionResult> MySets()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var hashtagSets = this.hashtagSetsService.GetAll<HashtagSetViewModel>(user.Id);
            var viewModel = new AllHashtagSetsByUserViewModel
            {
                HashtagSets = hashtagSets.OrderByDescending(h => h.CreatedOn).ToList(),
                UserName = user.UserName,
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            await this.hashtagSetsService.DeleteById(id, user.Id);
            return this.RedirectToAction("MySets");
        }

        public IActionResult Public()
        {
            var hashtagSets = this.hashtagSetsService.GetPublic<HashtagSetViewModel>();
            var viewModel = new AllPublicHashtagSetsViewModel
            {
                HashtagSets = hashtagSets,
            };
            return this.View(viewModel);
        }
    }
}
