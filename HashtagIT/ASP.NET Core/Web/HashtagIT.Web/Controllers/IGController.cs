namespace HashtagIT.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HashtagIT.Data.Models;
    using HashtagIT.Services.Data;
    using HashtagIT.Web.ViewModels.IG;
    using InstagramApiSharp.API;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class IGController : Controller
    {
        private readonly IIGService iGService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly API api;

        public IGController(IIGService iGService, UserManager<ApplicationUser> userManager, API api)
        {
            this.iGService = iGService;
            this.userManager = userManager;
            this.api = api;
        }

        [Authorize]
        public IActionResult IGIndex()
        {
            return this.View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult TwoFactor(LoginViewModel loginView, bool post = false)
        {
            return this.View(loginView);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactor(LoginViewModel viewModel)
        {
            var result = await this.iGService.TwoFactor(viewModel.IGUserName, viewModel.Password, viewModel.Code, viewModel.UserId);

            if (!result)
            {
                this.RedirectToAction("Login");
            }

            return this.RedirectToAction("IGIndex");
        }

        [Authorize]
        public IActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            var phone = await this.iGService.Login(user.Id, viewModel.IGUserName, viewModel.Password);
            viewModel.Phone = phone;
            string encryptedPassword = Cipher.Encrypt(viewModel.Password, viewModel.IGUserName);
            viewModel.Password = encryptedPassword;
            viewModel.UserId = user.Id;
            if (phone != string.Empty)
            {
                return this.RedirectToAction("TwoFactor", viewModel);
            }

            return this.View("IGIndex");
        }
    }
}
