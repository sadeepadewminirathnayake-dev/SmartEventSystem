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

        // ===============================
        // ✅ HOME PAGE WITH UPCOMING EVENTS
        // ===============================
        public IActionResult Index()
        {
            List<Event> events = new List<Event>();

            string connectionString =
                _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT TOP 6 
                    E.EventID,
                    E.EventName,
                    E.EventDate,
                    E.EventTime,
                    E.Price,
                    E.Description,
                    E.VenueID,
                    V.Capacity,
                    ISNULL(SUM(T.Quantity),0) AS BookedSeats
                FROM Event E
                LEFT JOIN Venue V ON E.VenueID = V.VenueID
                LEFT JOIN Booking B ON E.EventID = B.EventID
                LEFT JOIN Ticket T ON B.BookingID = T.BookingID
                WHERE E.EventDate >= GETDATE()
                GROUP BY 
                    E.EventID,
                    E.EventName,
                    E.EventDate,
                    E.EventTime,
                    E.Price,
                    E.Description,
                    E.VenueID,
                    V.Capacity
                ORDER BY E.EventDate ASC";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int capacity = reader["Capacity"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Capacity"]);
                    int bookedSeats = reader["BookedSeats"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BookedSeats"]);

                    string status = (capacity > 0 && capacity - bookedSeats > 0) ? "Available" : "Full";

                    events.Add(new Event
                    {
                        EventID = (int)reader["EventID"],
                        EventName = reader["EventName"].ToString(),
                        EventDate = (DateTime)reader["EventDate"],
                        EventTime = (TimeSpan)reader["EventTime"],
                        Price = (decimal)reader["Price"],
                        Description = reader["Description"].ToString(),
                        VenueID = (int)reader["VenueID"],
                        AvailabilityStatus = status
                    });
                }
            }

            return View(events);
        }

        // ===============================
        // ✅ Allow guests to search
        // ===============================
        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            return PerformSearch(searchTerm);
        }

        private IActionResult PerformSearch(string searchTerm)
        {
            List<Event> events = new List<Event>();

            string connectionString =
                _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    E.EventID,
                    E.EventName,
                    E.EventDate,
                    E.EventTime,
                    E.Price,
                    E.Description,
                    E.VenueID,
                    V.Capacity,
                    ISNULL(SUM(T.Quantity),0) AS BookedSeats
                FROM Event E
                LEFT JOIN Venue V ON E.VenueID = V.VenueID
                LEFT JOIN Booking B ON E.EventID = B.EventID
                LEFT JOIN Ticket T ON B.BookingID = T.BookingID
                WHERE E.EventName LIKE @Search
                GROUP BY 
                    E.EventID,
                    E.EventName,
                    E.EventDate,
                    E.EventTime,
                    E.Price,
                    E.Description,
                    E.VenueID,
                    V.Capacity";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Search", "%" + searchTerm + "%");

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int capacity = reader["Capacity"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Capacity"]);
                    int bookedSeats = reader["BookedSeats"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BookedSeats"]);

                    string status = (capacity > 0 && capacity - bookedSeats > 0) ? "Available" : "Full";

                    events.Add(new Event
                    {
                        EventID = (int)reader["EventID"],
                        EventName = reader["EventName"].ToString(),
                        EventDate = (DateTime)reader["EventDate"],
                        EventTime = (TimeSpan)reader["EventTime"],
                        Price = (decimal)reader["Price"],
                        Description = reader["Description"].ToString(),
                        VenueID = (int)reader["VenueID"],
                        AvailabilityStatus = status
                    });
                }
            }

            return View("SearchResult", events);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
