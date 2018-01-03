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

namespace Xmu.Crms.Insomnia
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClassController : Controller
    {
        private readonly CrmsContext _db;
        private readonly ICourseService _courseService;
        private readonly IClassService _classService;
        private readonly IUserService _userService;
        private readonly IFixGroupService _fixGroupService;
        private readonly ISeminarService _seminarService;
      

        public ClassController(CrmsContext db, ICourseService courseService, IClassService classService, 
            IUserService userService, IFixGroupService fixGroupService,
            ISeminarService seminarService)
        {
            _db = db;
            _courseService = courseService;
            _classService = classService;
            _userService = userService;
            _fixGroupService = fixGroupService;
            _seminarService = seminarService;
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/class")]
        public IActionResult GetUserClasses([FromQuery] string courseName, [FromQuery] string courseTeacher)
        {
            //List<ClassInfo> classes = new List<ClassInfo>();
            try
            {
                IList<ClassInfo> classes;
                if (string.IsNullOrEmpty(courseName) && string.IsNullOrEmpty(courseTeacher))
                {
                    classes = _classService.ListClassByUserId(User.Id());
                } else if (string.IsNullOrEmpty(courseTeacher))
                {
                    classes = _courseService.ListClassByCourseName(courseName);
                }
                else if (string.IsNullOrEmpty(courseName))
                {
                    classes = _courseService.ListClassByTeacherName(courseTeacher);
                }else
                {
                    var c = _courseService.ListClassByCourseName(courseName).ToHashSet();
                    c.IntersectWith(_courseService.ListClassByTeacherName(courseTeacher));
                    classes = c.ToList();
                }

                return Json(classes.Select(c => new { id = c.Id, name = c.Name, site = c.Site, time = c.ClassTime,
                    courseId = c.CourseId,
                    courseName = _courseService.GetCourseByCourseId(c.CourseId).Name,
                    courseTeacher = _userService.GetUserByUserId(_courseService.GetCourseByCourseId(c.CourseId).TeacherId).Name,
                    numStudent = _db.Entry(c).Collection(cl => cl.CourseSelections).Query().Count()
                }));
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "用户ID输入格式错误"});
            }
            
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/class/{classId:long}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetClassById([FromRoute] long classId)
        {
            try
            {
                var cls = _classService.GetClassByClassId(classId);
                var sems = _seminarService.ListSeminarByCourseId(cls.CourseId).FirstOrDefault(s => (_classService.GetCallStatusById(s.Id, cls.Id)?.Status ?? 0) == 1);
                return Json(new { 
                    id = cls.Id, 
                    name = cls.Name, 
                    time = cls.ClassTime, 
                    site = cls.Site, 
                    courseId = cls.CourseId,
                    calling = sems?.Id ?? -1, 
                    proportions = new
                    {
                        report = cls.ReportPercentage, 
                        presentation = cls.PresentationPercentage,
                        c = cls.ThreePointPercentage,
                        b = cls.FourPointPercentage,
                        a = cls.FivePointPercentage
                    }

                });

            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "班级ID输入格式有误"});
            }
        }

        [HttpDelete(API.Insomnia.Constant.PREFIX + "/class/{classId:long}")]
        public IActionResult DeleteClassById([FromRoute] long classId)
        {
            try
            {
                var userlogin = _userService.GetUserByUserId(User.Id());
                if (userlogin.Type == Shared.Models.Type.Teacher)
                {
                    _classService.DeleteClassByClassId(classId);
                    return NoContent();
                }
                return StatusCode(403, new {msg = "权限不足"});
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new {msg = "班级不存在"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "班级ID输入格式有误"});
            }
            
        }

        [HttpPut(API.Insomnia.Constant.PREFIX + "/class/{classId:long}")]
        public IActionResult UpdateClassById([FromRoute] long classId, [FromBody] ClassWithProportions updated)
        {
            try
            {
                var userlogin = _userService.GetUserByUserId(User.Id());
                if (userlogin.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new {msg = "权限不足"});
                }
                _classService.UpdateClassByClassId(classId, new ClassInfo
                {
                    Id = classId,
                    Name = updated.Name,
                    ClassTime = updated.Time,
                    Site = updated.Site,
                    ThreePointPercentage = updated.Proportions.C,
                    FourPointPercentage = updated.Proportions.B,
                    FivePointPercentage= updated.Proportions.A,
                    ReportPercentage = updated.Proportions.Report,
                    PresentationPercentage = updated.Proportions.Presentation
                });
                return NoContent();
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new {msg = "班级不存在"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "班级ID输入格式有误"});
            }
            
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/class/{classId:long}/student")]
        public IActionResult GetStudentsByClassId([FromRoute] long classId,[FromQuery] string numBeginWith, string nameBeginWith)
        {
            try
            {
                var users = _userService.ListUserByClassId(classId, numBeginWith, nameBeginWith);
                return Json(users.Select(u => new {id = u.Id, name = u.Name, number = u.Number}));
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new {msg = "班级格式输入有误"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "班级ID输入格式有误"});
            }
            //return Json(new List<ClassInfo>());
        }

        [HttpPost(API.Insomnia.Constant.PREFIX + "/class/{classId:long}/student")]
        public IActionResult SelectClass([FromRoute] long classId, [FromBody] UserInfo student)
        {
            try
            {
                var user = student;
                var userlogin = _userService.GetUserByUserId(User.Id());
                if (userlogin.Type == Shared.Models.Type.Student)
                {
                    if (User.Id() == student.Id)
                    {
                        _classService.InsertCourseSelectionById(student.Id, classId);
                        return Created($"/class/{classId}/student/{student.Id}",
                            new Dictionary<string, string> {["url"] = $"/class/{classId}/student/{student.Id}"});
                    }
                    return StatusCode(403, new {msg = "学生无法为他人选课"});
                }
                return StatusCode(403, new {msg = "权限不足"});
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new {msg = "班级不存在"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "班级ID输入格式有误"});
            }
        }

        [HttpDelete(API.Insomnia.Constant.PREFIX + "/class/{classId:long}/student/{studentId:long}")]
        public IActionResult DeselectClass([FromRoute] long classId, [FromRoute] long studentId)
        {
            try
            {
                var userlogin = _userService.GetUserByUserId(User.Id());
                if (userlogin.Type == Shared.Models.Type.Student)
                {
                    if (studentId == User.Id())
                    {
                        _classService.DeleteCourseSelectionById(studentId, classId);
                        return NoContent();
                    }
                    return StatusCode(403, new {msg = "用户无法为他人退课"});
                }
                return StatusCode(403, new {msg = "权限不足"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "错误的ID格式"});
            }
           
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/class/{classId}/classgroup")]
        public IActionResult GetUserClassGroupByClassId([FromRoute] long classId)
        {
            try
            {
                var userlogin = _userService.GetUserByUserId(User.Id());
                if (userlogin.Type != Shared.Models.Type.Student)
                {
                    return StatusCode(403, new {msg = "权限不足"});
                }

                var fixGroup = _fixGroupService.GetFixedGroupById(User.Id(), classId);
                var leader = fixGroup.Leader ?? _userService.GetUserByUserId(fixGroup.LeaderId??0);
                var members = _fixGroupService.ListFixGroupMemberByGroupId(fixGroup.Id);
                var result = Json(
                    new
                    {
                        leader = new
                        {
                            id = leader.Id,
                            name = leader.Name,
                            number = leader.Number
                        },
                        members = members.Select(m => new
                        {
                            id = m.Id,
                            name = m.Name,
                            number = m.Number
                        })
                    });
                return result;
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "课程ID格式错误"});
            }
        }

        /*
         * 这一部分删去，增加了新的方法
         */
        //[HttpPut("/class/{classId}/classgroup")]
        //public IActionResult UpdateUserClassGroupByClassId([FromRoute] long classId, [FromBody] FixGroup updated)
        //{
        //    try
        //    {
                
        //        return NoContent();
        //    }
        //    catch (ClassNotFoundException)
        //    {
        //        return StatusCode(404, new {msg = "不存在当前班级"});
        //    }
        //    catch (ArgumentException)
        //    {
        //        return StatusCode(400, new {msg = "班级ID格式有误"});
        //    }
        //}

        /*
         * 以下的四个controller为新添加的controller
         * 前两个方法模块组的同学说不会被调用，先不写
         */
        [HttpPut(API.Insomnia.Constant.PREFIX + "/class/{classId}/classgroup/resign")]
        public IActionResult GroupLeaderResignByClassId([FromRoute] long classId, [FromBody] UserInfo student)
        {
            try
            {
                //var groupId = _fixGroupService.GetFixedGroupById()
                //_seminarGroupService.ResignLeaderById
                return NoContent();
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new {msg = "不存在当前班级"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "班级ID格式错误"});
            }
        }

        [HttpPut(API.Insomnia.Constant.PREFIX + "/class/{classId}/classgroup/assign")]
        public IActionResult GroupLeaderAssignByClassId([FromRoute] long classId, [FromBody] UserInfo student)
        {
            try
            {
                //var groupId = _fixGroupService.GetFixedGroupById()
                //_seminarGroupService.ResignLeaderById
                return NoContent();
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "不存在当前班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "班级ID格式错误" });
            }
        }

        [HttpPut(API.Insomnia.Constant.PREFIX + "/class/{classId}/classgroup/add")]
        public IActionResult AddGroupMemberByClassId([FromRoute] long classId, [FromBody] UserInfo student)
        {
            try
            {
                var group = _fixGroupService.GetFixedGroupById(User.Id(), classId);
                _fixGroupService.InsertStudentIntoGroup(student.Id, group.Id);
                return NoContent();
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "不存在当前班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "班级ID格式错误" });
            }
        }

        [HttpPut(API.Insomnia.Constant.PREFIX + "/class/{classId}/classgroup/remove")]
        public IActionResult RemoveGroupMemberByClassId([FromRoute] long classId, [FromBody] UserInfo student)
        {
            try
            {
                var group = _fixGroupService.GetFixedGroupById(User.Id(), classId);
                _fixGroupService.DeleteFixGroupUserById(group.Id, student.Id);
                return NoContent();
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "不存在当前班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "班级ID格式错误" });
            }
        }
    }
}