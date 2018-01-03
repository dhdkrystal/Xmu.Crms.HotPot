using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Type = Xmu.Crms.Shared.Models.Type;

namespace Xmu.Crms.Insomnia
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GroupController : Controller
    {
        private readonly IFixGroupService _fixGroupService;
        private readonly IGradeService _gradeService;
        private readonly ISeminarGroupService _seminarGroupService;
        private readonly ITopicService _topicService;
        private readonly IUserService _userService;

        public GroupController(ICourseService courseService, IClassService classService,
            IUserService userService, IFixGroupService fixGroupService,
            ISeminarGroupService seminarGroupService, ITopicService topicService,
            ISeminarService seminarService,
            IGradeService gradeService, JwtHeader header)
        {
            _userService = userService;
            _fixGroupService = fixGroupService;
            _seminarGroupService = seminarGroupService;
            _topicService = topicService;
            _gradeService = gradeService;
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/group/{groupId:long}")]
        public IActionResult GetGroupById([FromRoute] long groupId, [FromQuery] bool embedGrade = false)
        {
            try
            {
                var group = _seminarGroupService.GetSeminarGroupByGroupId(groupId);
                var members = _seminarGroupService.ListSeminarGroupMemberByGroupId(groupId);
                var topics = _topicService.ListSeminarGroupTopicByGroupId(groupId);
                if (!embedGrade)
                {
                    return Json(new
                    {
                        id = group.Id,
                        name = group.Id + "组",
                        leader = new
                        {
                            id = group.Leader.Id,
                            name = group.Leader.Name
                        },
                        members = members.Select(m => new
                        {
                            id = m.Id,
                            name = m.Name
                        }),
                        topics = topics.Select(t => new
                        {
                            id = t.Topic.Id,
                            name = t.Topic.Name
                        }),
                        report = group.Report
                    });
                }

                return Json(new
                {
                    id = group.Id,
                    name = group.Id + "组",
                    leader = new
                    {
                        id = group.Leader.Id,
                        name = group.Leader.Name
                    },
                    members = members.Select(m => new
                    {
                        id = m.Id,
                        name = m.Name
                    }),
                    topics = topics.Select(t => new
                    {
                        id = t.Topic.Id,
                        name = t.Topic.Name
                    }),
                    report = group.Report,
                    grade = new
                    {
                        presentationGrade = _topicService.ListSeminarGroupTopicByGroupId(groupId).Select(p => new
                        {
                            id = p.Id,
                            grade = p.PresentationGrade
                        }),
                        reportGrade = group.ReportGrade,
                        grade = group.FinalGrade
                    }
                });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new {msg = "未找到小组"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "组号格式错误"});
            }
        }

        /*
         * 没有找到相应的修改seminarGroup的方法，修改了SeminarGroup变为FixedGroup
         */
        [HttpPut(API.Insomnia.Constant.PREFIX + "/group/{groupId:long}")]
        public IActionResult UpdateGroupById([FromRoute] long groupId, [FromBody] /*SeminarGroup*/FixGroup updated)
        {
            try
            {
                if (User.Type() != Type.Teacher)
                {
                    return StatusCode(403, new {msg = "权限不足"});
                }

                _fixGroupService.UpdateFixGroupByGroupId(groupId, updated);
                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new {msg = "没有找到组"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "组号格式错误"});
            }
        }

        [HttpPost(API.Insomnia.Constant.PREFIX + "/group/{groupId:long}/topic")]
        public IActionResult SelectTopic([FromRoute] long groupId, [FromBody] Topic selected)
        {
            try
            {
                if (User.Type() != Type.Student)
                {
                    return StatusCode(403, new {msg = "权限不足"});
                }

                _seminarGroupService.InsertTopicByGroupId(groupId, selected.Id);
                return Created($"/group/{groupId}/topic/{selected.Id}",
                    new Dictionary<string, string> {["url"] = $" /group/{groupId}/topic/{selected.Id}"});
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new {msg = "没有找到该课程"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "组号格式错误"});
            }
        }

        [HttpDelete(API.Insomnia.Constant.PREFIX + "/group/{groupId:long}/topic/{topicId:long}")]
        public IActionResult DeselectTopic([FromRoute] long groupId, [FromRoute] long topicId)
        {
            try
            {
                if (User.Type() != Type.Student)
                {
                    return StatusCode(403, new {msg = "权限不足"});
                }

                _topicService.DeleteSeminarGroupTopicById(groupId, topicId);
                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new {msg = "没有找到该课程"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "组号格式错误"});
            }
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/group/{groupId:long}/grade")]
        public IActionResult GetGradeByGroupId([FromRoute] long groupId)
        {
            try
            {
                var group = _seminarGroupService.GetSeminarGroupByGroupId(groupId);
                var pGradeTopics = _topicService.ListSeminarGroupTopicByGroupId(groupId);
                return Json(new
                {
                    presentationGrade = pGradeTopics.Select(p => new
                    {
                        id = p.Id,
                        grade = p.PresentationGrade
                    }),
                    reportGrade = group.ReportGrade,
                    grade = group.FinalGrade
                });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new {msg = "没有找到该课程"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "组号格式错误"});
            }
        }

        [HttpPut(API.Insomnia.Constant.PREFIX + "/group/{groupId:long}/grade/report")]
        public IActionResult UpdateGradeByGroupId([FromRoute] long groupId, [FromBody] StudentScoreGroup updated)
        {
            try
            {
                if (User.Type() != Type.Teacher)
                {
                    return StatusCode(403, new {msg = "权限不足"});
                }

                if (updated.Grade != null)
                {
                    _gradeService.UpdateGroupByGroupId(groupId, (int) updated.Grade);
                }

                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new {msg = "没有找到该课程"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "组号格式错误"});
            }
        }

        [HttpPut(API.Insomnia.Constant.PREFIX + "/group/{groupId:long}/grade/presentation/{studentId:long}")]
        public IActionResult SubmitStudentGradeByGroupId([FromBody] long groupId, [FromBody] long studentId,
            [FromBody] StudentScoreGroup updated)
        {
            try
            {
                if (User.Type() != Type.Student)
                {
                    return StatusCode(403, new {msg = "权限不足"});
                }

                if (updated.Grade == null)
                {
                    return NoContent();
                }

                _gradeService.InsertGroupGradeByUserId(updated.SeminarGroupTopic.Topic.Id, updated.Student.Id,
                    groupId, (int) updated.Grade);
                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new {msg = "没有找到该课程"});
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new {msg = "组号格式错误"});
            }
        }
    }
}