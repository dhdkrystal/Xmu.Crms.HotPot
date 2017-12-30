using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class SchoolController : Controller
    {
        // GET: School

        [Route("/Register/ChooseSchool")]
        public ActionResult ChooseSchool()
        {
            
            return View();
        }
        [Route("/Register/ChooseSchool1")]
        public ActionResult ChooseSchool1()
        {
            return View();
        }

        [Route("/Register/ChooseSchoo2")]
        public ActionResult ChooseSchool2()
        {
            return View();
        }

        [Route("/Register/CreateSchoolUI")]
        public ActionResult CreateSchoolUI()
        {
            return View();
        }

    }
}