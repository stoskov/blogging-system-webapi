using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BloggingSystem.Api.Models;
using BloggingSystem.Data;

namespace BloggingSystem.Api.Controllers
{
	public class TagsController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage Get(string sessionKey)
		{
			try
			{
				this.VerifySessionKey(sessionKey);
			}
			catch (Exception e)
			{
				var errorResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
				return errorResponse;
			}

			var context = new BloggingSystemContext();

			var tagsResult = from tag in context.Tags
							 orderby tag.Text
							 select new SimpleTagModel()
							 {
								 Id = tag.TagId,
								 Name = tag.Text,
								 PostsCount = tag.Posts.Count
							 };

			var response = this.Request.CreateResponse(HttpStatusCode.OK, tagsResult);

			return response;
		}

		[HttpGet]
		public HttpResponseMessage GetPosts(int tagId, string sessionKey)
		{
			var context = new BloggingSystemContext();
			var tag = context.Tags.FirstOrDefault(t => t.TagId == tagId);

			try
			{
				this.VerifySessionKey(sessionKey);

				if (tag == null)
				{
					throw new ArgumentOutOfRangeException("Tag not found");
				}
			}
			catch (Exception e)
			{
				var errorResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
				return errorResponse;
			}

			var posts = tag.Posts;

			var postsResult =
				(from post in posts
				 orderby post.PostDate descending
				 select new PostModel()
				 {
					 Id = post.PostId,
					 Title = post.Title,
					 Text = post.Text,
					 PostDate = post.PostDate,
					 PostedBy = post.User.DisplayName,
					 Tags = (from t in post.Tags
							 select t.Text),
					 Comments = (from comment in post.Comments
								 orderby comment.PostDate descending
								 select new CommentModel()
								 {
									 Text = comment.Text,
									 CommentedBy = comment.User.DisplayName,
									 PostDate = comment.PostDate
								 })
				 });

			var response = this.Request.CreateResponse(HttpStatusCode.Created, postsResult);

			return response;
		}

		private void VerifySessionKey(string sessionKey)
		{
			Helpers.Validation.VerifySessionKey(sessionKey);
		}
	}
}