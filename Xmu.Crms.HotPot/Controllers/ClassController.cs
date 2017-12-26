using Xmu.Crms.Shared.Models;
using Xmu.Crms.Services.HotPot;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;

namespace Xmu.Crms.HotPot.Controllers
{
    /*
    [Route("")]
    [Produces("application/json")]
    public class ClassController : Controller
    {
        private IClassService classService;
        private IFixGroupService fixGroupService;
        private IUserService userService;
        private readonly JwtHeader  _head;

        public ClassController(IClassService classService,IFixGroupService fixGroupService,
            IUserService userService, JwtHeader header)
        {
            this.classService = classService;
            this.fixGroupService = fixGroupService;
            this.userService = userService;
            this._head=header;
        }

        /// <summary>
        /// 获取与当前用户相关的或者符合条件的班级列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("/class")]
        public IActionResult GetUserClasses([FromBody]string courseName,[FromBody]string teacherName)
        {
            var c1 = new ClassInfo
            {
                Name= "周三1-2节",
                Site="公寓405"
            };
            var c2 = new ClassInfo
            {
                Name = "一班",
                Site = "海韵202"
            };
            return Json(new List<ClassInfo> {c1, c2});
        }


        /// <summary>
        /// 按ID获取班级详情
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId:long}")]
        public IActionResult GetClassById([FromRoute] long classId)
        {
            try
            {
                var c2 = classService.GetClassByClassId(classId);
                return Json(c2);
            }catch(ClassNotFoundException)
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
        public IActionResult UpdateClassById([FromRoute] long classId, [FromBody] ClassInfo updated)
        {
            try
            {
                classService.UpdateClassByClassId(classId, updated);
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
        public IActionResult DeleteClassById([FromRoute] long classId)
        {
            try
            {
                classService.DeleteClassByClassId(classId);
                return NoContent();
            }catch(ClassNotFoundException)
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
        public IActionResult GetStudentsByClassId([FromRoute] long classId,[FromBody]string  numBeginWith,
            [FromBody] string nameBeginWith)
        {
            IList<UserInfo> students = userService.ListUserByClassId(classId,numBeginWith,nameBeginWith);
            return Json(students);
        }

        /// <summary>
        /// 学生按ID选择班级
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost("/class/{classId:long}/student")]
        public IActionResult SelectClass([FromRoute] long classId, [FromBody] UserInfo student)
        {
            classService.InsertCourseSelectionById(student.Id,classId);
            return Created("/class/{classId}/student/1", new Dictionary<string, string> {["url"] = " /class/1/student/1"});
        }

        /// <summary>
        /// 学生按ID取消选择班级
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpDelete("/class/{classId:long}/student/{studentId:long}")]
        public IActionResult DeselectClass([FromRoute] long classId, [FromRoute] long studentId)
        {
            classService.DeleteCourseSelectionById(studentId,classId);
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
        public IActionResult GetAttendanceByClassId([FromRoute] long classId,[FromBody]long seminarId,
            [FromBody] bool showPresent,
            [FromBody]bool showLate,[FromBody] bool showAbsent)
        {
            if (showPresent)
            {
                IList<UserInfo> presentList = userService.ListPresentStudent(seminarId,classId);
                return Json(presentList);
            }
            if(showLate)
            {
                IList<UserInfo> lateList = userService.ListLateStudent(seminarId,classId);
                return Json(lateList);
            }
            if(showAbsent)
            {
                IList<UserInfo> absentList = userService.ListAbsenceStudent(seminarId,classId);
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
        public IActionResult UpdateAttendanceByClassId([FromRoute] long classId,[FromBody]long seminarId,
            [FromRoute] long studentId,  [FromBody] Location loc)
        {
            try
            {
                userService.InsertAttendanceById(classId, seminarId, studentId, loc.Longitude, loc.Latitude);
                return NoContent();
            }catch(UserNotFoundException)
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
        public IActionResult GetUserClassGroupByClassId([FromRoute] long classId)
        {

            IList<FixGroup> groups = fixGroupService.ListFixGroupByClassId(classId);
            return Json(groups);
        }

        /// <summary>
        /// 按ID修改班级小组
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpPut("/class/{classId}/classgroup")]
        public IActionResult UpdateUserClassGroupByClassId([FromRoute] long classId,[FromBody]long groupId,
            [FromBody]FixGroup fixGroupBo)
        {
            fixGroupService.UpdateFixGroupByGroupId(groupId,fixGroupBo);
            return NoContent();
        }
    }
    */
}