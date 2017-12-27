using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.HotPot.Controllers
{
    public class ClassController : Controller
    {
        // GET: Class
        [Route("/Class/ClassManage")]
        public ActionResult ClassManage()
        {
            ViewBag.ClassNum = 3;
            ViewBag.CourseName = "OOAD";
            ViewBag.SeminarName = "讨论课4";
            ViewBag.Time = "11月6日-11月12日";
            ViewBag.GroupType = "随机分组";
            return View();
        }
        [Route("/Class")]
        public ActionResult Class(string GroupType)
        {
            if (GroupType == "随机分组")
                Response.Redirect("RandomRollStartCallUI");
            else
                Response.Redirect("FixedRollStartCallUI");
            return View();
        }
        [Route("/Class/FixedRollStartCallUI")]
        public ActionResult FixedRollStartCallUI()
        {
            return View();
        }

        [Route("/Class/CheckGroup")]
        public ActionResult CheckGroup()
        {
            Response.Redirect("FixedGroupInfoUI");
            return View();
        }

        [Route("/Class/FixedGroupInfoUI")]
        public ActionResult FixedGroupInfoUI()
        {
            return View();
        }

        public ActionResult Start()
        {
            Response.Redirect("FixedRollCallUI");
            return View();
        }

        [Route("/Class/FixedRollCallUI")]
        public ActionResult FixedRollCallUI()
        {
            return View();
        }

        [Route("/Class/CheckListTmp")]
        public ActionResult CheckListTmp()
        {
            Response.Redirect("RollCallListUI");
            return View();
        }
        [Route("/Class/RollCallListUI")]
        public ActionResult RollCallListUI()
        {
            return View();
        }

        [Route("/Class/End")]
        public ActionResult End()
        {
            Response.Redirect("FixedEndRollCallUI");
            return View();
        }

        [Route("/Class/FixedEndRollCallUI")]
        public ActionResult FixedEndRollCallUI()
        {
            return View();
        }

        [Route("/Class/CheckList")]
        public ActionResult CheckList()
        {
            Response.Redirect("FixedRollCallEndUI1");
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

        [Route("/Class/ RandomRollCallUI")]
        public ActionResult RandomRollCallUI()
        {
            return View();
        }
    }
}