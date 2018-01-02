using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;

//Writen and tested by FJL

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseservice;
        private readonly IClassService _classservice;
        private readonly ISeminarService _seminarservice;
        private readonly IUserService _userservice;
        private readonly ISeminarGroupService _seminargroupservice;
        private CrmsContext context;
        private JwtHeader _head;
        //定义JWT的head
        public CourseController(ICourseService service, IClassService service1, ISeminarService service2, IUserService service3, ISeminarGroupService seminar4, CrmsContext db, JwtHeader head)
        {
            _courseservice = service;
            _classservice = service1;
            _seminarservice = service2;
            _userservice = service3;
            _seminargroupservice = seminar4;
            this.context = db;
            _head = head;
        }


        [HttpGet("/course")]//JWT保存token测试。直接获取用JWt存储的token的head获取User.Id()
        public IActionResult GetUserCourses()
        {
            try
            {
                IList<Course> il = _courseservice.ListCourseByUserId(User.Id());
                foreach (Course i in il)
                {

                    UserInfo us = context.UserInfo.Find(i.TeacherId);
                    i.Teacher = us;
                }
                return Json(il);
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "用户所包含课程不存在!" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "userID非法！！" });
            }

        }
        [HttpPost("/course")]//测试成功
        public IActionResult CreateCourse([FromBody] Course newCourse)
        {
            try
            {
                var id = _courseservice.InsertCourseByUserId(User.Id(), newCourse);//此处的1
                return Created($"/course/{id}", new { id });
            }
            catch (ArgumentException)
            {
                return StatusCode(403, new { msg = "非法用户" });
            }
        }

        [HttpGet("/course/{courseId:long}")]//测试成功
        public IActionResult GetCourseBycourseId([FromRoute] long courseId)
        {
            try
            {
                Course cou = _courseservice.GetCourseByCourseId(courseId);
                return Json(cou);
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "该课程不存在!" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "格式错误!" });
            }
        }

        [HttpDelete("/course/{courseId:long}")]//测试成功
        public IActionResult DeleteCourseById([FromRoute] long courseId)
        {
            try
            {
                _courseservice.DeleteCourseByCourseId(courseId);
                return StatusCode(200, new { msg = "删除成功！" });
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "course不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "格式错误！" });
            }
        }

        [HttpPut("/course/{courseId:long}")]//测试成功
        public IActionResult UpdateCourseById([FromRoute] long courseId, [FromBody] Course updated)
        {
            try
            {
                _courseservice.UpdateCourseByCourseId(courseId, updated);
                return StatusCode(200, new { msg = "更新成功！" });
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "course不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "格式错误！" });
            }

        }

        [HttpGet("/course/{courseId:long}/class")]//测试成功
        public IActionResult GetClassesByCourseName([FromRoute] long courseId)
        {
            try
            {
                IList<ClassInfo> il = _classservice.ListClassByCourseId(courseId);
                return Json(il);
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "该课程包含的班级不存在!" });
            }
        }


        [HttpPost("/course/{courseId:long}/class")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CreateClassByCourseId([FromRoute] long courseId, [FromBody] ClassInfo newClass)
        {
            try
            {
                var userlogin = _userservice.GetUserByUserId(User.Id());
                if (userlogin.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }

                if (newClass.PresentationPercentage + newClass.ReportPercentage != 100
                    || newClass.ThreePointPercentage + newClass.FourPointPercentage + newClass.FivePointPercentage != 100)
                    return StatusCode(422, new { msg = "比例失调" });

                if (newClass.ThreePointPercentage < 0 || newClass.FourPointPercentage < 0 || newClass.FivePointPercentage < 0
                    || newClass.PresentationPercentage < 0 || newClass.ReportPercentage < 0)
                    return StatusCode(422, new { msg = "比例不在正确范围内" });

                var classId = _courseservice.InsertClassById(courseId, newClass);

                return Created($"/class/{classId}", new { id = classId });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "格式错误" });
            }
        }



        [HttpGet("/course/{courseId:long}/seminar")]//测试成功！
        public IActionResult GetSeminarsByCourseId([FromRoute] long courseId)
        {
            try
            {
                IList<SeminarInfo> seminars = new List<SeminarInfo>();
                IList<Seminar> il = _seminarservice.ListSeminarByCourseId(courseId);
                
            
                foreach (var i in il)
                {
                    ClassInfo cla = _classservice.GetClassByUserIdAndSeminarId(User.Id(),i.Id);
                    Course cou = context.Course.Find(courseId);
                    i.Course.Name = cou.Name;
                    SeminarInfo seminar = new SeminarInfo(){
                        Seminar=i,
                        ClassId=cla.Id
                    };
                    seminars.Add(seminar);
                }
                return Json(seminars);
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "该课程所包含的讨论课不存在" });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "该课程所包含的班级不存在" });
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "该课程不存在" });
            }
        }

        [HttpPost("/course/{courseId:long}/seminar")]//测试成功
        public IActionResult CreateSeminarByCourseId([FromRoute] long courseId, [FromBody] Seminar newSeminar)
        {
            try
            {
                var id = _seminarservice.InsertSeminarByCourseId(courseId, newSeminar);
                return Created($"/seminar/{id}", new { id });
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "course不存在的" });
            }
            catch (ArgumentException)
            {
                return StatusCode(405, new { msg = "参数错误" });
            }
        }

        [HttpGet("/course/{courseId}/seminar/current")]//测试成功
        //既定当前时间段至多有一个讨论课仍未结束
        public IActionResult GetCurrentSeminarBycourseId([FromRoute] long courseId)
        {
            try
            {
                var course = _courseservice.GetCourseByCourseId(courseId);
                IList<Seminar> sem = _seminarservice.ListSeminarByCourseId(courseId);
                Seminar seminar = null;
                foreach (Seminar i in sem)
                {
                    if (DateTime.Compare(DateTime.Now, i.StartTime) * DateTime.Compare(i.EndTime, DateTime.Now) >= 0)
                    {
                        seminar = i;
                        Course cou = context.Course.Find(courseId);
                        seminar.Course.Name = cou.Name;
                    }
                }
                return Json(new {
                    coursename = course.Name,
                    courseId=course.Id,
                    currentseminar=seminar
                });
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "course不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "格式错误!" });
            }
        }

        [HttpGet("/course/{courseId:long}/grade")]//测试成功
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetGradeByCourseId([FromRoute] long courseId)
        {
            Grade gg = new Grade();
            try
            {
                IList<Seminar> ils = _seminarservice.ListSeminarByCourseId(courseId);
                foreach (var i in ils)
                {
                    SeminarGroup sm = _seminargroupservice.GetSeminarGroupById(i.Id, User.Id());//这里这里
                    gg.grade = sm.FinalGrade;
                    gg.name = i.Name;
                    gg.description = i.Description;
                    break;
                }
                return Json(gg);
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "学生不属于任何组啊" });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "该courseId下无任何讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "格式错误！" });
            }
        }

        public class Grade//用于输出学生成绩？
        {
            public int? grade { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
        public class SeminarInfo
        {
            public Seminar Seminar { get; set; }
            public long ClassId { get;set ;}
        }

    }
}