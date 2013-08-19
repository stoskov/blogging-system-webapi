using System;
using System.Linq;
using BloggingSystem.Data;

namespace BloggingSystem.Api.Helpers
{
	public static class Validation
	{
		public static void VerifySessionKey(string sessionKey)
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