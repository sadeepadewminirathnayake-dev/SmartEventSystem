using Microsoft.AspNetCore.Mvc;

namespace SmartEventSystem.Controllers
{
    public class InquiryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
