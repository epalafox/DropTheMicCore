using Microsoft.IdentityModel.Tokens;
using System;

namespace DropTheMicCore.Areas.api.Models
{
	public class TokenProviderOptionsModel
	{
		public string Issuer { get; set; }

		public string Audience { get; set; }

		public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);

		public SigningCredentials SigningCredentials { get; set; }
	}
}
