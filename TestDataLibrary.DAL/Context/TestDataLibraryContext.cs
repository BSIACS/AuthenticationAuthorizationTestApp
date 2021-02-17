using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TestDataLibrary.Domain.Entities;

namespace TestDataLibrary.DAL.Context
{
    public class TestDataLibraryContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public TestDataLibraryContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }


    }
}
