namespace HashtagIT.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using HashtagIT.Data.Models;
    using HashtagIT.Services.Data;
    using HashtagIT.Web.ViewModels.IG;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
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

        public IActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string guest)
        {
            var userId = this.userManager.GetUserId(this.User);
            if (guest != null)
            {
                await this.iGService.Login(userId, "system", string.Empty);
                return this.RedirectToAction(nameof(this.IGIndex));
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var phone = await this.iGService.Login(userId, viewModel.IGUserName, viewModel.Password);
            string encryptedPassword = Cipher.Encrypt(viewModel.Password, viewModel.IGUserName);

            viewModel.Phone = phone;
            viewModel.Password = encryptedPassword;
            viewModel.UserId = userId;

            if (phone != string.Empty)
            {
                return this.RedirectToAction(nameof(this.TwoFactor), viewModel);
            }

            return this.RedirectToAction(nameof(this.IGIndex));
        }

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
                this.RedirectToAction(nameof(this.Login));
            }

            return this.RedirectToAction(nameof(this.IGIndex));
        }

        public IActionResult IGIndex()
        {
            var userId = this.userManager.GetUserId(this.User);
            if (this.api.GetInstance(userId) == null)
            {
                return this.RedirectToAction(nameof(this.Login));
            }

            return this.View();
        }

        [HttpPost]
        public IActionResult TopNine(string username)
        {
            return this.RedirectToAction(nameof(this.TopNineResult), new { username });
        }

        public async Task<IActionResult> TopNineResult(string username)
        {
            var userId = this.userManager.GetUserId(this.User);
            TopNineViewModel viewModel = await this.iGService.TopNine(userId, username);
            return this.View(viewModel);
        }
    }
}
