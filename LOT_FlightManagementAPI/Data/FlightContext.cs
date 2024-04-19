using LOT_FlightManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LOT_FlightManagementAPI.Data
{
    public class FlightContext : DbContext
    {
        public FlightContext(DbContextOptions<FlightContext> options) : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
    }
}