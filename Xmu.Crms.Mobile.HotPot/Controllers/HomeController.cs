using Microsoft.AspNetCore.Mvc;
namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class HomeController : Controller
    {

        [Route("/Student/CheckStudentInfo")]
        public ActionResult CheckStudentInfo()
        {
            return View();
        }

        [Route("/Teacher/CheckTeacherInfo")]
        public ActionResult CheckTeacherInfo()
        {
            return View();
        }

        [Route("/Student")]
        public ActionResult Student()
        {
            return View();
        }

        [Route("/Teacher")]
        public ActionResult Teacher()
        {
            return View();
        }
    }
}
