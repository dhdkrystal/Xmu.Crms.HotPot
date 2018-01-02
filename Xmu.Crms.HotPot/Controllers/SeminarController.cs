using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xmu.Crms.Shared;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;
using System;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class SeminarController : Controller
    {
        private readonly ISeminarService _seminarService;
        private readonly ITopicService _topicService;
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        private readonly ISeminarGroupService _seminarGroupService;
        private readonly IClassService _classService;

        public SeminarController(IClassService classService,ISeminarService seminarService,ITopicService topicService, ISeminarGroupService seminarGroupService, IUserService userService, ICourseService courseService)
        {
            _classService = classService;
            _seminarService = seminarService;
            _userService = userService;
            _topicService = topicService;
            _courseService = courseService;
            _seminarGroupService = seminarGroupService;
        }

        [HttpGet("/seminar/{seminarId:long}")]//测试成功
        public IActionResult GetSeminarByIdRandom([FromRoute] long seminarId)
        {
            try
            {
                var sem = _seminarService.GetSeminarBySeminarId(seminarId);
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
                _seminarService.DeleteSeminarBySeminarId(seminarId);
                return NoContent();
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
        }

        /// <summary>
        /// 获取学生的讨论课详情
        /// </summary>
        /// <param name="seminarId"></param>
        /// <returns></returns>
        //[HttpGet("/seminar/{seminarId:long}/my")]//测试成功
        //public IActionResult GetSeminarById([FromRoute] long seminarId)
        //{
        //    try
        //    {
        //        var user = _userService.GetUserByUserId(User.Id());
        //        var sem = _seminarService.GetSeminarBySeminarId(seminarId);
        //        var cours = _courseService.GetCourseByCourseId(sem.CourseId);
        //        return Json(new SeminarViewModel()
        //        {
        //            Id = sem.Id,
        //            Name = sem.Name,
        //            IsFixed = sem.IsFixed,
        //            CourseName = cours.Name,
        //            StartTime=sem.StartTime,
        //            EndTime=_userService.ListAbsenceStudent(sem.Id,)
        //        });
        //    }
        //    catch (SeminarNotFoundException)
        //    {
        //        return StatusCode(404, new { msg = "讨论课不存在!" });
        //    }
        //}
        [HttpPut("/seminar/{seminarId:long}")]//测试成功
        public IActionResult UpdateSeminarById([FromRoute] long seminarId, [FromBody] Seminar updated)
        {
            try
            {
                _seminarService.UpdateSeminarBySeminarId(seminarId, updated);
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
                IList<Topic> l = _topicService.ListTopicBySeminarId(seminarId);
                return Json(l);
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
            _topicService.InsertTopicBySeminarId(seminarId, newTopic);
            return Created("/topic/{topicId:long}", newTopic);
        }

        [HttpGet("/seminar/{seminarId:long}/group")]
        public IActionResult GetGroupsBySeminarId([FromRoute] long seminarId)
        {
            try
            {
                IList<SeminarGroup> semg = _seminarGroupService.ListSeminarGroupBySeminarId(seminarId);
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

        /// <summary>
        /// 获得讨论课详情
        /// </summary>
        /// <param name="seminarId"></param>
        /// <returns></returns>
        [HttpGet("/seminar/{seminarId:long}/detail")]//测试成功
        public IActionResult GetSeminarDetailBySeminarId([FromRoute] long seminarId)
        {
             try
             {
              //var user = _userService.GetUserByUserId(User.Id());
                var sem = _seminarService.GetSeminarBySeminarId(seminarId);
                var cours = _courseService.GetCourseByCourseId(sem.CourseId);
                var teacher = _userService.GetUserByUserId(cours.TeacherId);
                cours.Teacher = teacher;
                sem.Course = cours;
                var classInfo = _classService.GetClassByUserIdAndSeminarId(4,seminarId);
                return Json(new SeminarViewModel()
                {
                    Id = sem.Id,
                    Name = sem.Name,
                    TeacherName = teacher.Name,
                    TeacherEmail = teacher.Email,
                    Site = classInfo.Site,
                    StartTime=sem.StartTime,
                    EndTime=sem.EndTime
                        
                    });
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
        /*
        [HttpPut("/seminar/{seminarId:long}/class/{classId:long}/attendance/{studentId:long}")]//测试成功
        public IActionResult CallInRoll([FromBody]Location location,[FromRoute] long seminarId,long classId,long studentId)
        {
            try
            {
                //var user = _userService.GetUserByUserId(User.Id());
                var sem = _seminarService.GetSeminarBySeminarId(seminarId);
                var cla = _classService.GetClassByClassId(classId);
                var user = _userService.GetUserByUserId(studentId);
                Location locat= new Location();
                locat.Latitude = location.Latitude;
                locat.Latitude = location.Latitude;
                _classService.CallInRollById(location);
                cours.Teacher = user;
                sem.Course = cours;
                var classInfo = _classService.GetClassByUserIdAndSeminarId(4, seminarId);
                return Json(new SeminarViewModel()
                {
                    Id = sem.Id,
                    Name = sem.Name,
                    TeacherName = user.Name,
                    TeacherEmail = user.Email,
                    Site = classInfo.Site,
                    StartTime = sem.StartTime,
                    EndTime = sem.EndTime

                });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "班级不存在!" });
            }

        }*/


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
    }
    
}