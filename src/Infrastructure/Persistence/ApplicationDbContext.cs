using Microsoft.EntityFrameworkCore;
using Yaroshinski.Blog.Application.Interfaces;
using Yaroshinski.Blog.Domain.Entities;
using Yaroshinski.Blog.Infrastructure.Persistence.Configuration;

namespace Yaroshinski.Blog.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CommentConfiguration());
            builder.Entity<PostTag>()
                .HasKey((entity) => new { entity.PostId, entity.TagId});
        }
    }
}
