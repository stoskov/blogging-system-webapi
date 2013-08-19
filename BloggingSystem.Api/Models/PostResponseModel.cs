using System;
using System.Linq;
using System.Runtime.Serialization;

namespace BloggingSystem.Api.Models
{
	[DataContract]
	public class PostResponseModel
	{
		[DataMember(Name = "id")]
		public int Id { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }
	}
}