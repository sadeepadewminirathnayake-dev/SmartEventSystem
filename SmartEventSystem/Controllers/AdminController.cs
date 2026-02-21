using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartEventSystem.Models;

namespace SmartEventSystem.Controllers
{
    public class AdminEventController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminEventController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ============================
        // GET: Create Event
        // ============================
        [HttpGet]
        public IActionResult Create()
        {
            // 🔒 Check Admin Login
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // ============================
        // POST: Create Event
        // ============================
        [HttpPost]
        public IActionResult Create(Event model)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            string connectionString =
                _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Event
                                (EventName, EventDate, EventTime, Price, Description, VenueID)
                                VALUES
                                (@EventName, @EventDate, @EventTime, @Price, @Description, @VenueID)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@EventName", model.EventName);
                cmd.Parameters.AddWithValue("@EventDate", model.EventDate);
                cmd.Parameters.AddWithValue("@EventTime", model.EventTime);
                cmd.Parameters.AddWithValue("@Price", model.Price);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@VenueID", model.VenueID);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["EventSuccess"] = "Event created successfully!";

            return RedirectToAction("Create");
        }
    }
}
