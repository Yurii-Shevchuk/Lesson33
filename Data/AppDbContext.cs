using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Lesson_33_MVC.Data.Models;

namespace Lesson_33_MVC.Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Avatar> Avatars { get; set; }

    public AppDbContext() : base()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
 
        optionsBuilder.UseInMemoryDatabase("in_memory");
    }
}