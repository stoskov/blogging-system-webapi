using System;
using System.Data.Entity;
using System.Linq;
using BloggingSystem.Data.Migrations;
using BloggingSystem.Entities;

namespace BloggingSystem.Data
{
	public class BloggingSystemContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public DbSet<Post> Posts { get; set; }

		public DbSet<Comment> Comments { get; set; }

		public DbSet<Tag> Tags { get; set; }

		public BloggingSystemContext()
			: base("BloggingSystem")
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<BloggingSystemContext, Configuration>());
		}
	}
}