using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using System.Linq;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly ISeminarService _seminarService;
        private CrmsContext _db;

        public TopicController(ISeminarService seminarService, ITopicService topicService, CrmsContext db)
        {
            _db = db;
            _topicService = topicService;
            _seminarService = seminarService;
        }
        [HttpGet("/topic/{topicId:long}")]
        public IActionResult GetTopicById([FromRoute] long topicId)
        {
            var topic = _topicService.GetTopicByTopicId(topicId);
            //得到该topic对应的siminar
            var seminar = _seminarService.GetSeminarBySeminarId(topic.SeminarId);
            int select = _db.SeminarGroupTopic.Where(c => c.TopicId == topicId).Count();
            //判断当前的讨论课是固定分组
            return Json(new TopicViewModel() {
                Id = topic.Id,
                Name = topic.Name,
                Description = topic.Description,
                GroupNumberLimit = topic.GroupNumberLimit,
                GroupStudentLimit = topic.GroupStudentLimit,
                GroupLeft = topic.GroupNumberLimit - select
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
    public class TopicViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int GroupNumberLimit { get; set; }
        public int GroupStudentLimit { get; set; }
        public int GroupLeft { get; set; }
    }
    
}