namespace HashtagIT.Web.Controllers
{
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
            if (guest != null)
            {
                return this.RedirectToAction("GuestIndex");
            }

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

            return this.RedirectToAction("IGIndex");
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
                this.RedirectToAction("Login");
            }

            return this.RedirectToAction("IGIndex");
        }

        public async Task<IActionResult> IGIndex()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var userId = user.Id;
            if (this.api.GetInstance(userId) == null)
            {
                return this.RedirectToAction("Login");
            }

            return this.View();
        }

        public IActionResult GuestIndex()
        {
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
