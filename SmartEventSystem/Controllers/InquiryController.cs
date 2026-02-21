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

        public IActionResult Management()
        {
            List<Inquiry> inquiries = new List<Inquiry>();

            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Inquiry ORDER BY InquiryDate DESC";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    inquiries.Add(new Inquiry
                    {
                        InquiryID = Convert.ToInt32(reader["InquiryID"]),
                        EventID = Convert.ToInt32(reader["EventID"]),
                        GuestName = reader["GuestName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Message = reader["Message"].ToString(),
                        InquiryDate = Convert.ToDateTime(reader["InquiryDate"])
                    });
                }
            }

            return View(inquiries);
        }

        public IActionResult Reply(int id)
        {
            Inquiry inquiry = new Inquiry();

            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Inquiry WHERE InquiryID = @InquiryID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@InquiryID", id);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    inquiry.InquiryID = Convert.ToInt32(reader["InquiryID"]);
                    inquiry.EventID = Convert.ToInt32(reader["EventID"]);
                    inquiry.GuestName = reader["GuestName"].ToString();
                    inquiry.Email = reader["Email"].ToString();
                    inquiry.Message = reader["Message"].ToString();
                    inquiry.InquiryDate = Convert.ToDateTime(reader["InquiryDate"]);
                }
            }

            return View(inquiry);
        }

        [HttpPost]
        public IActionResult Reply(Inquiry inquiry)
        {
            string connectionString = _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Inquiry
                         SET Reply = @Reply,
                             Status = 'Answered'
                         WHERE InquiryID = @InquiryID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Reply", inquiry.Reply ?? "");
                cmd.Parameters.AddWithValue("@InquiryID", inquiry.InquiryID);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Management");
        }




    }
}

