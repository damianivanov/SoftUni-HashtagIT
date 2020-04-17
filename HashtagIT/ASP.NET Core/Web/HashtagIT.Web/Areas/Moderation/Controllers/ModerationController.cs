namespace HashtagIT.Web.Areas.Moderation.Controllers
{
    using HashtagIT.Services.Data;
    using HashtagIT.Web.Controllers;
    using HashtagIT.Web.ViewModels.Administration.Dashboard;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = "Moderator")]
    [Area("Moderation")]
    public class ModerationController : BaseController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
