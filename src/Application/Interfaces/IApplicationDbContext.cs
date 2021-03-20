using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Yaroshinski.Blog.Domain.Entities;

namespace Yaroshinski.Blog.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Author> Authors { get; set; }

        DbSet<Post> Posts { get; set; }

        DbSet<Tag> Tags { get; set; }

        DbSet<PostTag> PostTags { get; set; }

        DbSet<Comment> Comments { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
