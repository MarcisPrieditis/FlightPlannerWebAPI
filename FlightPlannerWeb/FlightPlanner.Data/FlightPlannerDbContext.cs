using System.Threading.Tasks;
using FlightPlanner.Core.Models;
using FlightPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Data
{
    public class FlightPlannerDbContext : DbContext, IFlightPlannerDbContext
    {
        public FlightPlannerDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Airport> Airports { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}