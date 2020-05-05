using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AcutePediatricsOrientation.Models
{
    public class AcutePediatricsContext : DbContext
    {
        public AcutePediatricsContext (DbContextOptions<AcutePediatricsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Signature>().HasKey(table => new {
                table.UserId,
                table.TopicId
            });
        }

        public DbSet<AcutePediatricsOrientation.Models.Account> Account { get; set; }
        public DbSet<AcutePediatricsOrientation.Models.Category> Category { get; set; }
        public DbSet<AcutePediatricsOrientation.Models.Topic> Topic { get; set; }
        public DbSet<AcutePediatricsOrientation.Models.Documents> Document { get; set; }
        public DbSet<AcutePediatricsOrientation.Models.DocumentType> DocumentType { get; set; }
        public DbSet<AcutePediatricsOrientation.Models.Signature> Signature { get; set; }
        public DbSet<AcutePediatricsOrientation.Models.Role> Role { get; set; }
    }
}
