using System;
using System.Linq;
using System.Runtime.Serialization;

namespace BloggingSystem.Api.Models
{
	[DataContract]
	public class LoggedUserModel
	{
		[DataMember(Name = "displayName")]
		public string DisplayName { get; set; }

		[DataMember(Name = "sessionKey")]
		public string SessionKey { get; set; }

		public LoggedUserModel()
		{
		}
	}
}