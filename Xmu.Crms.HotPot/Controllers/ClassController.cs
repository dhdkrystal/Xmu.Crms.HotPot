using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class ClassController : Controller
    {/*
        /// <summary>
        /// 获取与当前用户相关的或者符合条件的班级列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("/class")]
        public IActionResult GetUserClasses()
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

       
        [HttpPost("/class")]
        public IActionResult CreateClass([FromBody] ClassInfo newClass)
        {
            return Created("/class/1", newClass);
        }

        /// <summary>
        /// 按ID获取班级详情
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId:long}")]
        public IActionResult GetClassById([FromRoute] long classId)
        {
            var c2 = new ClassInfo
            {
                Name = "一班",
                Site = "海韵202"
            };
            return Json(c2);
        }

        /// <summary>
        /// 按ID删除班级
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpDelete("/class/{classId:long}")]
        public IActionResult DeleteClassById([FromRoute] long classId)
        {
            return NoContent();
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
            return NoContent();
        }

        /// <summary>
        /// 按班级ID查找学生列表（查询学号、姓名开头）
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("/class/{classId:long}/student")]
        public IActionResult GetStudentsByClassId([FromRoute] long classId)
        {
            return Json(new List<ClassInfo>());
        }

        /// <summary>
        /// 学生按ID选择班级
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost("/class/{classId:long}/student")]
        public IActionResult SelectClass([FromRoute] long classId, [FromBody] User student)
        {
            return Created("/class/1/student/1", new Dictionary<string, string> {["url"] = " /class/1/student/1"});
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
            return NoContent();
        }


        [HttpGet("/class/{classId:long}/attendance")]
        public IActionResult GetAttendanceByClassId([FromRoute] long classId)
        {
            return Json(new List<ClassInfo>());
        }

        [HttpPut("/class/{classId:long}/attendance/{studentId:long}")]
        public IActionResult UpdateAttendanceByClassId([FromRoute] long classId, [FromRoute] long studentId,
            [FromBody] Location loc)
        {
            return NoContent();
        }

        [HttpGet("/class/{classId}/classgroup")]
        public IActionResult GetUserClassGroupByClassId([FromRoute] long classId)
        {
            var gu1 = new GroupUser()
            {
                IsLeader=true,
                Id= 2757,
                Name="张三",
                Number= "23320152202333"
            };
            var gu2 = new GroupUser()
            {
                IsLeader = false,
                Id = 2756,
                Name = "李四",
                Number = "23320152202443"
            };
            var gu3 = new GroupUser()
            {
                IsLeader = false,
                Id = 2777,
                Name = "王五",
                Number = "23320152202433"
            };
            return Json(new List<GroupUser> {gu1, gu2, gu3});
        }

        [HttpPut("/class/{classId}/classgroup")]
        public IActionResult UpdateUserClassGroupByClassId([FromRoute] long classId)
        {
            return NoContent();
        }

        //public class Attendance
        //{
        //    public int NumPresent { get; set; }
        //    public List<UserInfo> Present { get; set; }
        //    public List<UserInfo> Late { get; set; }
        //    public List<UserInfo> Absent { get; set; }
        //}


        //public struct Location
        //{
        //    public double Longitude { get; set; }
        //    public double Latitude { get; set; }
        //    public double Elevation { get; set; }
        //}

        public class GroupUser
        {
            public bool Isleader { get; set; }
            public long Id { get; set; }
            public string Name { get; set; }
            public string number { get; set; }
        }
        */
    }
}