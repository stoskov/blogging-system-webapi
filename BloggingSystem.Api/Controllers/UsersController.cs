using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using BloggingSystem.Api.Models;
using BloggingSystem.Data;
using BloggingSystem.Entities;

namespace BloggingSystem.Api.Controllers
{
	public class UsersController : ApiController
	{
		private static readonly Random rand = new Random();

		private const int MinUsernameLength = 6;
		private const int MaxUsernameLength = 30;
		private const int MinDislpayNameLength = 6;
		private const int MaxDislpayNameLength = 30;
		private const string ValidUsernameCharacters = "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.";
		private const string ValidDisplayNameCharacters = "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_. -";
		private const string SessionKeyChars = "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
		private const int SessionKeyLength = 50;
		private const int Sha1Length = 40;

		/*
		{  "username": "DonchoMinkov",
		"displayName": "Doncho Minkov",
		"authCode":   "bfff2dd4f1b310eb0dbf593bd83f94dd8d34077e" }
		*/

		[HttpPost]
		[ActionName("register")]
		public HttpResponseMessage RegisterUser(UserModel model)
		{
			var context = new BloggingSystemContext();
			var usernameLower = model.Username.ToLower();
			var displayNameLower = model.DisplayName.ToLower();

			try
			{
				this.ValidateUsername(model.Username);
				this.ValidateDisplayName(model.DisplayName);
				this.ValidateAuthCode(model.AuthCode);

				var existingUser = context.Users.FirstOrDefault(
					usr => usr.Username == usernameLower ||
						   usr.DisplayName.ToLower() == displayNameLower);

				if (existingUser != null)
				{
					throw new InvalidOperationException("Users exists");
				}
			}
			catch (Exception e)
			{
				var errorResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
				return errorResponse;
			}

			var user = new User()
			{
				Username = usernameLower,
				DisplayName = model.DisplayName,
				AuthCode = model.AuthCode
			};

			context.Users.Add(user);
			context.SaveChanges();

			user.SessionKey = this.GenerateSessionKey(user.UserId);
			context.SaveChanges();

			var loggedModel = new LoggedUserModel()
			{
				DisplayName = user.DisplayName,
				SessionKey = user.SessionKey
			};

			var response = this.Request.CreateResponse(HttpStatusCode.Created, loggedModel);

			return response;
		}

		[HttpPost]
		[ActionName("login")]
		public HttpResponseMessage LoginUser(UserModel model)
		{
			var context = new BloggingSystemContext();
			var usernameLower = model.Username.ToLower();

			var user = context.Users.FirstOrDefault(
				usr => usr.Username == usernameLower ||
					   usr.AuthCode == model.AuthCode);

			try
			{
				if (user == null)
				{
					throw new InvalidOperationException("Invalid username and/or password");
				}
			}
			catch (Exception e)
			{
				var errorResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
				return errorResponse;
			}

			if (user.SessionKey == null)
			{
				user.SessionKey = this.GenerateSessionKey(user.UserId);
				context.SaveChanges();
			}

			var loggedModel = new LoggedUserModel()
			{
				DisplayName = user.DisplayName,
				SessionKey = user.SessionKey
			};

			var response = this.Request.CreateResponse(HttpStatusCode.Created, loggedModel);

			return response;
		}

		[HttpPut]
		[ActionName("logout")]
		public HttpResponseMessage LogoutUser(string sessionKey)
		{
			var context = new BloggingSystemContext();

			var user = context.Users.FirstOrDefault(
				usr => usr.SessionKey == sessionKey);

			if (user != null)
			{
				user.SessionKey = null;
				context.SaveChanges();

				var response = this.Request.CreateResponse(HttpStatusCode.OK);
				return response;
			}
			else
			{
				var response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Wrong session");
				return response;
			}
		}

		private string GenerateSessionKey(int userId)
		{
			StringBuilder skeyBuilder = new StringBuilder(SessionKeyLength);
			skeyBuilder.Append(userId);
			while (skeyBuilder.Length < SessionKeyLength)
			{
				var index = rand.Next(SessionKeyChars.Length);
				skeyBuilder.Append(SessionKeyChars[index]);
			}
			return skeyBuilder.ToString();
		}

		private void ValidateAuthCode(string authCode)
		{
			if (authCode == null || authCode.Length != Sha1Length)
			{
				throw new ArgumentOutOfRangeException("Password should be encrypted with SHA1");
			}
		}

		private void ValidateDisplayName(string displayName)
		{
			if (displayName == null)
			{
				throw new ArgumentNullException("Display name cannot be empty");
			}
			else if (displayName.Length < MinDislpayNameLength)
			{
				throw new ArgumentOutOfRangeException(string.Format("Display name must be at least {0} characters long", MinDislpayNameLength));
			}
			else if (displayName.Length > MaxDislpayNameLength)
			{
				throw new ArgumentOutOfRangeException(string.Format("Display name must be less than {0} characters long", MaxDislpayNameLength));
			}
			else if (displayName.Any(ch => !ValidDisplayNameCharacters.Contains(ch)))
			{
				throw new ArgumentOutOfRangeException("Display name must contain only Latin letters, digits, dot, space, dash, and underscore");
			}
		}

		private void ValidateUsername(string username)
		{
			if (username == null)
			{
				throw new ArgumentNullException("Username cannot be empty");
			}
			else if (username.Length < MinUsernameLength)
			{
				throw new ArgumentOutOfRangeException(string.Format("Username must be at least {0} characters long", MinUsernameLength));
			}
			else if (username.Length > MaxUsernameLength)
			{
				throw new ArgumentOutOfRangeException(string.Format("Username must be less than {0} characters long", MaxUsernameLength));
			}
			else if (username.Any(ch => !ValidUsernameCharacters.Contains(ch)))
			{
				throw new ArgumentOutOfRangeException("Username must contain only Latin letters, digits, dot, and underscore");
			}
		}
	}
}