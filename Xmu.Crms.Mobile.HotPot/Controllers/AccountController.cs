using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class AccountController : Controller
    {
        [Route("/Account/Register")]
        public ActionResult Register()
        {
            return View();
        }

        [Route("/Login")]
        public ActionResult Login()
        {
            return View();
        }

        [Route("/Account/ChooseCharacter")]
        public ActionResult ChooseCharacter()
        {
            return View();
        }

        [Route("/Account/StudentBinding")]
        public ActionResult StudentBinding()
        {
            return View();
        }

        [Route("/Account/TeacherBinding")]
        public ActionResult TeacherBinding()
        {
            return View();
        }

        [Route("/")]
        public ActionResult Home()
        {
            return Redirect("/Login");
        }
    }
}
