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

			var postsResult = this.GetAllPosts();
			var response = this.Request.CreateResponse(HttpStatusCode.OK, postsResult);

			return response;
		}


		private void VerifySessionKey(string sessionKey)
		{
			var context = new BloggingSystemContext();

			var user = context.Users.FirstOrDefault(
				usr => usr.SessionKey == sessionKey);

			if (user == null)
			{
				throw new ArgumentOutOfRangeException("You must be logged in to see this content");
			}
		}
	}
}