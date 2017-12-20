using System.Net;
using System.Net.Http;
using System.Text;

namespace CourseManagementSystem.Controllers
{
    public class SeminarApiController : Controller
    {
        //GET:minar/{seminarId} 按ID获取讨论课
        [System.Web.Http.Route("api/seminar/{seminarId}")]
        [System.Web.Http.HttpGet]
        public JsonResult GetSeminar(int seminarId)
        {
            var result = new JsonResult();
            var topics = new object[]
            {
                new{id=257,name="领域模型与模块"}
            };
            var data = new object[]
            {
                new{id=32,name="概要设计",description = "模型层与数据库设计", groupingMethod = "fixed", startTime = "2017-10-10", endTime = "2017-10-24",topics}
            };
            result.Data = data;
            return result;
        }

        //PUT：minar/{seminarId} 按ID修改讨论课
        [System.Web.Http.Route("api/seminar/{seminarId}")]
        [System.Web.Http.HttpPut]
        public HttpResponseMessage ModifySeminar(int seminarId, [FromBody]dynamic jason)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
            response.Content = new StringContent("成功", Encoding.UTF8);
            return response;
        }
        
        //DELETE:minar/{seminarId} 按ID删除讨论课
        [System.Web.Http.Route("api/seminar/{seminarId}")]
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteSeminar(int seminarId)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
            response.Content = new StringContent("成功", Encoding.UTF8);
            return response;
        }

        //GET：minar/{seminarId}/my 按ID获取与学生有关的讨论课信息
        [System.Web.Http.Route("api/seminar/{seminarId}/my")]
        [System.Web.Http.HttpGet]
        public JsonResult GetStudentSeminar(int seminarId)
        {
            var result = new JsonResult();
            var proportions = new { report = "50", presentation = "50", c = "20", b = "60", a = "20" };
            var data = new { name = "概要设计", description = "模型层与数据库设计", groupingMethod = "fixed", startGTime = "2017-10-11", endTime = "2017-10-24", proportions };
            result.Data = data;
            return result;
        }

        //GET：/seminar/{seminarId}/detail 按ID获取讨论课详情
        [System.Web.Http.Route("api/seminar/{seminarId}/detail")]
        [System.Web.Http.HttpGet]
        public JsonResult getSeminarDetail(int seminarId)
        {
            var result = new JsonResult();
            var data = new { id = 32, name = "概要设计", startTime = "2017-10-10", endTime = "2017-10-24", site = "海韵201", teacherName = "邱明", teacherEmail = "mingqiu@xmu.edu.cn" };
            
            result.Data = data;
            return result;
        }

        //GET:/seminar/{seminarId}/topic 按ID获取讨论课的话题
        [System.Web.Http.Route("api/seminar/{seminarId}/topic")]
        [System.Web.Http.HttpGet]
        public JsonResult getTopic(int seminarId)
        {
            var result = new JsonResult();
            var data = new { id = 257, serial = "A", name = "领域模型与模块", description = "Domain model与模块划分", groupLimit = "5", groupMemberLimit = "6", groupLeft = "2" };
            
            result.Data = data;
            return result;
        }

        //POST:/seminar/{seminarId}/topic 在指定ID的讨论课创建话题
        [System.Web.Http.Route("api/seminar/{seminarId}/topic")]
        [System.Web.Http.HttpPost]
        public JsonResult PostTopic(int seminatId, [FromBody]dynamic json)
        {
            JsonResult result = new JsonResult();
            var Topic = new
            {
                id = 257,
                serial = json.serial,
                name = json.name,
                description = json.description,
                groupLimit = json.groupLimit,
                groupMemberLimit = json.groupMenberLimit
            };
            result.Data = Topic.id;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        //GET：/seminar/{seminarId}/group 按讨论课ID查找小组
        [System.Web.Http.Route("api/seminar/{seminarId}/group")]
        [System.Web.Http.HttpGet]
        public JsonResult GetGroup(int seminarId)
        {
            var result = new JsonResult();
            var topics = new { id = 257, name = "领域模型与模块" };
            var data = new object[]
            {
                new{id =28,name="1A1",topics},
                new { id = 29, name = "1A2",topics}
            };
            result.Data = data;
            return result;
        }

        //GET：minar/{seminarId}/group/my 按讨论课ID获取学生所在小组详情
        [System.Web.Http.Route("api/seminar/{seminarId}/group/my")]
        [System.Web.Http.HttpGet]
        public JsonResult GetGroupDetail(int seminarId)
        {
            var result = new JsonResult();
            var leader = new { id = 8888, name = "张三" };

            var members = new object[]
            {
                new{id=5324,name="李四"},
                new{id=5678,name="王五"}
            };
            var topics = new { id = 257, name = "领域模型与模块" };

            var data = new { id = 28, name = "28", leader, members, topics };
           
            result.Data = data;
            return result;
        }

        //GET:minar/{seminarId}/class/{classId}/attendance 按ID获取讨论课班级签到、分组状态
        [System.Web.Http.Route("api/seminar/{seminarId}/class/{classId}/attendance")]
        [System.Web.Http.HttpGet]
        public JsonResult GetAttendance(int seminarId, int classId)
        {
            var result = new JsonResult();
            var data = new object[]
            {
                new{numPresent="40",numStudent="60",status="calling",group="grouping"}
            };
            result.Data = data;
            return result;
        }

        //GET:minar/{seminarId}/class/{classId}/attendance/present 按ID获取讨论课班级已签到名单
        [System.Web.Http.Route("api/seminar/{seminarId}/class/{classId}/present")]
        [System.Web.Http.HttpGet]
        public JsonResult GetPresent(int seminarId, int classId)
        {
            var result = new JsonResult();
            var data = new object[]
            {
                new{id=2357,name="张三"},
                new{id=8232,name="李四"}
            };
            result.Data = data;
            return result;

        }

        //GET:minar/{seminarId}/class/{classId}/late 按ID获取讨论课班级迟到签到名单
        [System.Web.Http.Route("api/seminar/{seminarId}/class/{classId}/late")]
        [System.Web.Http.HttpGet]
        public JsonResult GetLate(int seminarId, int classId)
        {
            var result = new JsonResult();
            var data = new object[]
            {
                new{id=3412,name="王五"},
                new{id=5234,name="王七九"}
            };
            result.Data = data;
            return result;
        }

        //GET:minar/{seminarId}/class/{classId}/attendance/absent 按ID获取讨论课班级缺勤名单
        [System.Web.Http.Route("api/seminar/{seminarId}/class/{classId}/attendance/absent")]
        [System.Web.Http.HttpGet]
        public JsonResult GetAbsent(int seminarId, int classId)
        {
            var result = new JsonResult();
            var data = new object[]
            {
                new{id=34,name="张六"}
            };
            result.Data = data;
            return result;
        }

        //PUT：minar/{seminarId}/class/{classId}/attendance/{studentId} 签到(上传位置信息）
        [System.Web.Http.Route("api/seminar/{seminarId}/class/{classId}/attendance/{studentId}")]
        [System.Web.Http.HttpPut]
        public JsonResult Attendance(int seminarId, int classId, int studentId)
        {
            var result = new JsonResult();
            var data = new { longitude = "118.1132721", latitude = "24.4307197", elevation = "18.42", status = "late" };
            
            
            result.Data = data;
            return result;
        }
    }
}
