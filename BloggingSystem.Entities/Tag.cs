using System;
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

		[Required]
		public int PostId { get; set; }

		public virtual Post Post { get; set; }

		public Tag()
		{
		}
	}
}