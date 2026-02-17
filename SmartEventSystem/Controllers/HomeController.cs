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

        // ✅ Inject BOTH logger and configuration
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        // ✅ Restrict search for guests
        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                // Save search term in session
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
                string query = @"SELECT EventID, EventName, EventDate, EventTime, Price, Description
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
                        Description = reader["Description"].ToString()
                    });
                }
            }

            return View("SearchResult", events);
        }

        public IActionResult SearchAfterLogin(string searchTerm)
        {
            return PerformSearch(searchTerm);
        }


    }
    
}
