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

namespace Xmu.Crms.HotPot.Controllers
{

    [Route("")]
    [Produces("application/json")]
    public class GroupController : Controller
    {
        private readonly IFixGroupService _fixGroupService;
        private readonly IGradeService _gradeService;
        private readonly ISeminarGroupService _seminarGroupService;
        private readonly ITopicService _topicService;
        private readonly IUserService _userService;
        private CrmsContext _db;
        private JwtHeader _head;

        public GroupController(/*ICourseService courseService, IClassService classService,*/
            IUserService userService, IFixGroupService fixGroupService,
            ISeminarGroupService seminarGroupService, ITopicService topicService,
            /* ISeminarService seminarService,*/
            IGradeService gradeService, JwtHeader head, CrmsContext db)
        {
            _userService = userService;
            _fixGroupService = fixGroupService;
            _seminarGroupService = seminarGroupService;
            _topicService = topicService;
            _gradeService = gradeService;
            _db = db;
            _head = head;
        }

        //测试成功
        //9.老师课堂——ClassManege——固定分组——FixedRollStartCallUI——FixedGroupInfoUI
        //老师课堂——GroupInfoUI
        //老师课堂——GroupInfoUI2
        //学生课堂——固定分组&随机分组——未完成选题——RandomGroupLeaderUI
        //学生课堂——未完成选题——RandomGroupMemberUI
        //学生课堂——未完成选题——RandomGroupMemberUI2
        //学生课堂——已完成选题——RandomGroupLeaderUI
        //学生课堂——已完成选题——RandomGroupMemberUI
        //学生课堂——已完成选题——RandomGroupMemberUI2
        [HttpGet("/group/{groupId:long}")]
        public IActionResult GetGroupById([FromRoute] long groupId)
        {
            try
            {
                //按照Id查询某一讨论课小组的信息
                var group = _seminarGroupService.GetSeminarGroupByGroupId(groupId);
                var members = _seminarGroupService.ListSeminarGroupMemberByGroupId(groupId);
                var topics = _topicService.ListSeminarGroupTopicByGroupId(groupId);
                
                var usr = _userService.GetUserByUserId(group.LeaderId??0);
                return Json(new
                {
                    id = group.Id,
                    leader = new
                    {
                        id = group.LeaderId,
                        name = group.Leader.Name,
                        number = _userService.GetUserByUserId(group.LeaderId??0).Number
                    },
                    members = members.Select(m => new
                    {
                        id = m.Id,
                        name = m.Name,
                        number = _userService.GetUserByUserId(m.Id).Number
                    }),
                    topics = topics.Select(t => new
                    {
                        id = t.TopicId,
                        name = _topicService.GetTopicByTopicId(t.TopicId).Name
                    }),
                });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "组号格式错误" });
            }
        }

        public class TopicTemp
        {
            public long topicid { get; set; }
        }
        //测试成功
        //7.学生课堂选题——选题——RandomGroupChooseTopicUI
        //7.学生课堂选题——选题——RandomGroupChooseTopicUI2
        [HttpPost("/group/{groupId:long}/topic")]
        public IActionResult SelectTopic([FromRoute] long groupId, [FromBody]TopicTemp tt)
        {
            //seminargroupservice与shared返回值不同，自己修改
            try
            {
                //小组按Id选择话题
                _seminarGroupService.InsertTopicByGroupId(groupId, tt.topicid);
                return Created($"/group/{groupId}/topic/{tt.topicid}", new Dictionary<string, string> { ["url"] = $" /group/{groupId:long}/topic/{tt.topicid}" });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { mag = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "组号格式错误" });
            }
        }

        //测试成功
        //7.学生课堂选题——选题——RandomGroupChooseTopicUI
        //7.学生课堂选题——选题——RandomGroupChooseTopicUI2
        [HttpDelete("/group/{groupId:long}/topic/{topicId:long}")]
        public IActionResult DeselectTopic([FromRoute] long groupId, [FromRoute] long topicId)
        {
            try
            {
                //小组取消选择话题
                _topicService.DeleteSeminarGroupTopicById(groupId, topicId);
                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "组号或话题号格式错误" });
            }
        }

        //测试成功
        //8.学生课堂GradePresentationUI
        [HttpPut("/group/{groupId:long}/grade/report")]
        public IActionResult UpdateGradeByGroupId([FromRoute] long groupId, [FromBody] StudentScoreGroup updated)
        {
            try
            {
                //按ID设置小组打分
                //其中grade是reportgrade
                _gradeService.UpdateGroupByGroupId(groupId, (int)updated.Grade);
                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "组号格式错误" });
            }
        }
        
        //8.学生课堂GradePresentationEndUI
        [HttpPut("/group/grade/presentation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult SubmitStudentGradeByGroupId([FromBody] ScoreOnTopic sot)
        {
            try
            {
                //var groupid = _db.SeminarGroupTopic.Where(c => c.Id == sot.TopicId).ToList()[0].SeminarGroupId;
                //提交对其他小组的打分
                _gradeService.InsertGroupGradeByUserId(sot.TopicId, User.Id(), sot.GroupId, sot.Grade);
                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "组号格式错误" });
            }
        }

        public class Grade     //用于输出小组成绩
        { 
            public int? Reportgrade { get; set; }
            public int? Presentationgrade { get; set; }
            public int? Finalgrade { get; set; }
        }
        public class ScoreOnTopic     //用于打分
        {
            public long TopicId { get; set; }
            public long GroupId { get; set; }
            public int Grade { get; set; }
        }
        //随机分组添加成员
        //10.老师课堂——ClassManage——随机分组——GroupInfoUI
        //11.老师课堂——ClassManage——随机分组——GroupInfoUI2
        [HttpPut("/group/{groupId:long}/add")]
        public IActionResult InsertMemberByStudentId([FromRoute]long groupId, [FromBody] UserInfo updated)
        {
            try
            {
                //将学生加入讨论课小组
                _seminarGroupService.InsertSeminarGroupMemberById(updated.Id, groupId);
                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "组号格式错误" });
            }
        }


        //移除小组成员
        //11.老师课堂——ClassManage——随机分组——GroupInfoUI2
        [HttpPut("/group/{groupId:long}/remove")]
        public IActionResult RemoveMemberByStudentId([FromRoute]long groupId, [FromBody]UserInfo updated)
        {
            try
            {
                //删除小组成员
                _seminarGroupService.DeleteSeminarGroupMemberById(groupId, updated.Id);
                return NoContent();
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "组号格式错误" });
            }
        }


        //队长辞职
        //6.学生课堂——Seminar——FixedGroup——未完成的选题——FixedGroupLeaderUI2
        [HttpPut("/group/{groupId:long}/resign")]
        public IActionResult LeaderResign([FromRoute]long groupId)
        {
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                //同学按小组id和自身id,辞掉组长职位
                _seminarGroupService.ResignLeaderById(groupId, user.Id);
                return NoContent();
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "该学生不存在" });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "id格式错误" });
            }
        }

        //成为队长
        //6.学生课堂——Seminar——FixedGroup——未完成的选题——FixedGroupNoLeaderUI2
        [HttpPut("/group/{groupId:long}/assign")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult LeaderAssign([FromRoute]long groupId)
        {
            try
            {
                var user = _userService.GetUserByUserId(User.Id());
                //同学按小组id和自身id,辞掉组长职位
                _seminarGroupService.AssignLeaderById(groupId, user.Id);
                return NoContent();
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "该学生不存在" });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "该小组不存在" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "id格式错误" });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(400, new { msg = "小组已有组长" });
            }
        }


    }
}