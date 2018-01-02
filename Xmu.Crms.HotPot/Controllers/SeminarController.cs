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



        /// <summary>
        /// 获得讨论课详情
        /// </summary>
        /// <param name="seminarId"></param>
        /// <returns></returns>
        [HttpGet("/seminar/{seminarId:long}/class/{classId:long}/detail/{studentId:long}")]//测试成功
        public IActionResult GetSeminarDetailBySeminarId([FromRoute] long seminarId, long classId,long studentId)
        {
            try
            {
                var seminar = _seminarService.GetSeminarBySeminarId(seminarId);
                var classInfo = _classService.GetClassByClassId(classId);
                var cours = _courseService.GetCourseByCourseId(classInfo.CourseId);
                var teacher = _userService.GetUserByUserId(cours.TeacherId);
                var attendance = _userService.GetAttendanceById(classId, seminarId, studentId);
                //0表示出勤，1表示迟到，2表示缺勤,-1表示没有出勤记录
                var seminarVM=new SeminarViewModel()
                {
                    Id = seminar.Id,
                    Name = seminar.Name,
                    TeacherName = teacher.Name,
                    TeacherEmail = teacher.Email,
                    Site = classInfo.Site,
                    StartTime = seminar.StartTime,
                    EndTime = seminar.EndTime
                };
                if (attendance == null)
                {
                    seminarVM.Status = -1;
                    return Json(seminarVM);
                }
                else
                {
                    seminarVM.Status =Convert.ToInt32(attendance.AttendanceStatus);
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
        public IActionResult CallInRoll([FromBody]Location location,[FromRoute] long seminarId,long classId,long studentId)
        {
            try
            {
                
                AttendanceStatus attend=_userService.InsertAttendanceById(classId,seminarId,studentId, location.Longitude??0,location.Latitude??0);
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
        public int Status{get;set;}
    }
    
}