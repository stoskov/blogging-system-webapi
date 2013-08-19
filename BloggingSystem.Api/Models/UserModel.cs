using System;
using System.Linq;
using System.Runtime.Serialization;

namespace BloggingSystem.Api.Models
{
	[DataContract]
	public class UserModel
	{
		[DataMember(Name = "username")]
		public string Username { get; set; }

		[DataMember(Name = "displayName")]
		public string DisplayName { get; set; }

		[DataMember(Name = "authCode")]
		public string AuthCode { get; set; }

		public UserModel()
		{
		}
	}
}