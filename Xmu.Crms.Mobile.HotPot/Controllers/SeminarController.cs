using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class SeminarController : Controller
    {
        [Route("/Seminar/Seminar")]
        public ActionResult Seminar()
        {
            return View();
        }
        // GET: Seminar
        [Route("/Seminar/StudentRollCallUI")]
        public ActionResult StudentRollCallUI()
        {
            return View();
        }
       
        [Route("/Seminar/GradePresentationUI")]
        public ActionResult GradePresentationUI()
        {
            return View();
        }

        [Route("/Seminar/SeminarNoSelection")]
        public ActionResult SeminarNoSelection()
        {
            return View();
        }

        [Route("/Seminar/FixedGroupLeaderUI")]
        public ActionResult FixedGroupLeaderUI()
        {
            return View();
        }

        [Route("/Seminar/FixedGroupLeaderUI2")]
        public ActionResult FixedGroupLeaderUI2()
        {
            return View();
        }
        [Route("/Seminar/FixedGroupNoLeaderUI2")]
        public ActionResult FixedGroupNoLeaderUI2()
        {
            return View();
        }
        [Route("/Seminar/FixedGroupMemberUI2")]
        public ActionResult FixedGroupMemberUI2()
        {
            return View();
        }

        [Route("/Seminar/FixedGroupChooseTopicUI2")]
        public ActionResult FixedGroupChooseTopicUI2()
        {
            return View();
        }

        [Route("/Seminar/RandomGroupLeaderUI")]
        public ActionResult RandomGroupLeaderUI()
        {
            return View();
        }
        [Route("/Seminar/RandomGroupLeaderUI2")]
        public ActionResult RandomGroupLeaderUI2()
        {
            return View();
        }
        [Route("/Seminar/RandomGroupNoLeaderUI2")]
        public ActionResult RandomGroupNoLeaderUI2()
        {
            return View();
        }
        [Route("/Seminar/RandomGroupMemberUI2")]
        public ActionResult RandomGroupMemberUI2()
        {
            return View();
        }

        [Route("/Seminar/RandomGroupChooseTopicUI2")]
        public ActionResult RandomGroupChooseTopicUI2()
        {
            return View();
        }
    }
}