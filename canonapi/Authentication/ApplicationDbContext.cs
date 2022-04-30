//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using canonapi.Models;
using Microsoft.EntityFrameworkCore;

namespace canonapi.Authentication
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Image> images { get; set; }
        public DbSet<Imagebackup> imagesbackup { get; set; }
        public DbSet<ImageDrByUser> imagedrbyusers { get; set; }
        public DbSet<Datasets> datasets { get; set; }
        public DbSet<DatasetMap> datasetmap { get; set; }
        public DbSet<Sessions> sessions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var connetionString = Configuration.GetConnectionString("DefaultConnection");
                //options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
                modelBuilder.UseMySql(connetionString);

            // Map entities to tables  
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Image>().ToTable("images");

            // Configure Primary Keys  
            modelBuilder.Entity<User>().HasKey(u => u.id);
            modelBuilder.Entity<Image>().HasKey(i => i.id);

            // Configure indexes  
            modelBuilder.Entity<User>().HasIndex(p => p.username).IsUnique();

            // Configure columns  
            modelBuilder.Entity<Image>().Property(i => i.id).HasColumnType("bigint").UseMySqlIdentityColumn().IsRequired();
            modelBuilder.Entity<Image>().Property(i => i.imagename).HasColumnType("varchar(500)").IsRequired(false);
            modelBuilder.Entity<Image>().Property(i => i.drlevel_kaggle).HasColumnType("int").IsRequired();
            modelBuilder.Entity<Image>().Property(i => i.drlevel_sushrut).HasColumnType("int").IsRequired();
            modelBuilder.Entity<Image>().Property(i => i.imageurl).HasColumnType("varchar(1024)").IsRequired(false);

            modelBuilder.Entity<User>().Property(u => u.id).HasColumnType("int").UseMySqlIdentityColumn().IsRequired();
            modelBuilder.Entity<User>().Property(u => u.username).HasColumnType("varchar(50)").IsRequired();
            modelBuilder.Entity<User>().Property(u => u.userpassword).HasColumnType("varchar(50)").IsRequired();
            modelBuilder.Entity<User>().Property(u => u.firstname).HasColumnType("varchar100)").IsRequired();
            modelBuilder.Entity<User>().Property(u => u.lastname).HasColumnType("varchar100").IsRequired(false);
        }*/
    }
}
