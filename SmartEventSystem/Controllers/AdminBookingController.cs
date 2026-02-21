using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SmartEventSystem.Controllers
{
    public class AdminBookingController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminBookingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Index → show all bookings
        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");
            var bookings = new List<dynamic>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT b.BookingID, m.FullName, e.EventName, 
                                        b.TotalAmount, b.BookingStatus, 
                                        b.PaymentStatus, b.RefundStatus
                                 FROM Booking b
                                 INNER JOIN Member m ON b.MemberID = m.MemberID
                                 INNER JOIN Event e ON b.EventID = e.EventID";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bookings.Add(new
                        {
                            BookingID = reader["BookingID"],
                            FullName = reader["FullName"],
                            EventName = reader["EventName"],
                            TotalAmount = reader["TotalAmount"],
                            BookingStatus = reader["BookingStatus"],
                            PaymentStatus = reader["PaymentStatus"],
                            RefundStatus = reader["RefundStatus"]
                        });
                    }
                }
            }

            return View(bookings);
        }

        // Approve booking → set BookingStatus + PaymentStatus
        public IActionResult Approve(int id)
        {
            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Booking SET BookingStatus='Approved', PaymentStatus='Paid' WHERE BookingID=@ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Cancel booking → set BookingStatus + PaymentStatus
        public IActionResult Cancel(int id)
        {
            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Booking SET BookingStatus='Cancelled', PaymentStatus='Failed' WHERE BookingID=@ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Refund booking → set RefundStatus + PaymentStatus
        public IActionResult Refund(int id)
        {
            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE Booking SET RefundStatus='Refunded', PaymentStatus='Refunded' WHERE BookingID=@ID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}


