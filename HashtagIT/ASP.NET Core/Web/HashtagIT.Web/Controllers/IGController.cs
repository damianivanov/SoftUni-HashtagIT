namespace HashtagIT.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HashtagIT.Data.Models;
    using HashtagIT.Services.Data;
    using HashtagIT.Web.ViewModels.IG;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class IGController : Controller
    {
        private readonly IIGService iGService;
        private readonly UserManager<ApplicationUser> userManager;

        public IGController(IIGService iGService, UserManager<ApplicationUser> userManager)
        {
            this.iGService = iGService;
            this.userManager = userManager;
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

            return this.View();
        }
    }
}