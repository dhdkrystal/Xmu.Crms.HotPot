using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class SeminarController : Controller
    {
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
        [Route("/Seminar/StudentRollCallLateUI")]
        public ActionResult StudentRollCallLateUI()
        {
            return View();
        }

        [Route("/Seminar/StudentRollCallEndUI")]
        public ActionResult StudentRollCallEndUI()
        {
            return View();
        }
        [Route("/Seminar/GradePresentationUI")]
        public ActionResult GradePresentationUI()
        {
            return View();
        }
        [Route("/Seminar/GradePresentationEndUI")]
        public ActionResult GradePresentationEndUI()
        {
            return View();
        }

        [Route("/Seminar/SeminarRandomGroupNoSelection")]
        public ActionResult SeminarRandomGroupNoSelection()
        {
            return View();
        }

        [Route("/Seminar/SeminarFixedGroupNoSelection")]
        public ActionResult SeminarFixedGroupNoSelection()
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
        [Route("/Seminar/FixedGroupNoLeaderUI")]
        public ActionResult FixedGroupNoLeaderUI()
        {
            return View();
        }
        [Route("/Seminar/ FixedGroupMemberUI")]
        public ActionResult FixedGroupMemberUI()
        {
            return View();
        }

        [Route("/Seminar/ FixedGroupChooseTopicUI2")]
        public ActionResult FixedGroupChooseTopicUI2()
        {
            return View();
        }

        [Route("/Seminar/ RandomGroupLeaderUI")]
        public ActionResult RandomGroupLeaderUI()
        {
            return View();
        }
        [Route("/Seminar/ RandomGroupLeaderUI2")]
        public ActionResult RandomGroupLeaderUI2()
        {
            return View();
        }
        [Route("/Seminar/ RandomGroupNoLeaderUI")]
        public ActionResult RandomGroupNoLeaderUI()
        {
            return View();
        }
        [Route("/Seminar/ RandomGroupMemberUI")]
        public ActionResult RandomGroupMemberUI()
        {
            return View();
        }

        [Route("/Seminar/ RandomGroupChooseTopicUI2")]
        public ActionResult RandomGroupChooseTopicUI2()
        {
            return View();
        }
    }
}