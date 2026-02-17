using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartEventSystem.Models;

namespace SmartEventSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // ✅ HOME PAGE WITH UPCOMING EVENTS
        public IActionResult Index()
        {
            List<Event> events = new List<Event>();

            string connectionString =
                _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 6 EventID, EventName, EventDate, EventTime,
                                        Price, Description, VenueID
                                 FROM Event
                                 WHERE EventDate >= GETDATE()
                                 ORDER BY EventDate ASC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    events.Add(new Event
                    {
                        EventID = (int)reader["EventID"],
                        EventName = reader["EventName"].ToString(),
                        EventDate = (DateTime)reader["EventDate"],
                        EventTime = (TimeSpan)reader["EventTime"],
                        Price = (decimal)reader["Price"],
                        Description = reader["Description"].ToString(),
                        VenueID = (int)reader["VenueID"]
                    });
                }
            }

            return View(events); // ✅ IMPORTANT
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // ✅ Restrict search for guests
        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                HttpContext.Session.SetString("PendingSearch", searchTerm);
                return RedirectToAction("Login", "Account");
            }

            return PerformSearch(searchTerm);
        }

        private IActionResult PerformSearch(string searchTerm)
        {
            List<Event> events = new List<Event>();

            string connectionString =
                _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT EventID, EventName, EventDate, EventTime,
                                        Price, Description, VenueID
                                 FROM Event
                                 WHERE EventName LIKE @Search";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    events.Add(new Event
                    {
                        EventID = (int)reader["EventID"],
                        EventName = reader["EventName"].ToString(),
                        EventDate = (DateTime)reader["EventDate"],
                        EventTime = (TimeSpan)reader["EventTime"],
                        Price = (decimal)reader["Price"],
                        Description = reader["Description"].ToString(),
                        VenueID = (int)reader["VenueID"]
                    });
                }
            }

            return View("SearchResult", events);
        }

        public IActionResult SearchAfterLogin(string searchTerm)
        {
            return PerformSearch(searchTerm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
