using Microsoft.Data.Entity;
//using Microsoft.Data.Entity.Infrastructure;
//using Microsoft.IdentityModel.Protocols;

namespace ImageGallery.Models
{
    public class ApplicationDBContext : DbContext
    {
        //public ApplicationDBContext(DbContextOptions options): base(options)
        //{ 
        //}
    	public ApplicationDBContext()
	    {
		    Database.EnsureCreated();
	    }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var connectionString = Startup.Configuration["Data:DefaultConnection:ConnectionString"];
            optionsBuilder.UseSqlServer(Startup.Configuration["Data:DefaultConnection:ConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>().HasKey(p => p.Id);
            //modelBuilder.Entity<Category>().HasKey(pc => new { pc.CategoryId, pc.ProductId });
            base.OnModelCreating(modelBuilder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<Photo> Photos { get; set; }
    }
}
