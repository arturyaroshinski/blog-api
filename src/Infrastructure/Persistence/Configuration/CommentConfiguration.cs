using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Infrastructure.Persistence.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasOne(e => e.Author)
                .WithMany(p => p.Comments)
                .HasForeignKey(u => u.AuthorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(u => u.PostId)
                .IsRequired();
        }
    }
}
