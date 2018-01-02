using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    [Produces("application/json")]
    public class SchoolController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISchoolService _schoolService;

        public SchoolController(IUserService userService,ISchoolService schoolSevice)
        {
            _userService = userService;
            _schoolService = schoolSevice;
        }
        [HttpGet("/school")]
        public IActionResult GetSchools(string city)
        {
            IList<School> schools = _schoolService.ListSchoolByCity(city);
            return Json(schools);
        }

        [HttpGet("/school/province")]
        public IActionResult GetProvinces()
        {
            IList<string> _provinces = _schoolService.ListProvince();
            return Json(_provinces);
        }
        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("/school/city")]
        public IActionResult GetCitys(string province)
        {
            IList<string> _citys = _schoolService.ListCity(province);
            return Json(_citys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        [HttpGet("/school/{schoolId:long}")]
        public IActionResult GetSchoolById([FromRoute] long schoolId)
        {
            return Json(new School());
        }
        /// <summary>
        /// 添加学校
        /// </summary>
        /// <param name="newSchool"></param>
        /// <returns></returns>
        [HttpPost("/school")]
        public IActionResult CreateSchool([FromBody] School newSchool)
        {
                School school = new School()
                {
                    Province = newSchool.Province,
                    City = newSchool.City,
                    Name = newSchool.Name
                };
                school.Id = _schoolService.InsertSchool(school);
                return Json(school);
        }
        
    }
    
}