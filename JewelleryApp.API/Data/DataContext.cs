using JewelleryApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelleryApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base
        (options)
        {

        }

        public DbSet<Value> Values { get; set; } //Values names references the table name that gets craeted when we scaffolded the database
    }
}