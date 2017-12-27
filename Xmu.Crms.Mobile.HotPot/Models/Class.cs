using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Xmu.Crms.Mobile.HotPot.Models
{
    public class Class
    {
        //班级的唯一ID
        public int Id { get; set; }
        //班级的名称
        public string Name { get; set; }
        //班级的人数
        public int NumStudent { get; set; }
        public ClassTime TimeSite { get; set; }
        public bool Calling { get; set; }
        public string Roster { get; set; }
        public Proportions Proportions { get; set; }
        
    }

}