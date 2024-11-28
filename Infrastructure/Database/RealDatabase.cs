﻿using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    public class RealDatabase : DbContext
    {

        public RealDatabase(DbContextOptions<RealDatabase> options)
        : base(options) 
        {
        }


        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        //public DbSet<User> Users { get; set; }


    }
}
