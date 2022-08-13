using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcCalorie.Models;

namespace MvcCalorie.Data
{
    public class MvcCalorieContext : DbContext
    {
        public MvcCalorieContext (DbContextOptions<MvcCalorieContext> options)
            : base(options)
        {
        }

        public DbSet<MvcCalorie.Models.Portion>? Portion { get; set; }
    }
}
