﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Xmu.Crms.Mobile.HotPot.Models
{
    public class Xmu.Crms.Mobile.HotPotContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public Xmu.Crms.Mobile.HotPotContext() : base("name=Xmu.Crms.Mobile.HotPotContext")
        {
        }

        public System.Data.Entity.DbSet<Xmu.Crms.Mobile.HotPot.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<Xmu.Crms.Mobile.HotPot.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<Xmu.Crms.Mobile.HotPot.Models.Class> Classes { get; set; }
    }
}
