using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Xmu.Crms.Mobile.HotPot.Models
{
    public class ClassTime
    {
        public int Week { get; set; }
        public int Day { get; set; }
        public int Lesson { get; set; }
        public string Site { get; set; }
    }
}