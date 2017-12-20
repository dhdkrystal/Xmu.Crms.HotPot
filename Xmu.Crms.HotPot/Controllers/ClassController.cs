using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class ClassController : Controller
    {
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

        [HttpDelete("/class/{classId:long}")]
        public IActionResult DeleteClassById([FromRoute] long classId)
        {
            return NoContent();
        }

        [HttpPut("/class/{classId:long}")]
        public IActionResult UpdateClassById([FromRoute] long classId, [FromBody] ClassInfo updated)
        {
            return NoContent();
        }

        [HttpGet("/class/{classId:long}/student")]
        public IActionResult GetStudentsByClassId([FromRoute] long classId)
        {
            return Json(new List<ClassInfo>());
        }

        [HttpPost("/class/{classId:long}/student")]
        public IActionResult SelectClass([FromRoute] long classId, [FromBody] UserInfo student)
        {
            return Created("/class/1/student/1", new Dictionary<string, string> {["url"] = " /class/1/student/1"});
        }

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

        public class Attendance
        {
            public int NumPresent { get; set; }
            public List<UserInfo> Present { get; set; }
            public List<UserInfo> Late { get; set; }
            public List<UserInfo> Absent { get; set; }
        }


        public struct Location
        {
            public double Longitude { get; set; }
            public double Latitude { get; set; }
            public double Elevation { get; set; }
        }

        public class GroupUser
        {
            public bool IsLeader { get; set; }
            public long Id { get; set; }
            public string Name { get; set; }
            public string Number { get; set; }
        }
    }
}