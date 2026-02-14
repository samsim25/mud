using Microsoft.AspNetCore.Mvc;

namespace Avalon.Web.Controllers
{
    public class ClientController : Controller
    {
        [Route("/client")]
        public IActionResult Index()
        {
            // Render a minimal client interface
            return View();
        }
    }
}
