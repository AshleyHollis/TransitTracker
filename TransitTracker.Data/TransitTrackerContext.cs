using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TransitTracker.Data.Models;

namespace TransitTracker.Data
{
    public class TransitTrackerContext : DbContext
    {
        public TransitTrackerContext() : base("TransitTrackerContext")
        {
        }

        public DbSet<VehiclePosition> VehiclePositions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}