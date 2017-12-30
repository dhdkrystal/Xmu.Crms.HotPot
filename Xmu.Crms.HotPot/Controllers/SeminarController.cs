using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xmu.Crms.Shared;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class SeminarController : Controller
    {
        private readonly ISeminarService _service;
        private readonly ITopicService _service1;
        private readonly ISeminarGroupService _service2;

        public SeminarController(ISeminarService service, ITopicService service1, ISeminarGroupService service2)
        {
            _service = service;
            _service1 = service1;
            _service2 = service2;
        }

        [HttpGet("/seminar/{seminarId:long}")]//测试成功
        public IActionResult GetSeminarByIdRandom([FromRoute] long seminarId)
        {
            try
            {
                var sem = _service.GetSeminarBySeminarId(seminarId);
                return Json(sem);
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
        }

        [HttpDelete("/seminar/{seminarId:long}")]//测试成功
        public IActionResult DeleteSeminarById([FromRoute] long seminarId)
        {
            try
            {
                _service.DeleteSeminarBySeminarId(seminarId);
                return NoContent();
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
        }

        [HttpPut("/seminar/{seminarId:long}")]//测试成功
        public IActionResult UpdateSeminarById([FromRoute] long seminarId, [FromBody] Seminar updated)
        {
            try
            {
                _service.UpdateSeminarBySeminarId(seminarId, updated);
                return NoContent();
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "话题不存在!" });
            }
        }

        [HttpGet("/seminar/{seminarId:long}/topic")]//测试成功
        public IActionResult GetTopicsBySeminarId([FromRoute] long seminarId)
        {
            try
            {
                IList<Topic> l = _service1.ListTopicBySeminarId(seminarId);
                return Json(l);
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "话题不存在!" });
            }
        }

        [HttpPut("/seminar/{seminarId:long}/topic")]//测试成功
        public IActionResult CreateTopicBySeminarId([FromRoute] long seminarId, [FromBody] Topic newTopic)
        {
            _service1.InsertTopicBySeminarId(seminarId, newTopic);
            return Created("/topic/{topicId:long}", newTopic);
        }

        [HttpGet("/seminar/{seminarId:long}/group")]
        public IActionResult GetGroupsBySeminarId([FromRoute] long seminarId)
        {
            try
            {
                IList<SeminarGroup> semg = _service2.ListSeminarGroupBySeminarId(seminarId);
                return Json(semg);
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "讨论课不存在!" });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "小组不存在!" });
            }
        }
    }
}