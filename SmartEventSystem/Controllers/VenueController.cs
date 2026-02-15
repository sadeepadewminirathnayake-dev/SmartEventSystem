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

        // VIEW VENUE DETAILS
        public IActionResult Details(string venueName)
        {
            Venue venue = null;

            string connectionString =
                _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT VenueName, Address, City, Capacity, Description
                                 FROM Venue
                                 WHERE VenueName = @VenueName";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@VenueName", venueName);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    venue = new Venue
                    {
                        VenueName = reader["VenueName"].ToString(),
                        Address = reader["Address"].ToString(),
                        City = reader["City"].ToString(),
                        Capacity = (int)reader["Capacity"],
                        Description = reader["Description"].ToString()
                    };
                }
            }

            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }
    }
}

