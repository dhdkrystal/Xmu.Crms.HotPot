using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class TeacherController : Controller
    {
        [Route("/Teacher/ClassManage")]
        public ActionResult ClassManage()
        {
            return View();
        }
        [Route("/Teacher/FixedEndRollCallUI")]
        public ActionResult FixedEndRollCallUI()
        {
            return View();
        }

        [Route("/Teacher/FixedGroupInfoUI")]
        public ActionResult FixedGroupInfoUI()
        {
            return View();
        }
        [Route("/Teacher/FixedRollCallEndUI1")]
        public ActionResult FixedRollCallEndUI1()
        {
            return View();
        }
        [Route("/Teacher/FixedRollCallUI")]
        public ActionResult FixedRollCallUI()
        {
            return View();
        }

        [Route("/Teacher/FixedRollStartCallUI")]
        public ActionResult FixedRollStartCallUI()
        {
            return View();
        }
        [Route("/Teacher/GroupInforUI")]
        public ActionResult GroupInforUI()
        {
            return View();
        }
        [Route("/Teacher/GroupInforUI2")]
        public ActionResult GroupInforUI2()
        {
            return View();
        }

        [Route("/Teacher/RandomEndRollCallUI")]
        public ActionResult RandomEndRollCallUI()
        {
            return View();
        }

        [Route("/Teacher/RandomRollCallUI")]
        public ActionResult RandomRollCallUI()
        {
            return View();
        }
        [Route("/Teacher/RandomRollStartCallUI")]
        public ActionResult RandomRollStartCallUI()
        {
            return View();
        }

        [Route("/Teacher/RollCallListUI")]
        public ActionResult RollCallListUI()
        {
            return View();
        }


    }
}
