using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.HotPot.Controllers
{
    /*
    [Route("")]
    [Produces("application/json")]
    public class GroupController : Controller
    {
        [HttpGet("/group/{groupId:long}")]
        public IActionResult GetGroupById([FromRoute] long groupId)
        {
            var gs = new List<SeminarGroup>
            {
                new SeminarGroup(),
                new SeminarGroup(),
                new SeminarGroup()
            };
            return Json(gs[(int) groupId]);
        }

        [HttpPut("/group/{groupId:long}")]
        public IActionResult UpdateGroupById([FromRoute] long groupId, [FromBody] SeminarGroup updated)
        {
            return NoContent();
        }

        [HttpPost("/group/{groupId:long}/topic")]
        public IActionResult SelectTopic([FromRoute] long groupId, [FromBody] Topic selected)
        {
            return Created("/group/1/topic/1", new Dictionary<string, string> {["url"] = " /group/1/topic/1"});
        }

        [HttpDelete("/group/{groupId:long}/topic/{topicId:long}")]
        public IActionResult DeselectTopic([FromRoute] long groupId, [FromRoute] long topicId)
        {
            return NoContent();
        }

        [HttpGet("/group/{groupId:long}/grade")]
        public IActionResult GetGradeByGroupId([FromRoute] long groupId)
        {
            return Json(new StudentScoreGroup());
        }

        [HttpPut("/group/{groupId:long}/grade/report")]
        public IActionResult UpdateGradeByGroupId([FromRoute] long groupId, [FromBody] StudentScoreGroup updated)
        {
            return NoContent();
        }

        [HttpPut("/group/{groupId:long}/grade/presentation/{studentId:long}")]
        public IActionResult SubmitStudentGradeByGroupId([FromBody] long groupId, [FromBody] long studentId,
            [FromBody] StudentScoreGroup updated)
        {
            return NoContent();
        }
    }
    */
}