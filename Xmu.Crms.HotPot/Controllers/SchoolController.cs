using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class SchoolController : Controller
    {
        /*
        [HttpGet("/school")]
        public IActionResult GetSchools([FromQuery] string city)
        {
            var s1 = new School()
            {
                Name="厦门市人民公园",
                Province="福建",
                City="厦门"
            };
            return Json(new List<School>{s1});
        }

        [HttpGet("/school/{schoolId:long}")]
        public IActionResult GetSchoolById([FromRoute] long schoolId)
        {
            return Json(new School());
        }

        [HttpPost("/school")]
        public IActionResult CreateSchool([FromBody] School newSchool)
        {
            return Created("/school/1", newSchool);
        }
        */
    }
    
}