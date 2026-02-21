using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartEventSystem.Models;

namespace SmartEventSystem.Controllers
{
    public class EventController : Controller
    {
        private readonly IConfiguration _configuration;

        public EventController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ===============================
        // 🔹 Browse Events (Guest + Member)
        // ===============================
        public IActionResult Browse()
        {
            List<Event> events = new List<Event>();
            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

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
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int capacity = reader["Capacity"] != DBNull.Value ? (int)reader["Capacity"] : 0;
                    int bookedSeats = reader["BookedSeats"] != DBNull.Value ? (int)reader["BookedSeats"] : 0;

                    string status = (capacity - bookedSeats) > 0 ? "Available" : "Full";

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
        // 🔹 Event Details
        // ===============================
        public IActionResult Details(int id)
        {
            Event selectedEvent = null;
            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

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
                    V.Capacity,
                    ISNULL(SUM(T.Quantity),0) AS BookedSeats
                FROM Event E
                LEFT JOIN Venue V ON E.VenueID = V.VenueID
                LEFT JOIN Booking B ON E.EventID = B.EventID
                LEFT JOIN Ticket T ON B.BookingID = T.BookingID
                WHERE E.EventID = @EventID
                GROUP BY 
                    E.EventID,
                    E.EventName,
                    E.EventDate,
                    E.EventTime,
                    E.Price,
                    E.Description,
                    V.Capacity";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EventID", id);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int capacity = reader["Capacity"] != DBNull.Value ? (int)reader["Capacity"] : 0;
                    int bookedSeats = reader["BookedSeats"] != DBNull.Value ? (int)reader["BookedSeats"] : 0;

                    string status = (capacity - bookedSeats) > 0 ? "Available" : "Full";

                    selectedEvent = new Event
                    {
                        EventID = (int)reader["EventID"],
                        EventName = reader["EventName"].ToString(),
                        EventDate = (DateTime)reader["EventDate"],
                        EventTime = (TimeSpan)reader["EventTime"],
                        Price = (decimal)reader["Price"],
                        Description = reader["Description"].ToString(),
                        AvailabilityStatus = status
                    };
                }
            }

            return View(selectedEvent);
        }

        

            
        


    }
}


