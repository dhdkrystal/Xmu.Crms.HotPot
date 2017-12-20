namespace CourseManagementSystem.Controllers
{
    public class SchoolApiController : ApiController
    {
        //GET:/school
        [System.Web.Http.Route("api/school/{seminarId}/detail")]
        [System.Web.Http.HttpGet]
        public JsonResult GetProvince(int seminarId)
        {
            var result = new JsonResult();
            var school = new object[]
            {
                "北京","上海","贵州","江西","安徽","河北","河南","广西","江苏","广东","新疆","青海","重庆","吉林"
            };
            result.Data = school;
            return result;
        }

        //GET:/school
        [System.Web.Http.Route("api/school1/{seminarId}/detail")]
        [System.Web.Http.HttpGet]
        public JsonResult GetCity(int seminarId)
        {
            var result = new JsonResult();
            var school = new object[]
            {
               "厦门","三明","泉州","漳州","福州","永安"
            };
            result.Data = school;
            return result;
        }

        [System.Web.Http.Route("api/school2/{seminarId}/detail")]
        [System.Web.Http.HttpGet]
        public JsonResult GetSchool(int seminarId)
        {
            var result = new JsonResult();
            var school = new object[]
            {
               "厦门大学","厦门理工学院","华侨大学","集美大学","诚毅学院"
            };
            result.Data = school;
            return result;
        }


        //在指定城市创建学校
        //POST:/course/{courseId}/class
        [System.Web.Http.Route("api/school/{city}")]
        [System.Web.Http.HttpPost]
        public JsonResult PostSchool(int cityname, [FromBody]dynamic json)
        {
            JsonResult result = new JsonResult();
            var school = new { id = 23, name = json.name, city=cityname };
            result.Data = school.id;
            return result;
        }

        //GET:/courseinfo
        [System.Web.Http.Route("api/courseinfo/{schoolId}")]
        [System.Web.Http.HttpGet]
        public JsonResult GetCourseInfo(int schoolId)
        {
            var result = new JsonResult();
            var data = new { course="OOAD", teacher = "邱明", mail = "mingqiu@gmail.com", detail="面向对象分析与设计 "};
             result.Data = data;
            return result;
        }

        //GET:/
        [System.Web.Http.Route("api/courseui")]
        [System.Web.Http.HttpGet]
        public JsonResult GetCourseUI()
        {
            var result = new JsonResult();
            var data = new object[]
            {
                new {id =29, name= "界面原型设计",description= "界面原型设计",groupingMethod="fixed",startTime= "2017-09-25",endTime= "2017-10-09",grade= 4 },
                new {id =32, name= "概要设计",description= "模型层与数据库设计",groupingMethod="fixed",startTime= "2017-10-10",endTime= "2017-10-24",grade= 5 },
            };
            result.Data = data;
            return result;

        }



       

    }
}
