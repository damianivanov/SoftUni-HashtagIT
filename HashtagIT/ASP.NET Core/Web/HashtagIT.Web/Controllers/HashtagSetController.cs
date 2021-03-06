﻿namespace HashtagIT.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Common;
    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using HashtagIT.Services.Data;
    using HashtagIT.Web.ViewModels;
    using HashtagIT.Web.ViewModels.HashtagSets;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class HashtagSetController : Controller
    {
        private const int SetsPerPage = 5;
        private readonly IHashtagSetsService hashtagSetsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICategoriesService categoriesService;

        public HashtagSetController(IHashtagSetsService hashtagSetsService, UserManager<ApplicationUser> userManager, ICategoriesService categoriesService)
        {
            this.hashtagSetsService = hashtagSetsService;
            this.userManager = userManager;
            this.categoriesService = categoriesService;
        }

        public IActionResult Index()
        {
            return this.RedirectToAction(nameof(this.MySets));
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
            var userId = this.userManager.GetUserId(this.User);
            if (this.hashtagSetsService.IsOwner(id, userId) || this.User.IsInRole("Moderator"))
            {
                var hashtagViewModel = this.hashtagSetsService.GetById<HashtagSetViewModel>(id);
                if (hashtagViewModel != null)
                {
                    return this.View(hashtagViewModel);
                }
            }

            return this.View("Error");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = this.userManager.GetUserId(this.User);
            if (this.hashtagSetsService.IsOwner(id, userId) || this.User.IsInRole("Moderator"))
            {
                await this.hashtagSetsService.DeleteByIdAsync(id, userId);
            }

            return this.RedirectToAction("MySets");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return this.View("Error");
            }

            var userId = this.userManager.GetUserId(this.User);
            var viewModel = this.hashtagSetsService.GetById<HashtagSetCreateInputModel>(id.Value);
            viewModel.Categories = this.categoriesService.GetAll<CategoryDropDownViewModel>();
            if (id.HasValue && (this.hashtagSetsService.IsOwner(id.Value, userId) || this.User.IsInRole("Moderator")))
            {
                return this.View(viewModel);
            }

            return this.View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HashtagSetCreateInputModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                await this.hashtagSetsService.EditAsync(viewModel.Text, viewModel.CategoryId, viewModel.IsPrivate, viewModel.Id);
                return this.RedirectToAction(nameof(this.ById), new { id = viewModel.Id });
            }

            viewModel.Categories = this.categoriesService.GetAll<CategoryDropDownViewModel>();
            return this.View(viewModel);
        }

        public async Task<IActionResult> MySets(int page = 1)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var hashtagSets = this.hashtagSetsService.GetAllByUser<HashtagSetViewModel>(user.Id, SetsPerPage, (page - 1) * SetsPerPage);
            var allPrivatePosts = this.hashtagSetsService.GetCountPrivate(user.Id);
            var viewModel = new AllHashtagSetsByUserViewModel
            {
                PagesCount = (int)Math.Ceiling((double)allPrivatePosts / SetsPerPage),
                HashtagSets = hashtagSets,
                UserName = user.UserName,
            };
            if (viewModel.PagesCount == 0)
            {
                viewModel.PagesCount = 1;
            }

            viewModel.CurrentPage = page;

            return this.View(viewModel);
        }

        public IActionResult Public(int page = 1)
        {
            var hashtagSets = this.hashtagSetsService.GetPublic<HashtagSetViewModel>(SetsPerPage, (page - 1) * SetsPerPage);
            var allPublicPosts = this.hashtagSetsService.GetCountPublic();
            var viewModel = new AllPublicHashtagSetsViewModel
            {
                PagesCount = (int)Math.Ceiling((double)allPublicPosts / SetsPerPage),
                HashtagSets = hashtagSets,
            };
            if (viewModel.PagesCount == 0)
            {
                viewModel.PagesCount = 1;
            }

            viewModel.CurrentPage = page;

            return this.View(viewModel);
        }
    }
}
