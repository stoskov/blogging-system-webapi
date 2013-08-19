namespace BloggingSystem.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<BloggingSystem.Data.BloggingSystemContext>
	{
		public Configuration()
		{
			this.AutomaticMigrationsEnabled = true;
		}

		protected override void Seed(BloggingSystem.Data.BloggingSystemContext context)
		{
		}
	}
}