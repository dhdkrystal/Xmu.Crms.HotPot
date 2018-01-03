using Microsoft.AspNetCore.Mvc;

namespace Xmu.Crms.Web.Insomnia.Controllers.Student
{
    
    public class StudentController : Controller
    {
        private const string PREFIX = "/desktop";
        [Route(PREFIX + "/Student")]
        public IActionResult StudentHomePage()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Modify")]
        public IActionResult StudentInfoModifyPage()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Course")]
        public IActionResult StudentCourseHome()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Choosecourse")]
        public IActionResult StudentChooseCoursePage()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Course/Courseinfo")]
        public IActionResult StudentCourseInformation()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Seminar/Fixed")]
        public IActionResult StudentSeminarPageFixed()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Seminar/Random")]
        public IActionResult StudentSeminarPageRandom()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Course/Managegroup")]
        public IActionResult StudentModifyGroupPage()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Seminar/Topic/Fixed")]
        public IActionResult StudentViewTopicPageFixed()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Seminar/Topic/Random")]
        public IActionResult StudentViewTopicPageRandom()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Seminar/Grade/Fixed")]
        public IActionResult StudentViewGradePage()
        {
            return View();
        }

        [Route(PREFIX + "/Student/Course/Group")]
        public IActionResult StudentViewGroupPage()
        {
            return View();
        }

    }
}