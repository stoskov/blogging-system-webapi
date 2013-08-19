using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using BloggingSystem.Api.Models;
using BloggingSystem.Data;
using BloggingSystem.Entities;

namespace BloggingSystem.Api.Controllers
{
	public class PostsController : ApiController
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

			var postsResult = this.GetAllPosts();
			var response = this.Request.CreateResponse(HttpStatusCode.OK, postsResult);

			return response;
		}

		[HttpGet]
		public HttpResponseMessage GetById(int id, string sessionKey)
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

			var postsResult = this.GetAllPosts().Where(p => p.Id == id);
			var response = this.Request.CreateResponse(HttpStatusCode.OK, postsResult);

			return response;
		}

		[HttpGet]
		public HttpResponseMessage GetPaged(int count, int page, string sessionKey)
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

			var postsResult = this.GetAllPosts().Skip(page * count).Take(count);
			var response = this.Request.CreateResponse(HttpStatusCode.OK, postsResult);

			return response;
		}

		[HttpGet]
		public HttpResponseMessage GetByKeyword(string keyword, string sessionKey)
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

			var postsResult = this.GetAllPosts().Where(p => p.Title.Contains(keyword));
			var response = this.Request.CreateResponse(HttpStatusCode.OK, postsResult);

			return response;
		}

		[HttpGet]
		public HttpResponseMessage GetByTags(string tags, string sessionKey)
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

			var tagsList = tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			var postsResult = this.GetAllPosts().Where(p => tagsList.All(tag => p.Tags.Contains(tag)));

			var response = this.Request.CreateResponse(HttpStatusCode.OK, postsResult);

			return response;
		}

		[HttpPost]
		public HttpResponseMessage Post(PostModel model, string sessionKey)
		{
			try
			{
				this.VerifySessionKey(sessionKey);

				if (string.IsNullOrEmpty(model.Title))
				{
					throw new ArgumentNullException("Post title is mandatory");
				}

				if (string.IsNullOrEmpty(model.Text))
				{
					throw new ArgumentNullException("Post content is mandatory");
				}
			}
			catch (Exception e)
			{
				var errorResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
				return errorResponse;
			}

			var context = new BloggingSystemContext();

			var user = context.Users.FirstOrDefault(
				usr => usr.SessionKey == sessionKey);

			var post = new Post()
			{
				Title = model.Title,
				Text = model.Text,
				PostDate = DateTime.Now,
				User = user
			};

			var matches = Regex.Matches(model.Title, @"\b[a-zA-Z]{1,}\b");
			foreach (Match match in matches)
			{
				var tag = context.Tags.FirstOrDefault(t => t.Text == match.Value);

				if (tag == null)
				{
					tag = new Tag()
					{
						Text = match.Value
					};
				}

				post.Tags.Add(tag);
			}

			foreach (string tagText in model.Tags)
			{
				var tag = context.Tags.FirstOrDefault(t => t.Text == tagText);

				if (tag == null)
				{
					tag = new Tag()
					{
						Text = tagText
					};
				}

				post.Tags.Add(tag);
			}

			context.Posts.Add(post);
			context.SaveChanges();

			var result = new PostResponseModel()
			{
				Id = post.PostId,
				Title = post.Title
			};

			var response = this.Request.CreateResponse(HttpStatusCode.Created, result);

			return response;
		}

		[HttpPost]
		public HttpResponseMessage PostComment(int postId, CommentModel model, string sessionKey)
		{
			var context = new BloggingSystemContext();
			var post = context.Posts.FirstOrDefault(p => p.PostId == postId);

			try
			{
				this.VerifySessionKey(sessionKey);

				if (post == null)
				{
					throw new ArgumentOutOfRangeException("Post not found");
				}

				if (string.IsNullOrEmpty(model.Text))
				{
					throw new ArgumentNullException("Comment text is mandatory");
				}
			}
			catch (Exception e)
			{
				var errorResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
				return errorResponse;
			}

			var user = context.Users.FirstOrDefault(
				usr => usr.SessionKey == sessionKey);

			var comment = new Comment()
			{
				Text = model.Text,
				PostDate = DateTime.Now,
				User = user,
				Post = post
			};

			context.Comments.Add(comment);
			context.SaveChanges();

			var response = this.Request.CreateResponse(HttpStatusCode.Created);

			return response;
		}

		private IQueryable<PostModel> GetAllPosts()
		{
			var context = new BloggingSystemContext();
			var posts = context.Posts;

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
					 Tags = (from tag in post.Tags
							 select tag.Text),
					 Comments = (from comment in post.Comments
								 orderby comment.PostDate descending
								 select new CommentModel()
								 {
									 Text = comment.Text,
									 CommentedBy = comment.User.DisplayName,
									 PostDate = comment.PostDate
								 })
				 });

			return postsResult;
		}

		private void VerifySessionKey(string sessionKey)
		{
			Helpers.Validation.VerifySessionKey(sessionKey);
		}
	}
}