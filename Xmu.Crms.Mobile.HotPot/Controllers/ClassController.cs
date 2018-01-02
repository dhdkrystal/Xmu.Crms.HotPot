using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class ClassController : Controller
    {
        // GET: Class
        [Route("/Class/ClassManage")]
        public ActionResult ClassManage()
        {
            return View();
        }
 
        [Route("/Class/FixedRollStartCallUI")]
        public ActionResult FixedRollStartCallUI()
        {
            return View();
        }

        [Route("/Class/FixedGroupInfoUI")]
        public ActionResult FixedGroupInfoUI()
        {
            return View();
        }

        [Route("/Class/FixedRollCallUI")]
        public ActionResult FixedRollCallUI()
        {
            return View();
        }

        [Route("/Class/RollCallListUI")]
        public ActionResult RollCallListUI()
        {
            return View();
        }


        [Route("/Class/FixedEndRollCallUI")]
        public ActionResult FixedEndRollCallUI()
        {
            return View();
        }

        [Route("/Class/FixedRollCallEndUI1")]
        public ActionResult FixedRollCallEndUI1()
        {
            return View();
        }

        [Route("/Class/GroupInfoUI")]
        public ActionResult GroupInfoUI()
        {
            return View();
        }
        [Route("/Class/AddGroupMember")]
        public ActionResult GroupInfoUI2()
        {
            return View();
        }
        [Route("/Class/RandomRollStartCallUI")]
        public ActionResult RandomRollStartCallUI()
        {
            return View();
        }

        [Route("/Class/RandomEndRollCallUI")]
        public ActionResult RandomEndRollCallUI()
        {
            return View();
        }

        [Route("/Class/RandomRollCallUI")]
        public ActionResult RandomRollCallUI()
        {
            return View();
        }
    }
}