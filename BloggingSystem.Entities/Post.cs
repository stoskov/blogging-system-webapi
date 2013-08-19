using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BloggingSystem.Entities
{
	public class Post
	{
		[Key]
		public int PostId { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Text { get; set; }

		[Required]
		public DateTime PostDate { get; set; }

		[Required]
		public int UserId { get; set; }

		public virtual User User { get; set; }

		private ICollection<Comment> comments;

		public virtual ICollection<Comment> Comments
		{
			get
			{
				return this.comments;
			}
			set
			{
				this.comments = value;
			}
		}

		private ICollection<Tag> tags;

		public virtual ICollection<Tag> Tags
		{
			get
			{
				return this.tags;
			}
			set
			{
				this.tags = value;
			}
		}

		public Post()
		{
			this.comments = new HashSet<Comment>();
			this.tags = new HashSet<Tag>();
		}
	}
}