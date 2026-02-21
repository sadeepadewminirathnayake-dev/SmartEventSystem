using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartEventSystem.Models;

namespace SmartEventSystem.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IConfiguration _configuration;

        public ReviewController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ✅ GET: Review/Create
        [HttpGet]
        public IActionResult Create(int eventId)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Review review = new Review
            {
                EventID = eventId
            };

            return View(review);
        }

        // ✅ POST: Review/Create
        [HttpPost]
        public IActionResult Create(Review model)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Get MemberID from session email
                string memberQuery = "SELECT MemberID FROM Member WHERE Email=@Email";
                SqlCommand memberCmd = new SqlCommand(memberQuery, con);
                memberCmd.Parameters.AddWithValue("@Email", HttpContext.Session.GetString("UserEmail"));

                int memberId = (int)memberCmd.ExecuteScalar();

                string reviewQuery = @"INSERT INTO Review 
                                      (MemberID, EventID, Rating, Comment, ReviewDate)
                                      VALUES (@MemberID, @EventID, @Rating, @Comment, @ReviewDate)";

                SqlCommand cmd = new SqlCommand(reviewQuery, con);
                cmd.Parameters.AddWithValue("@MemberID", memberId);
                cmd.Parameters.AddWithValue("@EventID", model.EventID);
                cmd.Parameters.AddWithValue("@Rating", model.Rating);
                cmd.Parameters.AddWithValue("@Comment", model.Comment);
                cmd.Parameters.AddWithValue("@ReviewDate", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
            TempData["ReviewSuccess"] = "Review submitted successfully!";
            return RedirectToAction("Details", "Event", new { id = model.EventID });
        }

        public IActionResult Browse(int eventId)
        {
            List<Review> reviews = new List<Review>();

            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT Rating, Comment, ReviewDate
                         FROM Review
                         WHERE EventID = @EventID
                         ORDER BY ReviewDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EventID", eventId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reviews.Add(new Review
                    {
                        Rating = (int)reader["Rating"],
                        Comment = reader["Comment"]?.ToString(),
                        ReviewDate = (DateTime)reader["ReviewDate"],
                        EventID = eventId
                    });
                }
            }

            ViewBag.EventID = eventId;

            return View(reviews);


        }

        public IActionResult Management()
        {
            List<Review> reviews = new List<Review>();

            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT ReviewID, MemberID, EventID, Rating, Comment, ReviewDate
                         FROM Review
                         ORDER BY ReviewDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reviews.Add(new Review
                    {
                        ReviewID = Convert.ToInt32(reader["ReviewID"]),
                        MemberID = Convert.ToInt32(reader["MemberID"]),
                        EventID = Convert.ToInt32(reader["EventID"]),
                        Rating = Convert.ToInt32(reader["Rating"]),
                        Comment = reader["Comment"].ToString(),
                        ReviewDate = Convert.ToDateTime(reader["ReviewDate"])
                    });
                }
            }

            return View(reviews);
        }

        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Review WHERE ReviewID = @ReviewID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ReviewID", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Management");
        }









    }
}
