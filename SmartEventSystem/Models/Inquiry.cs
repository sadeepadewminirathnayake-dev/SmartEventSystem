using System;

namespace SmartEventSystem.Models
{
    public class Inquiry
    {
        public int InquiryID { get; set; }
        public int EventID { get; set; }

        public string GuestName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }

        public DateTime InquiryDate { get; set; }
    }
}

