using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eco.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EcoDbContext>
    {
        public EcoDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EcoDbContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-I89TMU1\\SQLEXPRESS;Database=EcoStudentDB;Trusted_Connection=true;TrustServerCertificate=true;");
            
            return new EcoDbContext(optionsBuilder.Options);
        }
    }
}
