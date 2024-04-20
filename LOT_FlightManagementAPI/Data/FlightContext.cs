using LOT_FlightManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LOT_FlightManagementAPI.Data
{
    public class FlightContext : DbContext
    {
        public DbSet<Flight> Flights { get; set; }
        public FlightContext(DbContextOptions<FlightContext> options) : base(options) { }
        
        // let the Unit Tests modify the ID when they create the object 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flight>()
                .Property(f => f.FlightId)
                .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);
        }
    }
}