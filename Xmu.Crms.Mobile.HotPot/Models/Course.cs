using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xmu.Crms.Mobile.HotPot.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public int NumClass { get; set; }
        public int NumStudent { get; set; }
        public DateTime StartTime { get; set ; }
        public DateTime EndTime { get; set ; }
        public string Description { get; set; }
        public Proportions Proportion { get; set; }   

    }
    public class Proportions
    {
        public int Id { get; set; }
        public int a { get; set; }
        public int b { get; set; }
        public int c { get; set; }
        public int report { get; set; }
        public int presentation { get; set; }
    }
}