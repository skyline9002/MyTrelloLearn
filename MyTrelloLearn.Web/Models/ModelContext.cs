using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTrelloLearn.Web.Models
{
    using System.Data.Entity;

    public class ModelContext : DbContext
    {
        public ModelContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}