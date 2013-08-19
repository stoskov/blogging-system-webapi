using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BloggingSystem.Entities
{
	public class Tag
	{
		[Key]
		public int TagId { get; set; }

		[Required]
		public string Text { get; set; }

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

		public Tag()
		{
			this.posts = new HashSet<Post>();
		}
	}
}