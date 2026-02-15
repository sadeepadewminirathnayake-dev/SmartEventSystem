using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartEventSystem.Models;

namespace SmartEventSystem.Data
{
    public class SmartEventSystemContext : DbContext
    {
        public SmartEventSystemContext (DbContextOptions<SmartEventSystemContext> options)
            : base(options)
        {
        }

        public DbSet<SmartEventSystem.Models.Event> Event { get; set; } = default!;

        
    }
}
