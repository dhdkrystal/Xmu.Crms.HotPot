using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using System.Linq;
using System;
using Xmu.Crms.Shared.Exceptions;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly ISeminarService _seminarService;
        private readonly ISeminarGroupService _seminarGroupService;
        private CrmsContext _db;

        public TopicController(ISeminarGroupService seminarGroupService, ISeminarService seminarService, ITopicService topicService, CrmsContext db)
        {
            _db = db;
            _seminarGroupService = seminarGroupService;
            _topicService = topicService;
            _seminarService = seminarService;
        }

        [HttpGet("/topic/{topicId:long}")]
        public IActionResult GetTopicById([FromRoute] long topicId)
        {
            var topic = _topicService.GetTopicByTopicId(topicId);
            //得到该topic对应的siminar
            //var seminar = _seminarService.GetSeminarBySeminarId(topic.SeminarId);
            int select = _db.SeminarGroupTopic.Where(c => c.TopicId == topicId).Count();
            //判断当前的讨论课是固定分组
            return Json(new TopicViewModel()
            {
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
            try
            {
                _topicService.DeleteTopicByTopicId(topicId);
                return NoContent();
            }

            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { msg = "用户的权限不足" });
            }
            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到话题" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        [HttpPut("/topic/{topicId:long}")]
        public IActionResult UpdateTopicById([FromRoute] long topicId, [FromBody] Topic updated)
        {
            try
            {
                _topicService.UpdateTopicByTopicId(topicId, updated);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { msg = "用户的权限不足" });
            }
            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到话题" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        [HttpGet("/topic/{topicId:long}/group")]
        public IActionResult GetGroupsByTopicId([FromRoute] long topicId)
        {
            try
            {
                List<SeminarGroupViewModel> ss = new List<SeminarGroupViewModel>();
                var groups = _seminarGroupService.ListGroupByTopicId(topicId);
                foreach (var g in groups)
                {
                    SeminarGroupViewModel s = new SeminarGroupViewModel();
                    s.Id = g.Id;
                    //没找到小组名字这个属性？？
                    ss.Add(s);
                }
                return Json(ss);
            }

            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }
    }
   
    public class SeminarGroupViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public static implicit operator List<object>(SeminarGroupViewModel v)
        {
            throw new NotImplementedException();
        }
    }


}