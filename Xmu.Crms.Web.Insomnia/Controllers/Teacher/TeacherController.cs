using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.Insomnia.Controllers.Teacher
{
   
    public class TeacherController : Controller
    {
        private const string PREFIX = "/desktop";
        [Route(PREFIX + "/Teacher")]
        public IActionResult TeacherHomePage()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Modify")]
        public IActionResult TeacherInfoModifyPage()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Course")]
        public IActionResult TeacherCourseHomePage()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Course/Courseinfo")]
        public IActionResult TeacherCourseInformation()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Course/Create")]
        public IActionResult TeacherCreateCoursePage()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Class")]
        public IActionResult TeacherClassInfo()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Class/Modify")]
        public IActionResult TeacherUpdateClass()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Class/Create")]
        public IActionResult TeacherCreateClass()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Seminar")]
        public IActionResult TeacherSeminarInfo()
        {
            return View("TeacherSenimarInfo");
        }

        [Route(PREFIX + "/Teacher/Seminar/Update")]
        public IActionResult TeacherUpdateSeminar()
        {
            return View("TeacherUpdateSenimar");
        }

        [Route(PREFIX + "/Teacher/Seminar/Create")]
        public IActionResult TeacherCreateSeminar()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Topic")]
        public IActionResult TeacherTopicInfo()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Topic/Update")]
        public IActionResult TeacherUpdateTopic()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Topic/Create")]
        public IActionResult TeacherCreateTopic()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Seminar/Score")]
        public IActionResult TeacherScoreHome()
        {
            return View();
        }

        [Route(PREFIX + "/Teacher/Seminar/GroupReport")]
        public IActionResult TeacherScoreReportPage()
        {
            return View();
        }
    }
}