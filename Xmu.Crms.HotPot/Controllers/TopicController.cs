using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class TopicController : Controller
    {
        [HttpGet("/topic/{topicId:long}")]
        public IActionResult GetTopicById([FromRoute] long topicId)
        {
            var t = new Topic
            {
                Name= "领域模型与模块",
                Description= "Domain model与模块划分",
                GroupNumberLimit = 5,
                GroupStudentLimit = 6
            };
            return Json(t);
        }

        [HttpDelete("/topic/{topicId:long}")]
        public IActionResult DeleteTopicById([FromRoute] long topicId)
        {
            return NoContent();
        }

        [HttpPut("/topic/{topicId:long}")]
        public IActionResult UpdateTopicById([FromRoute] long topicId, [FromBody] Topic updated)
        {
            return NoContent();
        }

        [HttpGet("/topic/{topicId:long}/group")]
        public IActionResult GetGroupsByTopicId([FromRoute] long topicId)
        {
            return Json(new List<dynamic>
            {
                new {id = 1, name = "1A1"},
                new {id = 2, name = "1A2"},
                new {id = 43, name = "2A1"},
                new {id = 65, name = "2A2"},
            });
        }
    }
}