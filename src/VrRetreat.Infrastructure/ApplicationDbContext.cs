using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VrRetreat.Infrastructure.Entities;

namespace VrRetreat.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<VrRetreatUser>
    {
        private readonly string _connStringFile = string.Empty;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, string connStringFile)
            : base(options)
        {
            _connStringFile = connStringFile;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if(!string.IsNullOrEmpty(_connStringFile))
                optionsBuilder.UseSqlServer(File.ReadAllText(_connStringFile));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
