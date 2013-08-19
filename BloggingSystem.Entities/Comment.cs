using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BloggingSystem.Entities
{
	public class Comment
	{
		[Key]
		public int CommentId { get; set; }

		[Required]
		public string Text { get; set; }

		[Required]
		public DateTime PostDate { get; set; }

		[Required]
		public int UserId { get; set; }

		public virtual User User { get; set; }

		[Required]
		public int PostId { get; set; }

		public virtual Post Post { get; set; }

		public Comment()
		{
		}
	}
}