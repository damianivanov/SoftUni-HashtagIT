namespace HashtagIT.Web.ViewModels.IG
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using HashtagIT.Data.Models;
    using HashtagIT.Services.Mapping;

    public class LoginViewModel : IMapFrom<IGUser>
    {
        [Required]
        public string IGUserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string UserId { get; set; }

        public string Phone { get; set; }

        public string Code { get; set; }
    }
}
