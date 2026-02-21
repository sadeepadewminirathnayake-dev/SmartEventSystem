using System;

namespace SmartEventSystem.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        public int MemberID { get; set; }

        public int EventID { get; set; }

        public DateTime BookingDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string SeatType { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string PaymentStatus { get; set; }
    }
}

