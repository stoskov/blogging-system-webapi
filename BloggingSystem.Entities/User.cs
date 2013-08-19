using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BloggingSystem.Entities
{
	public class User
	{
		[Key]
		public int UserId { get; set; }

		[Required]
		public string Username { get; set; }

		[Required]
		public string DisplayName { get; set; }

		[Required]
		public string AuthCode { get; set; }

		public string SessionKey { get; set; }

		private ICollection<Post> posts;

		public virtual ICollection<Post> Posts
		{
			get
			{
				return this.posts;
			}
			set
			{
				this.posts = value;
			}
		}

		public User()
		{
			this.posts = new HashSet<Post>();
		}
	}
}