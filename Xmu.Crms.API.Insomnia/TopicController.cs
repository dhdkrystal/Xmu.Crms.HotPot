using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Insomnia
{
    [Route("")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly ISeminarGroupService _seminarGroupService;

        public TopicController(ITopicService topicService, ISeminarGroupService seminarGroupService)
        {
            _topicService = topicService;
            _seminarGroupService = seminarGroupService;
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/topic/{topicId:long}")]
        public IActionResult GetTopicById([FromRoute] long topicId)
        {
            try
            {
                var t = _topicService.GetTopicByTopicId(topicId);
                return Json(new
                {
                    id = t.Id,
                    serial = t.Serial,
                    name = t.Name,
                    description = t.Description,
                    groupLimit = t.GroupNumberLimit,
                    groupMemberLimit = t.GroupStudentLimit,
                });
            }
            catch (TopicNotFoundException )
            {
                return StatusCode(404, new { msg = "话题不存在" });
            }

        }

        [HttpDelete(API.Insomnia.Constant.PREFIX + "/topic/{topicId:long}")]
        public IActionResult DeleteTopicById([FromRoute] long topicId)
        {
            if (User.Type() != Type.Teacher)
            {
                return StatusCode(403, new {msg = "权限不足"});
            }

            try
            {
                _topicService.DeleteTopicByTopicId(topicId);
                return NoContent();
            }
            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "话题不存在" });
            }
        }

        [HttpPut(API.Insomnia.Constant.PREFIX + "/topic/{topicId:long}")]
        public IActionResult UpdateTopicById([FromRoute] long topicId, [FromBody] Topic updated)
        {
            if (User.Type() != Type.Teacher)
            {
                return StatusCode(403, new { msg = "权限不足" });
            }

            try
            {
                _topicService.UpdateTopicByTopicId(topicId, updated);
                return NoContent();
            }
            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "话题不存在" });
            }
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/topic/{topicId:long}/group")]
        public IActionResult GetGroupsByTopicId([FromRoute] long topicId)
        {
            try
            {
                return Json(_seminarGroupService.ListGroupByTopicId(topicId).Select(s => new{id = s.Id, name = s.Id + "组"}));
            }
            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "话题不存在" });
            }
        }
    }
}