using Microsoft.AspNetCore.Mvc;

namespace ReportesASPNET.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
