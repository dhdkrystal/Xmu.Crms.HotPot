using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Mobile.HotPot.Controllers
{
    public class ClassController : Controller
    {
        // GET: Class
        public ActionResult ClassManage()
        {
            ViewBag.ClassNum = 3;
            ViewBag.CourseName = "OOAD";
            ViewBag.SeminarName = "讨论课4";
            ViewBag.Time = "11月6日-11月12日";
            ViewBag.GroupType = "随机分组";
            return View();
        }

        public ActionResult Class(string GroupType)
        {
            if (GroupType == "随机分组")
                Response.Redirect("RandomRollStartCallUI");
            else
                Response.Redirect("FixedRollStartCallUI");
            return View();
        }
        public ActionResult FixedRollStartCallUI()
        {
            return View();
        }
        public ActionResult CheckGroup()
        {
            Response.Redirect("FixedGroupInfoUI");
            return View();
        }
        public ActionResult FixedGroupInfoUI()
        {
            return View();
        }
        public ActionResult Start()
        {
            Response.Redirect("FixedRollCallUI");
            return View();
        }

        public ActionResult FixedRollCallUI()
        {
            return View();
        }
        public ActionResult CheckListTmp()
        {
            Response.Redirect("RollCallListUI");
            return View();
        }
        public ActionResult RollCallListUI()
        {
            return View();
        }
        public ActionResult End()
        {
            Response.Redirect("FixedEndRollCallUI");
            return View();
        }
        public ActionResult FixedEndRollCallUI()
        {
            return View();
        }
        public ActionResult CheckList()
        {
            Response.Redirect("FixedRollCallEndUI1");
            return View();
        }
        public ActionResult FixedRollCallEndUI1()
        {
            return View();
        }
        public ActionResult GroupInfoUI()
        {
            return View();
        }
        public ActionResult RandomRollStartCallUI()
        {
            return View();
        }
        public ActionResult RandomEndRollCallUI()
        {
            return View();
        }
        public ActionResult RandomRollCallUI()
        {
            return View();
        }
    }
}