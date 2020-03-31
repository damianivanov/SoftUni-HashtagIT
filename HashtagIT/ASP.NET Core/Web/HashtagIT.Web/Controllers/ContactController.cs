namespace HashtagIT.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using HashtagIT.Common;
    using HashtagIT.Data.Common.Repositories;
    using HashtagIT.Data.Models;
    using HashtagIT.Services.Messaging;
    using HashtagIT.Web.ViewModels.Contact;
    using Microsoft.AspNetCore.Mvc;

    public class ContactController : Controller
    {
        private readonly IEmailSender emailSender;

        private readonly IRepository<ContactEntry> contactRepository;

        public ContactController(IEmailSender emailSender, IRepository<ContactEntry> contactRepository)
        {
            this.contactRepository = contactRepository;
            this.emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactFormViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            viewModel.IsValid = true;

            var contactFormEntry = new ContactEntry
            {
                FullName = viewModel.FullName,
                Email = viewModel.Email,
                Subject = viewModel.Subject,
                Content = viewModel.Content,

            };
            await this.contactRepository.AddAsync(contactFormEntry);
            await this.contactRepository.SaveChangesAsync();
            await this.emailSender.SendEmailAsync(viewModel.Email, viewModel.FullName, GlobalConstants.SystemEmail, viewModel.Subject, viewModel.Content);
            return this.Redirect("/");
        }
    }
}
