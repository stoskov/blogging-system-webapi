using System;
using System.Data.Entity;
using System.Linq;
using BloggingSystem.Data.Migrations;

namespace BloggingSystem.Data
{
	public class BloggingSystemContext : DbContext
	{
		public BloggingSystemContext()
			: base("BloggingSystem")
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<BloggingSystemContext, Configuration>());
		}
	}
}