namespace HashtagIT.Web.Areas.Moderation.Controllers
{
    using HashtagIT.Services.Data;
    using HashtagIT.Web.ViewModels.Administration.Dashboard;

    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : ModerationController
    {
        public IActionResult IndexDashboard()
        {
            return this.View();
        }
    }
}
