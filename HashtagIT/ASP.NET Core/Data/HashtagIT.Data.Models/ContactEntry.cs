namespace HashtagIT.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using HashtagIT.Data.Common.Models;

    public class ContactEntry : BaseModel<int>
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
    }
}
