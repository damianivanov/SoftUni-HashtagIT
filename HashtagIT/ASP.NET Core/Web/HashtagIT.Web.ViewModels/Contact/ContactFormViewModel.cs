namespace HashtagIT.Web.ViewModels.Contact
{
    using System.ComponentModel.DataAnnotations;

    using HashtagIT.Data.Models;
    using HashtagIT.Services.Mapping;
    using HashtagIT.Web.Infrastructure;

    public class ContactFormViewModel : IMapFrom<ContactEntry>
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Your Name")]
        [Display(Name = "Your Name")]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Your Email.")]
        [EmailAddress(ErrorMessage = "Enter Valid Email")]
        [Display(Name = "Your Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Your Subject.")]
        [StringLength(100, ErrorMessage = "Subject has to be atleast {2} symbols and up to {1}.", MinimumLength = 5)]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter  Your Message.")]
        [StringLength(10000, ErrorMessage = "Message has to be atlest {2} symbols.", MinimumLength = 20)]
        [Display(Name = "Your Message")]
        public string Content { get; set; }

        public bool IsValid { get; set; }

        // [GoogleReCaptchaValidation]
        // public string RecaptchaValue { get; set; }
    }
}
