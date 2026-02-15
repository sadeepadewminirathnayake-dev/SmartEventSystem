using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartEventSystem.Models;

namespace SmartEventSystem.Controllers
{
    public class InquiryController : Controller
    {
        private readonly IConfiguration _configuration;

        public InquiryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // OPEN PAGE (Guest allowed)
        [HttpGet]
        public IActionResult Create(int eventId)
        {
            Inquiry inquiry = new Inquiry
            {
                EventID = eventId
            };

            return View(inquiry);
        }

        // SUBMIT INQUIRY
        [HttpPost]
        public IActionResult Create(Inquiry model)
        {
            if (ModelState.IsValid)
            {
                string connectionString =
                    _configuration.GetConnectionString("SmartEventDBConnection");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Inquiry
                                    (EventID, GuestName, Email, Message, InquiryDate)
                                    VALUES
                                    (@EventID, @GuestName, @Email, @Message, GETDATE())";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@EventID", model.EventID);
                    cmd.Parameters.AddWithValue("@GuestName", model.GuestName);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Message", model.Message);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Message = "Inquiry submitted successfully!";
            }

            return View(model);
        }
    }
}

