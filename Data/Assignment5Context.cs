using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Assignment5.Models;

namespace Assignment5.Data
{
    public class Assignment5Context : DbContext
    {
        public Assignment5Context (DbContextOptions<Assignment5Context> options)
            : base(options)
        {
        }

        public DbSet<Assignment5.Models.Song> Song { get; set; } = default!;
    }
}
