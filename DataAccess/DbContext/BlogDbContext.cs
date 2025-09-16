using ApplicationCore.Entities.Concrete;
using ApplicationCore.UserEntites.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataAccess.DbContext
{
    public class BlogDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }


        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Tag> Tags { get; set; } = default!;
        public DbSet<PostTag> PostTags { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);


            b.Entity<PostTag>(e =>
            {
                e.HasKey(x => new { x.PostId, x.TagId });
                e.HasOne(x => x.Post).WithMany(p => p.PostTags)
                                     .HasForeignKey(x => x.PostId)
                                     .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Tag).WithMany(t => t.PostTags)
                                    .HasForeignKey(x => x.TagId)
                                    .OnDelete(DeleteBehavior.Cascade);
            });


            b.Entity<Post>(e =>
            {
                e.HasIndex(x => x.Slug).IsUnique();

                e.HasOne(x => x.Category)
                 .WithMany(c => c.Posts)
                 .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict); 

                e.HasOne(x => x.Author)
                 .WithMany() 
                 .HasForeignKey(x => x.AuthorId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            b.Entity<Category>(e => { e.HasIndex(x => x.Slug).IsUnique(); });
            b.Entity<Tag>(e => { e.HasIndex(x => x.Slug).IsUnique(); });

            b.Entity<Comment>(e =>
            {
                e.HasOne(x => x.Post)
                 .WithMany(p => p.Comments)
                 .HasForeignKey(x => x.PostId)
                 .OnDelete(DeleteBehavior.Cascade);
            });



        }
    }
}
