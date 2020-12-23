using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using NHiLo.Example.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHiLo.Example.Repository
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext()
            : base("NHiLoExample")
        {

        }

        public DbSet<Person> People { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // IDENTITY option is the Evil on Earth!
            modelBuilder.Entity<Person>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Person>().Property(p => p.Name).HasMaxLength(100);
            modelBuilder.Entity<Contact>().Property(p => p.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Contact>().Property(p => p.TelephoneNumber).HasMaxLength(10);
            modelBuilder.Entity<Contact>().HasRequired(c => c.Person);  // it will cascade the deletions
        }
    }
}
