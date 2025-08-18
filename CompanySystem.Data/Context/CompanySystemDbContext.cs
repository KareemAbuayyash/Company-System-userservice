using Microsoft.EntityFrameworkCore;
using CompanySystem.Data.Models;

namespace CompanySystem.Data.Context
{
    public class CompanySystemDbContext : DbContext
    {
        public CompanySystemDbContext(DbContextOptions<CompanySystemDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<MainPageContent> MainPageContents { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepartmentId);
                entity.Property(e => e.DepartmentId).ValueGeneratedOnAdd();
                entity.Property(e => e.DepartmentName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                
                entity.HasIndex(e => e.DepartmentName);
            });

            modelBuilder.Entity<MainPageContent>(entity =>
            {
                entity.HasKey(e => e.ContentId);
                entity.Property(e => e.ContentId).ValueGeneratedOnAdd();
                entity.Property(e => e.SectionName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired().HasColumnType("TEXT");
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                
                entity.HasIndex(e => e.SectionName);
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.NoteId);
                entity.Property(e => e.NoteId).ValueGeneratedOnAdd();
                entity.Property(e => e.EmployeeId).IsRequired();
                entity.Property(e => e.NoteType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired().HasColumnType("TEXT");
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                
                entity.HasIndex(e => e.EmployeeId);
                entity.HasIndex(e => e.NoteType);
                entity.HasIndex(e => e.CreatedBy);
            });

            modelBuilder.Entity<Department>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<MainPageContent>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Note>().HasQueryFilter(e => !e.IsDeleted);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    DepartmentId = 1,
                    DepartmentName = "Human Resources",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Department
                {
                    DepartmentId = 2,
                    DepartmentName = "Information Technology",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                }
            );

            // Seed MainPageContent
            modelBuilder.Entity<MainPageContent>().HasData(
                new MainPageContent
                {
                    ContentId = 1,
                    SectionName = "Overview",
                    Title = "Welcome to Our Company",
                    Content = "We are a leading company in our industry, committed to excellence and innovation.",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                },
                new MainPageContent
                {
                    ContentId = 2,
                    SectionName = "AboutUs",
                    Title = "About Our Company",
                    Content = "Founded in 2020, we have been providing exceptional services to our clients worldwide.",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                },
                new MainPageContent
                {
                    ContentId = 3,
                    SectionName = "Services",
                    Title = "Our Professional Services",
                    Content = "We offer a comprehensive range of professional services including consulting and development.",
                    CreatedBy = "System",
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                }
            );
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is TrackingEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (TrackingEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedDate = DateTime.UtcNow;
                }
            }
        }
    }
}
