using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartEventSystem.Models;

namespace SmartEventSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly IConfiguration _configuration;

        public BookingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 🔒 GET: Booking/Create
        [HttpGet]
        public IActionResult Create(int eventId)
        {
            // Check login
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                HttpContext.Session.SetInt32("PendingBookingEventId", eventId);
                return RedirectToAction("Login", "Account");
            }

            Booking booking = new Booking
            {
                EventID = eventId
            };

            return View(booking);
        }

        // 🔒 POST: Booking/Create
        [HttpPost]
        public IActionResult Create(Booking model)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.Quantity <= 0)
            {
                ModelState.AddModelError("", "Quantity must be greater than 0.");
                return View(model);
            }

            string connectionString =
                _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Get MemberID
                string getMemberQuery =
                    "SELECT MemberID FROM Member WHERE Email=@Email";

                SqlCommand memberCmd =
                    new SqlCommand(getMemberQuery, con);

                memberCmd.Parameters.AddWithValue(
                    "@Email",
                    HttpContext.Session.GetString("UserEmail"));

                object result = memberCmd.ExecuteScalar();

                if (result == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                int memberId = (int)result;

                // Seat pricing logic
                decimal ticketPrice = model.SeatType switch
                {
                    "Regular" => 2500,
                    "Balcony" => 3000,
                    "Premium" => 3500,
                    "VIP" => 5000,
                    "Front Row" => 6000,
                    "VVIP" => 8000,
                    _ => 2500
                };

                decimal totalAmount = ticketPrice * model.Quantity;

                // =============================
                // Insert Booking
                // =============================
                string bookingQuery = @"INSERT INTO Booking
                                        (MemberID, EventID, BookingDate, TotalAmount)
                                        OUTPUT INSERTED.BookingID
                                        VALUES (@MemberID, @EventID, @BookingDate, @TotalAmount)";

                SqlCommand bookingCmd = new SqlCommand(bookingQuery, con);
                bookingCmd.Parameters.AddWithValue("@MemberID", memberId);
                bookingCmd.Parameters.AddWithValue("@EventID", model.EventID);
                bookingCmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);
                bookingCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);

                int bookingId = (int)bookingCmd.ExecuteScalar();

                // =============================
                // Insert Ticket
                // =============================
                string ticketQuery = @"INSERT INTO Ticket
                                       (BookingID, SeatType, Quantity, Price)
                                       VALUES (@BookingID, @SeatType, @Quantity, @Price)";

                SqlCommand ticketCmd = new SqlCommand(ticketQuery, con);
                ticketCmd.Parameters.AddWithValue("@BookingID", bookingId);
                ticketCmd.Parameters.AddWithValue("@SeatType", model.SeatType);
                ticketCmd.Parameters.AddWithValue("@Quantity", model.Quantity);
                ticketCmd.Parameters.AddWithValue("@Price", ticketPrice);

                ticketCmd.ExecuteNonQuery();

                // =============================
                // Redirect To Payment Page
                // =============================
                return RedirectToAction("Create", "Payment",
                    new
                    {
                        bookingId = bookingId,
                        amount = totalAmount
                    });
            }
        }
    }
}


