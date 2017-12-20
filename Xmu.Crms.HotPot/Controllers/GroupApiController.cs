using System.Net;
using System.Net.Http;
using System.Text;

namespace CourseManagementSystem.Controllers
{
    public class GroupApiController : ApiController
    {
        //PUT:/group/{groupId}/resign 组长辞职
        [System.Web.Http.Route("api/group/{groupId}/resign")]
        [System.Web.Http.HttpPut]
        public HttpResponseMessage ResignLeader(int groupId, [FromBody]dynamic json)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
            response.Content = new StringContent("成功", Encoding.UTF8);
            return response;
        }

        //PUT:/group/{groupId}/assign 成为组长
        [System.Web.Http.Route("api/group/{groupId}/assign")]
        [System.Web.Http.HttpPut]
        public HttpResponseMessage AssignLeader(int groupId, [FromBody]dynamic json)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
            response.Content = new StringContent("成功", Encoding.UTF8);
            return response;
        }

        //PUT://group/{groupId}/grade/presentation/{studentId} 提交对其他小组的打分
        [System.Web.Http.Route("api/group/{groupId}/grade/presentation/{studentId}")]
        [System.Web.Http.HttpPut]
        public JsonResult SubmitGrade(int groupId)
        {
            var result = new JsonResult();
            var presentationGrade = new object[] {
                new{topicId="257",grade="4"},
                new{topicId="258",grade="5"}
            };
            result.Data = presentationGrade;
            return result;
        }

        //POST://group/{groupId}/topic 小组按ID选择话题
        [System.Web.Http.Route("api/group/{groupId}/topic")]
        [System.Web.Http.HttpPost]
        public JsonResult SelectTopic(int groupId, [FromBody]dynamic json)
        {
            JsonResult result = new JsonResult();
            var topic = new { id = 23 };
            result.Data = topic;
            return result;
        }
    }
}



