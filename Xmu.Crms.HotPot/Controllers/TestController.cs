using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Services.HotPot;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Xmu.Crms.HotPot.Controllers
{
    [Route("")]
    public class TestController : Controller
    {
        public IClassService _cs;

        //public CrmsContext _db;
        // GET: api/values
        public TestController(IClassService classService,ILoginService l,IGradeService gradeService)
        {
            _cs = classService;
        }
        [HttpGet(("/test"))]
        public void Get()
        {
            /*
             * 测试UpdateClassByClassId
            ClassInfo classInfo = new ClassInfo() {
                Id = 1,
                Name = "OOAD英文班",
                Site = "海韵101",
                ClassTime = "星期一下午五六节",
                ReportPercentage = 50,
                PresentationPercentage=50,
                FourPointPercentage=30,
                FivePointPercentage=20
            };
            var c=_cs.ListClassByCourseId(1);
            _cs.UpdateClassByClassId(c.First().Id, classInfo);
            */
            _cs.EndCallRollById(1, 1);
            System.Diagnostics.Debug.WriteLine("result");

        }

    }
}
