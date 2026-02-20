using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartEventSystem.Models;

public class PaymentController : Controller
{
    private readonly IConfiguration _configuration;

    public PaymentController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // =========================
    // SHOW PAYMENT PAGE
    // =========================
    public IActionResult Create(int bookingId)
    {
        ViewBag.BookingID = bookingId;
        return View();
    }

    // =========================
    // PROCESS PAYMENT
    // =========================
    [HttpPost]
    public IActionResult Create(Payment model)
    {
        string connectionString =
            _configuration.GetConnectionString("SmartEventDBConnection");

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = @"INSERT INTO Payment
                            (PaymentMethod, PaymentDate, Amount, PaymentStatus, BookingID)
                            VALUES
                            (@Method, @Date, @Amount, @Status, @BookingID)";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@Method", model.PaymentMethod);
            cmd.Parameters.AddWithValue("@Date", DateTime.Now);
            cmd.Parameters.AddWithValue("@Amount", model.Amount);
            cmd.Parameters.AddWithValue("@Status", "Completed");
            cmd.Parameters.AddWithValue("@BookingID", model.BookingID);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["PaymentSuccess"] = "Payment completed successfully!";

        return RedirectToAction("MyBookings", "Booking");
    }
}
