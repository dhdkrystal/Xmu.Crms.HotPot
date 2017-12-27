using Microsoft.AspNetCore.Mvc;
namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class HomeController : Controller
    {

        [Route("/Home/CheckStudentInfo")]
        public ActionResult CheckStudentInfo()
        {
            return View();
        }

        [Route("/Course/CheckTeacherInfo")]
        public ActionResult CheckTeacherInfo()
        {
            return View();
        }

        [Route("/Course/Student")]
        public ActionResult Student()
        {
            return View();
        }

        [Route("/Course/Teacher")]
        public ActionResult Teacher()
        {
            return View();
        }
    }
}
