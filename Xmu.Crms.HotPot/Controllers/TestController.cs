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
        public ClassService _cs;
        public CrmsContext _db;
        // GET: api/values
        public TestController(ClassService classService)
        {
            _cs = classService;
        }
        [HttpGet(("/test"))]
        public void Get()
        {
            var location=_cs.ListClassByUserId(4);
            Console.WriteLine(location);
            
        }

    }
}
