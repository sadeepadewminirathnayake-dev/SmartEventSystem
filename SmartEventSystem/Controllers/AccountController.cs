using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartEventSystem.Models;

namespace SmartEventSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // =========================
        // REGISTER
        // =========================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Member model)
        {
            if (ModelState.IsValid)
            {
                string connectionString =
                    _configuration.GetConnectionString("SmartEventDBConnection");

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Member 
                                    (FullName, Email, Password, DOB, ContactNB, RegistrationDate, Preferences)
                                    VALUES 
                                    (@FullName, @Email, @Password, @DOB, @ContactNB, @RegistrationDate, @Preferences)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@FullName", model.FullName);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@DOB", model.DOB);
                    cmd.Parameters.AddWithValue("@ContactNB", model.ContactNB);
                    cmd.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Preferences",
                        model.Preferences ?? (object)DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // =========================
        // LOGIN
        // =========================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Member model)
        {
            if (string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Message = "Email or Password is empty";
                return View();
            }

            string connectionString =
                _configuration.GetConnectionString("SmartEventDBConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query =
                    "SELECT FullName FROM Member WHERE Email=@Email AND Password=@Password";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Password", model.Password);

                con.Open();

                var result = cmd.ExecuteScalar();

                if (result != null)
                {
                    // ✅ Store session
                    HttpContext.Session.SetString("UserEmail", model.Email);
                    HttpContext.Session.SetString("UserName", result.ToString());

                    // ✅ Check if there was a pending search
                    string pendingSearch =
                        HttpContext.Session.GetString("PendingSearch");

                    if (!string.IsNullOrEmpty(pendingSearch))
                    {
                        HttpContext.Session.Remove("PendingSearch");

                        return RedirectToAction(
                            "SearchAfterLogin",
                            "Home",
                            new { searchTerm = pendingSearch });
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "Invalid Email or Password";
                }
            }

            return View();
        }

        // =========================
        // LOGOUT
        // =========================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}


