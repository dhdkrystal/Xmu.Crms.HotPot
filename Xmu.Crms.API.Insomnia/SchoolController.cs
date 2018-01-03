using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Insomnia
{
    [Produces("application/json")]

    public class SchoolController : Controller
    {
        private readonly ISchoolService _schoolService;

        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/school")]
        public IActionResult GetSchools([FromQuery] string city)
        {
            var schools = _schoolService.ListSchoolByCity(city);
            return Json(schools.Select(t => new
            {
                id = t.Id,
                name = t.Name,
                province = t.Province,
                city = t.City

            }));
        }

        [HttpGet(API.Insomnia.Constant.PREFIX + "/school/{schoolId:long}")]
        public IActionResult GetSchoolById([FromRoute] long schoolId)
        {
            try
            {
                var schoolinfo = _schoolService.GetSchoolBySchoolId(schoolId);
                return Json(new { name = schoolinfo.Name, province = schoolinfo.Province, city = schoolinfo.City });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "学校ID输入格式有误" });
            }
        }



        /*
         * 这里school的查找有问题
         */
        [HttpPost(API.Insomnia.Constant.PREFIX + "/school")]
        public IActionResult CreateSchool([FromBody] School newSchool)
        {
            var schoolId = _schoolService.InsertSchool(newSchool);
            return Created("school/" + schoolId, newSchool);
        }
    }
}