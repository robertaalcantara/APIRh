using System.Web.Mvc;

namespace APIRh.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "API RH";

            return View();
        }
    }
}