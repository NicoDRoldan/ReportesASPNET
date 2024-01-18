using Microsoft.AspNetCore.Mvc;

namespace ReportesASPNET.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
