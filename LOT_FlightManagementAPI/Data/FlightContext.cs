using LOT_FlightManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LOT_FlightManagementAPI.Data
{
    public class FlightContext : DbContext
    {
        public DbSet<Flight> Flights { get; set; }
        public FlightContext(DbContextOptions<FlightContext> options) : base(options)
        {
        }
    }
}