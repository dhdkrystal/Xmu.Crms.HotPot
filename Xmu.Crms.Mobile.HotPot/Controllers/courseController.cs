using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Mobile.HotPot.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course
        public ActionResult CourseInfoUI()
        {
            return View();
        }
        public ActionResult CourseUI()
        {
            return View();
        }
    }
}