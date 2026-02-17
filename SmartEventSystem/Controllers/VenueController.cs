using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartEventSystem.Models;

namespace SmartEventSystem.Controllers
{
    public class VenueController : Controller
    {
        private readonly IConfiguration _configuration;

        public VenueController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Details(int id)
        {
            Venue venue = null;

            string connectionString =
                _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT VenueID, VenueName, Address, Capacity
                                 FROM Venue
                                 WHERE VenueID = @VenueID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@VenueID", id);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    venue = new Venue
                    {
                        VenueID = (int)reader["VenueID"],
                        VenueName = reader["VenueName"].ToString(),
                        Address = reader["Address"].ToString(),
                        Capacity = (int)reader["Capacity"]
                    };
                }
            }

            if (venue == null)
                return NotFound();

            return View(venue);
        }
    }
}
