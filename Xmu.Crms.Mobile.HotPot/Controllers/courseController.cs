using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course
        [Route("/Course/CourseInfoUI")]
        public ActionResult CourseInfoUI()
        {
            return View();
        }

        [Route("/Course/CourseUI")]
        public ActionResult CourseUI()
        {
            return View();
        }
    }
}