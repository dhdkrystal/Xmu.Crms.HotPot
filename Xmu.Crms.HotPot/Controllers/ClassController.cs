using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Services.HotPot;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class ClassController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IClassService _classService;
        private readonly IFixGroupService _fixGroupService;
        private readonly IUserService _userService;

        public ClassController(IClassService classService, ICourseService courseService,
            IFixGroupService fixGroupService, IUserService userService)
        {
            _classService = classService;
            _courseService = courseService;
            _fixGroupService = fixGroupService;
            _userService = userService;
        }

        /// <summary>
        /// 获取与当前用户相关的或者符合条件的班级列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("/class")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUserClasses([FromBody]string courseName, [FromBody]string teacherName)
        {
            IList<ClassInfo> classes = new List<ClassInfo> { };
            //按课程名称获取班级列表.
            foreach (ClassInfo c in _courseService.ListClassByCourseName(courseName))
            {
                classes.Add(c);
            }
            //按教师名称获取班级列表
            foreach (ClassInfo c in _courseService.ListClassByCourseName(teacherName))
            {
                classes.Add(c);
            }
            //按课程名称和教师名称获得班级列表
            foreach (ClassInfo c in _courseService.ListClassByName(courseName, teacherName))
            {
                classes.Add(c);
            }
            return Json(classes);
        }


        /// <summary>
        /// 按ID获取班级详情
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId:long}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetClassById([FromRoute] long classId)
        {
            try
            {
                var c2 = _classService.GetClassByClassId(classId);
                return Json(c2);
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
        }

        /// <summary>
        /// 按ID修改班级
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        [HttpPut("/class/{classId:long}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateClassById([FromRoute] long classId, [FromBody] ClassInfo updated)
        {
            if (User.Type() != Type.Teacher)
            {
                return StatusCode(403, new { msg = "权限不足" });
            }
            try
            {
                _classService.UpdateClassByClassId(classId, updated);
                return NoContent();
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
        }

        /// <summary>
        /// 按ID删除班级
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpDelete("/class/{classId:long}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult DeleteClassById([FromRoute] long classId)
        {
            if (User.Type() != Type.Teacher)
            {
                return StatusCode(403, new { msg = "权限不足" });
            }
            try
            {
                _classService.DeleteClassByClassId(classId);
                return NoContent();
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
        }

        /// <summary>
        /// 按班级ID查找学生列表（查询学号、姓名开头）
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId:long}/student")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetStudentsByClassId([FromRoute] long classId, [FromBody]string numBeginWith,
            [FromBody] string nameBeginWith)
        {
            IList<UserInfo> students = _userService.ListUserByClassId(classId, numBeginWith, nameBeginWith);
            return Json(students);
        }

        /// <summary>
        /// 学生按ID选择班级
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost("/class/{classId:long}/student")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult SelectClass([FromRoute] long classId, [FromBody] UserInfo student)
        {
            _classService.InsertCourseSelectionById(student.Id, classId);
            return Created("/class/{classId}/student/1", new Dictionary<string, string> { ["url"] = " /class/{classId}/student/1" });
        }

        /// <summary>
        /// 学生按ID取消选择班级
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpDelete("/class/{classId:long}/student/{studentId:long}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult DeselectClass([FromRoute] long classId, [FromRoute] long studentId)
        {
            _classService.DeleteCourseSelectionById(studentId, classId);
            return NoContent();
        }

        /// <summary>
        /// 按ID获取班级签到状态
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="showPresent"></param>
        /// <param name="showLate"></param>
        /// <param name="showAbsent"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId:long}/attendance")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAttendanceByClassId([FromRoute] long classId, [FromBody]long seminarId,
            [FromBody] bool showPresent,
            [FromBody]bool showLate, [FromBody] bool showAbsent)
        {
            if (showPresent)
            {
                IList<UserInfo> presentList = _userService.ListPresentStudent(seminarId, classId);
                return Json(presentList);
            }
            if (showLate)
            {
                IList<UserInfo> lateList = _userService.ListLateStudent(seminarId, classId);
                return Json(lateList);
            }
            if (showAbsent)
            {
                IList<UserInfo> absentList = _userService.ListAbsenceStudent(seminarId, classId);
                return Json(absentList);
            }
            return Json(new List<ClassInfo>());
        }

        /// <summary>
        /// 签到（上传位置信息）
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        [HttpPut("/class/{classId:long}/attendance/{studentId:long}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateAttendanceByClassId([FromRoute] long classId, [FromBody]long seminarId,
            [FromRoute] long studentId, [FromBody] Location loc)
        {
            try
            {
                _userService.InsertAttendanceById(classId, seminarId, studentId, (double)loc.Longitude, (double)loc.Latitude);
                return NoContent();
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "不存在这个学生或班级" });
            }
        }

        /// <summary>
        /// 按ID获取班级小组
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId}/classgroup")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUserClassGroupByClassId([FromRoute] long classId)
        {

            IList<FixGroup> groups = _fixGroupService.ListFixGroupByClassId(classId);
            return Json(groups);
        }

        /// <summary>
        /// 按ID修改班级小组
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpPut("/class/{classId}/classgroup")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateUserClassGroupByClassId([FromRoute] long classId, [FromBody]long groupId,
            [FromBody]FixGroup fixGroupBo)
        {
            _fixGroupService.UpdateFixGroupByGroupId(groupId, fixGroupBo);
            return NoContent();
        }
    }
}