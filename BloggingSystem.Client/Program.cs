using System;
using System.Linq;
using BloggingSystem.Data;
using BloggingSystem.Entities;

namespace BloggingSystem.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			User user = new User()
			{
				DisplayName = "Displany Name",
				Username = "username",
				AuthCode = "123"
			};

			Post post = new Post()
			{
				User = user,
				Title = "title",
				Text = "text",
				PostDate = DateTime.Now,
			};

			BloggingSystemContext context = new BloggingSystemContext();
			context.Users.Add(user);
			context.Posts.Add(post);
			context.SaveChanges();
		}
	}
}