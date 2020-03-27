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
        private readonly IInstaApi ap;

        public IGController(IIGService iGService, UserManager<ApplicationUser> userManager)
        {
            this.iGService = iGService;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult TwoFactor(string phone)
        {
            var login = new LoginViewModel
            {
                Phone = phone,
            };
            return this.View(login);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactor(LoginViewModel viewModel)
        {
            var result = await this.iGService.TwoFactor(viewModel.Code, viewModel.Phone);

            if (!result)
            {
                this.RedirectToAction("Login");
            }

            return this.View("IGIndex");
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
            if (phone != string.Empty)
            {

                return this.RedirectToAction("TwoFactor", new { phone = viewModel.Phone });
            }

            return this.View("IGIndex");
        }
    }
}
