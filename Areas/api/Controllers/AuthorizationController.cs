using System;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using DropTheMicCore.Areas.api.Models;

namespace EvaaCore.Areas.Api.Controllers
{
	[Produces("application/json")]
	[Route("api/login")]
	public class AuthorizationController : Controller
	{
		//TODO: Mover a variable segura
		private static readonly string secretKey = "MISUPEREXTRADIFFICULTKEY";

		public ActionResult Get(string username, string password)
		{
			// Add JWT generation endpoint:
			var keyByteArray = Encoding.ASCII.GetBytes(secretKey);
			var signingKey = new SymmetricSecurityKey(keyByteArray);
			var options = new TokenProviderOptionsModel
			{
				Audience = "DropTheMic",
				Issuer = "DropTheMicCore",
				SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
			};
			ClaimsIdentity claim;
			if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
			{
				//if (user == null)
				//{
				//	return NotFound();
				//}

				System.Security.Principal.GenericIdentity identity = new System.Security.Principal.GenericIdentity(username, "Token");

				claim = new ClaimsIdentity(identity, new Claim[] { });
			}
			else
			{
				return NotFound();
			}

			var now = DateTime.UtcNow;

			// Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
			// You can add other claims here, if you want:
			var claims = new Claim[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, username),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iat, now.Second.ToString(), ClaimValueTypes.Integer64),
				new Claim("idUser", "111"),
			};

			// Create the JWT and write it to a string
			var jwt = new JwtSecurityToken(
				issuer: options.Issuer,
				audience: options.Audience,
				claims: claims,
				notBefore: now,
				expires: now.Add(options.Expiration),
				signingCredentials: options.SigningCredentials);
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return Ok(new
			{
				access_token = encodedJwt,
				expires_in = (int)options.Expiration.TotalSeconds
			});
		}
	}
}