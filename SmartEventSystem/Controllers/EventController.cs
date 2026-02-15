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

        public IActionResult Browse()
        {
            List<Event> events = new List<Event>();

            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT EventID, EventName, EventDate, EventTime, Price, Description FROM Event";

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
                        Description = reader["Description"].ToString()
                    });
                }
            }

            return View(events);
        }

        public IActionResult Details(int id)
        {
            Event selectedEvent = null;

            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Event WHERE EventID = @EventID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EventID", id);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    selectedEvent = new Event
                    {
                        EventID = (int)reader["EventID"],
                        EventName = reader["EventName"].ToString(),
                        EventDate = (DateTime)reader["EventDate"],
                        EventTime = (TimeSpan)reader["EventTime"],
                        Price = (decimal)reader["Price"],
                        Description = reader["Description"].ToString()
                    };
                }
            }

            return View(selectedEvent);
        }
    }
}
