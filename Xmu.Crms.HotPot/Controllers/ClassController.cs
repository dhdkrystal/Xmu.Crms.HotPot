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
using System;
using System.Linq;

namespace Xmu.Crms.HotPot.Controllers
{
    /*
    [Route("")]
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
        private readonly ISeminarGroupService _seminarGroupService;
        private readonly ITopicService _topicService;
        private JwtHeader _head;

        public ClassController(JwtHeader head,CrmsContext db, IClassService classService, ICourseService courseService,
            IFixGroupService fixGroupService, IUserService userService, ISeminarService seminarService,
            ISeminarGroupService seminarGroupService, ITopicService topicService)
        {
            _head = head;
            _db = db;
            _classService = classService;
            _courseService = courseService;
            _fixGroupService = fixGroupService;
            _userService = userService;
            _seminarService = seminarService;
            _seminarGroupService = seminarGroupService;
            _topicService = topicService;
        }
        /// <summary>
        /// 获取课程下的所有班级
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpGet("/course/{courseId:long}/classes")]
        public IActionResult GetCourseClasses([FromRoute]long courseId)
        {
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                if (user.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }
                IList<ClassInfo> classes = _classService.ListClassByCourseId(courseId);
                return Json(classes.Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    courseId = c.CourseId
                }));
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "课程不存在" });
            }

        }
        /// <summary>
        /// 获取班级信息
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("/classInfo/{classId}")]
        public IActionResult GetClassInfo([FromRoute]long classId)
        {
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                if (user.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }
                var classInfo = _classService.GetClassByClassId(classId);
                return Json(new
                {
                    id = classId,
                    classname = classInfo.Name,
                    numStudent = _db.Entry(classInfo).Collection(cl => cl.CourseSelections).Query().Count()

                });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "班级不存在" });
            }
        }
        /// <summary>
        /// 获取讨论课下班级的小组信息
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="seminarId"></param>
        /// <returns></returns>
        [HttpGet("/seminar/{seminarId:long}/group/{classId:long}")]
        public IActionResult GetGroupInfo([FromRoute]long classId, [FromRoute]long seminarId)
        {
            try
            {
                 var user = _userService.GetUserByUserId(User.Id());
             //   var user = _userService.GetUserByUserId(1);
                if (user.Type == Shared.Models.Type.Teacher)
                {
                    var seminar = _seminarService.GetSeminarBySeminarId(seminarId);
                    IList<SeminarGroup> groupss = _seminarGroupService.ListSeminarGroupBySeminarId(seminarId);
                    List<SeminarGroup> groups=new List<SeminarGroup>();
                    foreach(SeminarGroup g in groupss)
                    {
                        if (g.ClassId == classId)
                        {    
                            g.SeminarGroupTopics= _topicService.ListSeminarGroupTopicByGroupId(g.Id); ;
                            groups.Add(g);
                            
                        }
                            
                    }
                    return Json(groups.Select(g =>
                      new
                      {
                          groupId = g.Id,
                          topicname = g.SeminarGroupTopics.Select(t => new {
                                                serial=t.Topic.Serial,
                                               id=t.Topic.Id,
                                                 name=t.Topic.Name}),
                       members = _seminarGroupService.ListSeminarGroupMemberByGroupId(g.Id).Select(m => new
                       {
                           id = m.Id,
                           name = m.Name,
                           number = m.Number
                       })
                   }));
                }
                else
                {
                    return StatusCode(404, new { msg = "权限不足" });
                }
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
        }

        /// <summary>
        /// 获取与当前用户相关的或者符合条件的班级列表
        /// data:JSON.stringify({coueseName:"",teacherName:""})
        /// </summary>
        /// <returns></returns>
        [HttpGet("/class")]
        public IActionResult GetUserClasses([FromQuery] string courseName, [FromQuery] string teacherName)
        {
            try
            {
                IList<ClassInfo> classes;
                if (string.IsNullOrEmpty(courseName) && string.IsNullOrEmpty(teacherName))
                {
                    //按学生ID获取班级列表
                    //User.Claims.Single(c => c.Type == "id").Value)
                    classes = _classService.ListClassByUserId(User.Id());
                }
                else if (string.IsNullOrEmpty(teacherName))
                {
                    //按课程名称获取班级列表.
                    classes = _courseService.ListClassByCourseName(courseName);
                }
                else if (string.IsNullOrEmpty(courseName))
                {
                    //按教师名称获取班级列表
                    classes = _courseService.ListClassByTeacherName(teacherName);
                }
                else
                {
                    //按课程名称和教师名称获得班级列表
                    var c = _courseService.ListClassByCourseName(courseName).ToHashSet();
                    c.IntersectWith(_courseService.ListClassByTeacherName(teacherName));
                    classes = c.ToList();
                }
                return Json(classes.Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    site = c.Site,
                    time = c.ClassTime,
                    courseId = c.CourseId,
                    courseName = _courseService.GetCourseByCourseId(c.CourseId).Name,
                    courseTeacher = _userService.GetUserByUserId(_courseService.GetCourseByCourseId(c.CourseId).TeacherId).Name,
                    numStudent = _db.Entry(c).Collection(cl => cl.CourseSelections).Query().Count()
                }));
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "用户ID输入格式错误" });
            }
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
                var c = _classService.GetClassByClassId(classId);
                var se = _seminarService.ListSeminarByCourseId(c.CourseId).FirstOrDefault(
                    s => (_classService.GetCallStatusById(s.Id, c.Id)?.Status ?? 0) == 1);
                return Json(new
                {
                    id = c.Id,
                    name = c.Name,
                    time = c.ClassTime,
                    site = c.Site,
                    courseId = c.CourseId,
                    calling = se?.Id ?? -1,
                    proportions = new
                    {
                        report = c.ReportPercentage,
                        presentation = c.PresentationPercentage,
                        c = c.ThreePointPercentage,
                        b = c.FourPointPercentage,
                        a = c.FivePointPercentage
                    }

                });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "班级ID输入格式有误" });
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
                var user = _userService.GetUserByUserId(User.Id());
                if (user.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }
                _classService.UpdateClassByClassId(classId, updated);
                return NoContent();
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "班级ID格式错误" });
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
                var user = _userService.GetUserByUserId(User.Id());
                if (user.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }
                _classService.DeleteClassByClassId(classId);
                return NoContent();
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "班级ID格式错误" });
            }
        }

        /// <summary>
        /// 按班级ID查找学生列表（查询学号、姓名开头）
        /// data: JSON.stringify({ numBeginWith:" xxx", nameBeginWith:"xxx" })
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId:long}/student")]
        public IActionResult GetStudentsByClassId([FromRoute] long classId, [FromQuery] string numBeginWith, string nameBeginWith)
        {
            try
            {
                var users = _userService.ListUserByClassId(classId, numBeginWith, nameBeginWith);
                return Json(users.Select(u => new { id = u.Id, name = u.Name, number = u.Number }));
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "班级格式输入有误" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "班级ID输入格式有误" });
            }
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
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                if (user.Type == Shared.Models.Type.Student)
                {
                    var id = _classService.InsertCourseSelectionById(student.Id, classId);
                    return Created($"/class/{classId}/student/{student.Id}",
                        new Dictionary<string, string> { ["url"] = $" /class/{classId}/student/{student.Id}" });
                }
                else
                    return StatusCode(403, new { msg = "权限不足" });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "班级不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "班级ID输入格式有误" });
            }
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
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                if (user.Type == Shared.Models.Type.Student)
                {
                    _classService.DeleteCourseSelectionById(studentId, classId);
                    return NoContent();
                }
                else
                    return StatusCode(403, new { msg = "权限不足" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "ID格式错误" });
            }
        }

        /// <summary>
        /// 获取班级签到人数及名单
        /// </summary>
        /// data:JSON.stringify({ showPresent:"", showLate:"",showAbsent:"" }),
        /// <param name="classId"></param>
        /// <param name="seminarId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId:long}/{seminarId:long}/attendance/showPresent")]
        public IActionResult GetPresentList([FromRoute] long classId, [FromRoute]long seminarId)
        {
            try
            {
                 var user = _userService.GetUserByUserId(User.Id());
                //var user = _userService.GetUserByUserId(1);
                if (user.Type !=Shared.Models .Type .Teacher)
                {
                    return StatusCode(403,new { msg="权限不足"});
                }
                var presentList = _userService.ListPresentStudent(seminarId, classId);
                var classInfo = _classService.GetClassByClassId(classId);
                return Json(new {
                classname=classInfo.Name,
                presentcount=presentList.Count(),
                numStudent = _db.Entry(classInfo).Collection(cl => cl.CourseSelections).Query().Count(),
                studentList =presentList.Select (p=>new {
                    id=p.Id,
                 number=p.Number ,
                name=p.Name})
                });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404,new { msg="未找到班级"});
            }catch(SeminarNotFoundException )
            {
                return StatusCode(404,new { msg="未找到讨论课"});
            }catch(ArgumentException )
            {
                return StatusCode(400,new { msg="错误的ID格式"});
            }
        }
        /// <summary>
        /// 获取班级缺勤名单
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="seminarId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId}/{seminarId}/attendance/showabsent")]
        public IActionResult GetAbsentList([FromRoute]long classId,[FromRoute ]long seminarId)
        {
            try
            {
                 var user = _userService.GetUserByUserId(User.Id());
              //  var user = _userService.GetUserByUserId(1);
                if (user.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }
                var absenttList = _userService.ListAbsenceStudent(seminarId ,classId);
               
                var latsList= _userService.ListLateStudent(seminarId,classId);
                foreach(UserInfo u in latsList)
                {
                    absenttList.Add(u);
                }
                return Json(new
                {
                    absentcount = absenttList.Count(),
                    studentList = absenttList.Select(p => new {
                        id=p.Id,
                        number = p.Number,
                        name = p.Name
                    })
                });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }
        /// <summary>
        /// 获取班级迟到学生名单
        /// </summary>
        /// <param name="seminarId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId}/{seminarId}/attendance/showlate")]
        public IActionResult GetLateStudent([FromRoute]long seminarId,[FromRoute ]long classId)
        {
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                //var user = _userService.GetUserByUserId(1);
                if (user.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }
                var lateList = _userService.ListLateStudent(seminarId,classId);
                return Json(new {
                    latecount = lateList.Count(),
                    studentList=lateList.Select(p=>new {
                        id=p.Id,
                    number=p.Number ,
                    name=p.Name})
                });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }
        /// <summary>
        /// 随机小组添加迟到成员
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        [HttpPut("/group/{groupId:long}/addlate/{studentId:long}")]
        public IActionResult InsertMemberByStudentId([FromRoute]long groupId, [FromRoute] long studentId)
        {
            try
            {
                //将学生加入讨论课小组
                _seminarGroupService.InsertSeminarGroupMemberById(studentId, groupId);
                var group = _seminarGroupService.GetSeminarGroupByGroupId(groupId);
                var attendance = _db.Attendances.SingleOrDefault(s => (s.SeminarId == group.SeminarId && s.ClassId == group.ClassId && s.StudentId == studentId));
                attendance.AttendanceStatus = AttendanceStatus.Present;
                _db.SaveChanges();
                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "组号格式错误" });
            }
        }
        /// <summary>
        /// 教师发起签到
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        [HttpPost("/class/{classId:long}/startrollcall")]
        public IActionResult StartRollCallById([FromBody]Location loc)
        {
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                //var user = _userService.GetUserByUserId(1);
                if (user.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }
                else
                {
                    _userService.InsertClassAttendanceById(loc.ClassInfoId,loc.SeminarId);
                    loc.ClassInfo = _classService.GetClassByClassId(loc.ClassInfoId);
                    loc.Seminar = _seminarService.GetSeminarBySeminarId(loc.SeminarId);
                    var id = _classService.CallInRollById(loc);
                    return Created($"/class/{loc.ClassInfo.Id}/rollcall/{id}",
                            new Dictionary<string, string> { ["url"] = $"/class/{loc.ClassInfo.Id}/rollcall/{id}" });
                }
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "ID格式错误" });
            }
        }

        /// <summary>
        /// 老师结束签到
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="seminarId"></param>
        /// <returns></returns>
        [HttpPut("/class/{classId:long}/{seminarId:long}/endrollcall")]
        public IActionResult EndRollCall([FromRoute]long classId, [FromRoute]long seminarId)
        {
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
             //   var user = _userService.GetUserByUserId(1);
                if (user.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }
                else
                {
                    var seminar = _seminarService.GetSeminarBySeminarId(seminarId);
                    _classService.EndCallRollById(seminarId, classId);
                    if (seminar.IsFixed == false)
                    {
                        _seminarGroupService.AutomaticallyGrouping(seminarId, classId);
                    }
                    return NoContent();
                }
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "ID格式错误" });
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
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                if (user.Type == Shared.Models.Type.Teacher)
                {

                    IList<FixGroup> groups = _fixGroupService.ListFixGroupByClassId(classId);
                    return Json(groups.Select(g =>
                    new
                    {
                        leader = new
                        {
                            id = g.LeaderId,
                            name = _userService.GetUserByUserId(g.LeaderId).Name,
                            number = _userService.GetUserByUserId(g.LeaderId).Number
                        },
                        member = _fixGroupService.ListFixGroupMemberByGroupId(g.Id).Select(m => new
                        {
                            id = m.Id,
                            name = m.Name,
                            number = m.Number
                        })
                    }
                        ));
                }
                else
                {
                    var fixGroup = _fixGroupService.GetFixedGroupById(User.Id(), classId);
                    var leader = fixGroup.Leader ?? _userService.GetUserByUserId(fixGroup.LeaderId);
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
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "课程ID格式错误" });
            }
        }

        /// <summary>
        /// 按ID修改班级小组
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpPut("/class/{classId}/classgroup/{groupId:long}")]
        public IActionResult UpdateUserClassGroupByClassId([FromRoute] long classId, [FromRoute]long groupId,
            [FromBody]FixGroup fixGroupBo)
        {
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                if (user.Type != Shared.Models.Type.Teacher)
                {
                    return StatusCode(403, new { msg = "权限不足" });
                }
                _fixGroupService.UpdateFixGroupByGroupId(groupId, fixGroupBo);
                return NoContent();
            }catch(ClassNotFoundException )
            {
                return StatusCode(404,new { msg="未找到班级"});
            }catch(FixGroupNotFoundException )
            {
                return StatusCode(404,new { msg ="未找到该小组"});
            }catch(ArgumentException )
            {
                return StatusCode(400,new { msg="ID格式错误"});
            }
        }
    }
    */
}