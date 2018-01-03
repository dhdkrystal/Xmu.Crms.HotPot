using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using System.Linq;
using Xmu.Crms.Shared.Exceptions;
using Type = Xmu.Crms.Shared.Models.Type;

namespace Xmu.Crms.Insomnia
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IClassService _classService;
        private readonly IUserService _userService;
        private readonly IFixGroupService _fixGroupService;
        private readonly ISeminarGroupService _seminarGroupService;
        private readonly ISeminarService _seminarService;

        private readonly CrmsContext _db;

        public CourseController(ICourseService courseService, IClassService classService,
            IUserService userService, IFixGroupService fixGroupService,
            ISeminarGroupService seminarGroupService,
            ISeminarService seminarService, CrmsContext db)
        {
            _courseService = courseService;
            _classService = classService;
            _userService = userService;
            _fixGroupService = fixGroupService;
            _seminarGroupService = seminarGroupService;
            _seminarService = seminarService;
            _db = db;
        }

        /*
         * 无法计算每个课程里面学生的人数，需要多表联合查询，查询难度非常大
         * 缺少班级总人数字段
         */
        [HttpGet(API.Insomnia.Constant.PREFIX + "/course")]
        public IActionResult GetUserCourses()
        {
            var userlogin = _userService.GetUserByUserId(User.Id());
            if (userlogin.Type != Type.Teacher)
            {
                return StatusCode(403, new {msg = "权限不足"});
            }

            var courses = _courseService.ListCourseByUserId(User.Id());
            return Json(courses.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                numClass = _classService.ListClassByCourseId(c.Id).Count,
                numStudent = _classService.ListClassByCourseId(c.Id).Aggregate(0, (total, cls) => _db.Entry(cls).Collection(cl => cl.CourseSelections).Query().Count() + total),
                startTime = c.StartDate.ToString("yyyy-MM-dd"),
                endTime = c.EndDate.ToString("yyyy-MM-dd"),
            }));
        }

        [HttpPost(API.Insomnia.Constant.PREFIX + "/course")]
        public IActionResult CreateCourse([FromBody] CourseWithProportions newCourse)
        {
            var userlogin = _userService.GetUserByUserId(User.Id());
            if (userlogin.Type != Type.Teacher)
            {
                return StatusCode(403, new {msg = "权限不足"});
            }

            var id = _courseService.InsertCourseByUserId(User.Id(), new Course
            {
                Name = newCourse.Name,
                Description = newCourse.Description,
                StartDate = newCourse.StartTime,
                EndDate = newCourse.EndTime,
                ThreePointPercentage = newCourse.Proportions.C,
                FourPointPercentage = newCourse.Proportions.B,
                FivePointPercentage = newCourse.Proportions.A,
                ReportPercentage = newCourse.Proportions.Report,
                PresentationPercentage = newCourse.Proportions.Presentation,
                TeacherId = User.Id()
            });
            return Created($"/course/{id}", new { id });

        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/course/{courseId:long}")]
        public IActionResult GetCourseById([FromRoute] long courseId)
        {
            try
            {
                var course = _courseService.GetCourseByCourseId(courseId);
                var result = Json(new
                {
                    id = course.Id,
                    name = course.Name,
                    description = course.Description,
                    teacherName = course.Teacher.Name,
                    teacherEmail = course.Teacher.Email
                });
                return result;
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        [HttpDelete(API.Insomnia.Constant.PREFIX + "/course/{courseId:long}")]
        public IActionResult DeleteCourseById([FromRoute] long courseId)
        {
            try
            {
                var userlogin = _userService.GetUserByUserId(User.Id());
                if (userlogin.Type == Type.Teacher)
                {
                    _courseService.DeleteCourseByCourseId(courseId);
                    return NoContent();
                }
                return StatusCode(403, new { msg = "权限不足" });
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "课程ID格式错误" });
            }
        }

        [HttpPut(API.Insomnia.Constant.PREFIX + "/course/{courseId:long}")]
        public IActionResult UpdateCourseById([FromRoute] long courseId, [FromBody] Course updated)
        {
            try
            {
                var userlogin = _userService.GetUserByUserId(User.Id());
                if (userlogin.Type == Type.Teacher)
                {
                    _courseService.UpdateCourseByCourseId(courseId, updated);
                    return NoContent();
                }
                return StatusCode(403, new { msg = "权限不足" });
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "课程ID格式错误" });
            }
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/course/{courseId:long}/class")]
        public IActionResult GetClassesByCourseId([FromRoute] long courseId)
        {
            try
            {
                var classes = _classService.ListClassByCourseId(courseId);
                return Json(classes.Select(c => new
                {
                    id = c.Id,
                    name = c.Name
                }));
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "课程ID格式错误" });
            }
        }

        [HttpPost(API.Insomnia.Constant.PREFIX + "/course/{courseId:long}/class")]
        public IActionResult CreateClassByCourseId([FromRoute] long courseId, [FromBody] ClassWithProportions newClass)
        {
            var userlogin = _userService.GetUserByUserId(User.Id());
            if (userlogin.Type != Type.Teacher)
            {
                return StatusCode(403, new {msg = "权限不足"});
            }

            var classId = _courseService.InsertClassById(courseId, new ClassInfo
            {
                Name = newClass.Name,
                ClassTime = newClass.Time,
                Site = newClass.Site,
                ThreePointPercentage = newClass.Proportions.C,
                FourPointPercentage = newClass.Proportions.B,
                FivePointPercentage = newClass.Proportions.A,
                ReportPercentage = newClass.Proportions.Report,
                PresentationPercentage = newClass.Proportions.Presentation
            });
            return Created($"/class/{classId}", new { id = classId });
        }

        /*
         * 这里新增了一个FromBody的embededGrade的参数，用于判断是否已经打分
         */
        [HttpGet(API.Insomnia.Constant.PREFIX + "/course/{courseId:long}/seminar")]
        public IActionResult GetSeminarsByCourseId([FromRoute] long courseId, [FromQuery] bool embededGrade)
        {
            try
            {
                var seminars = _seminarService.ListSeminarByCourseId(courseId);
                if (!embededGrade)
                {
                    return Json(seminars.Select(s => new
                    {
                        id = s.Id,
                        name = s.Name,
                        description = s.Description,
                        groupingMethod = (s.IsFixed == true) ? "fixed" : "random",
                        startTime = s.StartTime.ToString("YYYY-MM-dd"),
                        endTime = s.EndTime.ToString("YYYY-MM-dd")
                    }));
                }
                return Json(seminars.Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    description = s.Description,
                    groupingMethod = (s.IsFixed == true) ? "fixed" : "random",
                    startTime = s.StartTime.ToString("YYYY-MM-dd"),
                    endTime = s.EndTime.ToString("YYYY-MM-dd"),
                    grade = _seminarGroupService.GetSeminarGroupById(s.Id, User.Id()).FinalGrade
                }));
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "课程ID格式错误" });
            }
        }

        [HttpPost(API.Insomnia.Constant.PREFIX + "/course/{courseId:long}/seminar")]
        public IActionResult CreateSeminarByCourseId([FromRoute] long courseId, [FromBody] Seminar newSeminar)
        {
            var userlogin = _userService.GetUserByUserId(User.Id());
            if (userlogin.Type == Type.Teacher)
            {
                var seminarId = _seminarService.InsertSeminarByCourseId(courseId, newSeminar);
                return Created($"/seminar/{seminarId}", new { id = seminarId });
            }
            return StatusCode(403, new { msg = "权限不足" });
        }

        /*
         * 这里用了一个foreach，但是实际用途缺不是很大。获得当前班级对应的所有讨论课信息，这个条件是基于时间的，即基于讨论课的讨论课组信息。
         * 不同的讨论课的组分数信息没有办法也不应该放在一起展示，这一点很关键。所以本Controller是直接用foreach方法List调用来完成的。
         * 已经反馈给模块组，不过也不知道改不改得了了。
         */
        [HttpGet(API.Insomnia.Constant.PREFIX + "/course/{courseId:long}/grade")]
        public IActionResult GetGradeByCourseId([FromRoute] long courseId)
        {
            try
            {
                var seminarGroups = _seminarGroupService.ListSeminarGroupIdByStudentId(User.Id());
                return Json(seminarGroups.Select(s => new
                {
                    seminarName = _seminarService.GetSeminarBySeminarId(s.SeminarId).Name,
                    groupName = s.Id + "组",//这里还是没有组名的问题
                    leaderName = _userService.GetUserByUserId(s.LeaderId??0).Name,
                    presentationGrade = s.PresentationGrade,
                    reportGrade = s.ReportGrade,
                    grade = s.FinalGrade
                }));
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "课程ID格式错误" });
            }
        }
    }

    public class CourseWithProportions
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Proportions Proportions { get; set; }
    }


    public class ClassWithProportions
    {
        public string Name { get; set; }

        public string Site { get; set; }

        public string Time { get; set; }

        public Proportions Proportions { get; set; }
    }

    public class Proportions
    {
        public int Report { get; set; }
        public int Presentation { get; set; }
        public int C { get; set; }
        public int B { get; set; }
        public int A { get; set; }
    }
}