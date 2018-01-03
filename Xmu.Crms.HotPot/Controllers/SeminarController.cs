
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
using Microsoft.EntityFrameworkCore;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class SeminarController : Controller
    {
        private readonly IClassService _classService;
        private readonly ICourseService _courseService;
        private readonly ISeminarService _service;
        private readonly ITopicService _service1;
        private readonly ISeminarGroupService _service2;
        private readonly ITopicService _service3;
        private readonly IUserService _userService;
        private CrmsContext _db;
        private JwtHeader _head;
        public SeminarController(IClassService classService, ICourseService courseService, IUserService userService, JwtHeader head, CrmsContext db, ISeminarService service, ITopicService service1, ISeminarGroupService service2, ITopicService service3)
        {
            _head = head;
            _db = db;
            _service = service;
            _service1 = service1;
            _service2 = service2;
            _service3 = service3;
            _userService = userService;
            _classService = classService;
            _courseService = courseService;
        }

        [HttpGet("/seminar/{seminarId:long}")]//测试成功
        public IActionResult GetSeminarByIdRandom([FromRoute] long seminarId)
        {
            try
            {
                var sem = _service.GetSeminarBySeminarId(seminarId);
                var cou = _db.Course.Find(sem.CourseId);
                sem.Course.Name = cou.Name;
                return Json(sem);
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
        }

        [HttpDelete("/seminar/{seminarId:long}")]//测试成功
        public IActionResult DeleteSeminarById([FromRoute] long seminarId)
        {
            try
            {
                _service.DeleteSeminarBySeminarId(seminarId);
                return NoContent();
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
        }

        [HttpPut("/seminar/{seminarId:long}")]//测试成功
        public IActionResult UpdateSeminarById([FromRoute] long seminarId, [FromBody] Seminar updated)
        {
            try
            {
                _service.UpdateSeminarBySeminarId(seminarId, updated);
                return NoContent();
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "话题不存在!" });
            }
        }

        [HttpGet("/seminar/{seminarId:long}/topic")]//测试成功
        public IActionResult GetTopicsBySeminarId([FromRoute] long seminarId)
        {
            try
            {
                List<TopicViewModel> res = new List<TopicViewModel>();
                IList<Topic> l = _service1.ListTopicBySeminarId(seminarId);

                foreach (var topic in l)
                {
                    int select = _db.SeminarGroupTopic.Where(c => c.TopicId == topic.Id).Count();

                    res.Add(new TopicViewModel()
                    {
                        Id = topic.Id,
                        Name = topic.Name,
                        Description = topic.Description,
                        GroupNumberLimit = topic.GroupNumberLimit,
                        GroupStudentLimit = topic.GroupStudentLimit,
                        GroupLeft = topic.GroupNumberLimit - select
                    });
                }
                return Json(res);
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "话题不存在!" });
            }
        }

        [HttpPut("/seminar/{seminarId:long}/topic")]//测试成功
        public IActionResult CreateTopicBySeminarId([FromRoute] long seminarId, [FromBody] Topic newTopic)
        {
            _service1.InsertTopicBySeminarId(seminarId, newTopic);
            return Created("/topic/{topicId:long}", newTopic);
        }

        [HttpGet("/seminar/{seminarId:long}/group")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetGroupsBySeminarId([FromRoute] long seminarId)
        {
            try
            {
                IList<SeminarGroup> semg = _service2.ListSeminarGroupBySeminarId(seminarId);

                return Json(semg);
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "小组不存在!" });
            }
        }

        [HttpGet("/seminar/{seminarId:long}/group/my")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetGroupsById([FromRoute] long seminarId)
        {
            try
            {
                var usrid = User.Id();
                var semg = _service2.GetSeminarGroupById(seminarId, User.Id());
                var lead = _service2.GetSeminarGroupLeaderById(usrid, seminarId);
                var topics = _service3.ListSeminarGroupTopicByGroupId(semg.Id);

                return Json(new { userid = usrid, seminargroup = semg, leader = lead, topic = topics });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "小组不存在!" });
            }
        }

        [HttpGet("/seminar/{seminarId:long}/group/grade")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetSeminarGroupByIdwithoutSameTopic([FromRoute] long seminarId)
        {
            var mine = _service2.GetSeminarGroupById(seminarId, User.Id());

            List<SeminarGroup> groupsameclass = _db.SeminarGroup.Where(c => c.ClassId == mine.ClassId && c.SeminarId == seminarId).ToList();

            List<SeminarGroupTopic> myseminargrouptopic = _service3.ListSeminarGroupTopicByGroupId(mine.Id);

            List<SeminarGroupTopic> allsemtopics = new List<SeminarGroupTopic>();
            List<SeminarGroupTopic> seminargrouptopictemp = new List<SeminarGroupTopic>();
            List<SeminarGroupTopic> seminargrouptopicsametopic = new List<SeminarGroupTopic>();

            if (myseminargrouptopic.Count == 1)
            {
                foreach (var i in groupsameclass)
                {
                    seminargrouptopictemp = _service3.ListSeminarGroupTopicByGroupId(i.Id);
                    foreach (var j in seminargrouptopictemp)
                        allsemtopics.Add(j);
                }
                seminargrouptopicsametopic = _db.SeminarGroupTopic.Where(c => c.TopicId == myseminargrouptopic[0].TopicId).ToList();
                foreach (var i in seminargrouptopicsametopic)
                    allsemtopics.Remove(i);
            }

            else if (myseminargrouptopic.Count > 1)
            {
                foreach (var i in groupsameclass)
                {
                    seminargrouptopictemp = _service3.ListSeminarGroupTopicByGroupId(i.Id);
                    foreach (var j in seminargrouptopictemp)
                        allsemtopics.Add(j);
                }
                foreach (var i in myseminargrouptopic)
                    allsemtopics.Remove(i);
            }

            return Json(allsemtopics);
        }
        /*[HttpGet("/seminar/{seminarId:long}/group/grade")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetSeminarGroupByIdwithoutSameTopic([FromRoute] long seminarId)
        {
            IList<SeminarGroup> semg = _service2.ListSeminarGroupBySeminarId(seminarId);

            var mine = _service2.GetSeminarGroupById(seminarId,User.Id());
            IList<SeminarGroupTopic> myseminargrouptopic = _service3.ListSeminarGroupTopicByGroupId(mine.Id);

            List<long> topicid=null;
            foreach (var i in myseminargrouptopic)
                topicid = _service3.GetTopicByTopicId(i.TopicId).Id;

            IList<SeminarGroupTopic> allsemtopics = null;
            IList<SeminarGroupTopic> seminargrouptopic = null;
            foreach (var i in semg)
            {
                seminargrouptopic = _service3.ListSeminarGroupTopicByGroupId(i.Id);
                foreach (var j in seminargrouptopic)
                    allsemtopics.Add(j);
            }
            
            foreach(var i in myseminargrouptopic)

            
            return Json(semg);
        }*/

        /// <summary>
        /// 获得讨论课详情
        /// </summary>
        /// <param name="seminarId"></param>
        /// <returns></returns>
        [HttpGet("/seminar/{seminarId:long}/class/{classId:long}/detail/{studentId:long}")]//测试成功
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetSeminarDetailBySeminarId([FromRoute] long seminarId, long classId, long studentId)
        {
            try
            {
                var seminar = _service.GetSeminarBySeminarId(seminarId);
                var classInfo = _classService.GetClassByClassId(classId);
                var cours = _courseService.GetCourseByCourseId(classInfo.CourseId);
                var teacher = _userService.GetUserByUserId(cours.TeacherId);
                var attendance = _userService.GetAttendanceById(classId, seminarId, studentId);
                //0表示出勤，1表示迟到，2表示缺勤,-1表示没有出勤记录
                var seminarVM = new SeminarViewModel()
                {
                    Id = seminar.Id,
                    Name = seminar.Name,
                    TeacherName = teacher.Name,
                    TeacherEmail = teacher.Email,
                    Site = classInfo.Site,
                    StartTime = seminar.StartTime,
                    EndTime = seminar.EndTime
                };
                if (attendance.AttendanceStatus == null)
                {
                    seminarVM.Status = -1;
                    return Json(seminarVM);
                }
                else
                {
                    seminarVM.Status = Convert.ToInt32(attendance.AttendanceStatus);
                    return Json(seminarVM);
                }
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "班级不存在!" });
            }
        }



        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="seminarId"></param>
        /// <returns></returns>
        [HttpPut("/seminar/{seminarId:long}/class/{classId:long}/attendance/{studentId:long}")]//测试成功
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult CallInRoll([FromBody]Location location, [FromRoute] long seminarId, long classId, long studentId)
        {
            try
            {
                AttendanceStatus? attend = _userService.UpdateAttendanceById(classId, seminarId, studentId, location.Longitude ?? 0, location.Latitude ?? 0);
                return NoContent();
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "班级不存在!" });
            }
            catch (ArgumentException)
            {
                return StatusCode(404, new { msg = "参数错误！" });
            }
        }
    }

    public class SeminarViewModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string TeacherName { get; set; }

        public string Site { get; set; }

        public string TeacherEmail { get; set; }

        public bool? IsFixed { get; set; }

        public string CourseName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public long ClassCalling { get; set; }

        public bool IsLeader { get; set; }

        public bool AreTopicSelected { get; set; }

        //0表示出勤，1表示迟到，2表示缺勤,-1表示没有出勤记录
        public int Status { get; set; }

    }

    public class TopicViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int GroupNumberLimit { get; set; }
        public int GroupStudentLimit { get; set; }
        public int GroupLeft { get; set; }
    }

}