using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xmu.Crms.Shared.Models;
namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class SeminarController : Controller
    {
        [HttpGet("/seminar/{seminarId:long}")]
        public IActionResult GetSeminarByIdRandom([FromRoute] long seminarId)
        {
            var s1 = new Seminar();
            var s2 = new Seminar();
            return Json(seminarId == 1 ? s2 : s1);
        }

        [HttpDelete("/seminar/{seminarId:long}")]
        public IActionResult DeleteSeminarById([FromRoute] long seminarId)
        {
            return NoContent();
        }

        [HttpPut("/seminar/{seminarId:long}")]
        public IActionResult UpdateSeminarById([FromRoute] long seminarId, [FromBody] Seminar updated)
        {
            return NoContent();
        }

        [HttpGet("/seminar/{seminarId:long}/topic")]
        public IActionResult GetTopicsBySeminarId([FromRoute] long seminarId)
        {
            return Json(new List<Topic>());
        }

        [HttpPut("/seminar/{seminarId:long}/topic")]
        public IActionResult CreateTopicBySeminarId([FromRoute] long seminarId, [FromBody] Seminar newSeminar)
        {
            return Created("/topic/1", newSeminar);
        }

        [HttpGet("/seminar/{seminarId:long}/group")]
        public IActionResult GetGroupsBySeminarId([FromRoute] long seminarId)
        {
            return Json(new List<SeminarGroup>(), Utils.Ignoring("Group*", "Members", "Leader", "Report", "Grade"));
        }
    }
}