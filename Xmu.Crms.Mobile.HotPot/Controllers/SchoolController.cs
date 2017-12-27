using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class SchoolController : Controller
    {
        // GET: School

        [Route("/School/ChooseSchool")]
        public ActionResult ChooseSchool()
        {
            
            return View();
        }
        [Route("/School/ChooseSchool1")]
        public ActionResult ChooseSchool1()
        {
            return View();
        }

        [Route("/School/ChooseSchoo2")]
        public ActionResult ChooseSchool2()
        {
            return View();
        }

        [Route("/School/CreateSchoolUI")]
        public ActionResult CreateSchoolUI()
        {
            return View();
        }

    }
}