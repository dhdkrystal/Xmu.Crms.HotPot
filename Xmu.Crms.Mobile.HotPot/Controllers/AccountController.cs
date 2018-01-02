using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class AccountController : Controller
    {
        [Route("/Register")]
        public ActionResult Register()
        {
            return View();
        }

        [Route("/Login")]
        public ActionResult Login()
        {
            return View();
        }

        [Route("/Register/ChooseCharacter")]
        public ActionResult ChooseCharacter()
        {
            return View();
        }

        [Route("/Register/StudentBinding")]
        public ActionResult StudentBinding()
        {
            return View();
        }

        [Route("/Register/TeacherBinding")]
        public ActionResult TeacherBinding()
        {
            return View();
        }

        [Route("/")]
        public ActionResult Home()
        {
            return Redirect("/Login");
        }
        [Route("/Register/ChooseSchool")]
        public ActionResult ChooseSchool()
        {

            return View();
        }
       
        [Route("/Register/CreateSchool")]
        public ActionResult CreateSchoolUI()
        {
            return View();
        }
    }
}
