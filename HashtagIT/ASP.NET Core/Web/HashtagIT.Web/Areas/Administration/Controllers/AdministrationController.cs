﻿namespace HashtagIT.Web.Areas.Administration.Controllers
{
    using HashtagIT.Common;
    using HashtagIT.Data.Models;
    using HashtagIT.Web.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
