using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlantMania.Web.Models;

namespace PlantMania.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostTag> PostTags => Set<PostTag>();
        public DbSet<Answer> Answers => Set<Answer>();
        public DbSet<Comment> Comments => Set<Comment>();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // IMPORTANT: keep Identity configuration
            base.OnModelCreating(builder);

            // Composite key for many-to-many
            builder.Entity<PostTag>()
                .HasKey(x => new { x.PostId, x.TagId });

            // Unique slugs
            builder.Entity<Category>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder.Entity<Tag>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder.Entity<Post>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder.Entity<Article>()
                .HasIndex(x => x.Slug)
                .IsUnique();
        }
    }
}
