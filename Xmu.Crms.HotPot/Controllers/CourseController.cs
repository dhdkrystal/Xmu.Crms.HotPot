using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class CourseController : Controller
    {/*
        [HttpGet("/course")]
        public IActionResult GetUserCourses()
        {
            var c1 = new Course
            {
                Name = "OOAD",
                Description = "Description"
            };
            var c2 = new Course
            {
                Name = "J2EE",
                Description = "Description"
            };
            return Json(new List<Course> { c1, c2});
        }

        [HttpPost("/course")]
        public IActionResult CreateCourse([FromBody] Course newCourse)
        {
            return Created("/course/1", new {id = 1});
        }

        [HttpGet("/course/{courseId:long}")]
        public IActionResult GetCourseById([FromRoute] long courseId)
        {
            var c1 = new Course
            {
                Name = "OOAD",
                Description = "Description"
            };
            var c2 = new Course
            {
                Name = "J2EE",
                Description = "Description"
            };
            return Json(courseId == 0 ? c1 : c2);
        }

        [HttpDelete("/course/{courseId:long}")]
        public IActionResult DeleteCourseById([FromRoute] long courseId)
        {
            return NoContent();
        }

        [HttpPut("/course/{courseId:long}")]
        public IActionResult UpdateCourseById([FromRoute] long courseId, [FromBody] Course updated)
        {
            return NoContent();
        }

        [HttpGet("/course/{courseId:long}/class")]
        public IActionResult GetClassesByCourseId([FromRoute] long courseId)
        {
            var c1 = new ClassInfo
            {
                Name = "周三1-2"
            };
            var c2 = new ClassInfo
            {
                Name = "周三910"
            };
            return Json(new List<ClassInfo> {c1, c2});
        }

        [HttpPost("/course/{courseId:long}/class")]
        public IActionResult CreateClassByCourseId([FromRoute] long courseId, [FromBody] ClassInfo newClass)
        {
            return Created("/class/1", new { id = 1 });
        }

        [HttpGet("/course/{courseId:long}/seminar")]
        public IActionResult GetSeminarsByCourseId([FromRoute] long courseId)
        {
            var s1 = new Seminar();
            var s2 = new Seminar();
            return Json(new List<Seminar>{s1, s2});
        }

        [HttpPost("/course/{courseId:long}/seminar")]
        public IActionResult CreateSeminarByCourseId([FromRoute] long courseId, [FromBody] Seminar newSeminar)
        {
            return Created("/seminar/1", new { id = 1 });
        }

        [HttpGet("/course/{courseId:long}/grade")]
        public IActionResult GetGradeByCourseId([FromRoute] long courseId)
        {
            var gd1 = new StudentScoreGroup();
            var gd2 = new StudentScoreGroup();
            return Json(new List<StudentScoreGroup> {gd1, gd2});
        }
        */
    }
    
}