namespace HashtagIT.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using HashtagIT.Common;
    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using HashtagIT.Services.Messaging;
    using HashtagIT.Web.ViewModels;
    using HashtagIT.Web.ViewModels.Contact;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IEmailSender emailSender;
        private readonly IRepository<ContactEntry> contactRepository;

        public HomeController(IEmailSender emailSender, IRepository<ContactEntry> contactRepository)
        {
            this.emailSender = emailSender;
            this.contactRepository = contactRepository;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
