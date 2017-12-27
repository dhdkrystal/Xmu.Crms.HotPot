using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }
        [HttpGet("/topic/{topicId:long}")]
        public IActionResult GetTopicById([FromRoute] long topicId)
        {         
            var topic=_topicService.GetTopicByTopicId(topicId);
            //得到该topic对应的siminar
            var seminar=
            //判断当前的讨论课是固定分组
            return Json(new Topic {
                Id = topic.Id,
                Serial = topic.Serial,
                Name=topic.Name,
                Description=topic.Description,
                GroupNumberLimit=topic.GroupNumberLimit,
                GroupStudentLimit=topic.GroupStudentLimit,
                
            });
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