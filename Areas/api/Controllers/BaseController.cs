using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DropTheMicCore.Areas.api.Controllers
{
    [Produces("application/json")]
	[Authorize("Bearer")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class BaseController : Controller
	{
		internal long IdUser
		{
			get
			{
				var accessToken = User.Claims.ToList();
				return long.Parse(accessToken.FirstOrDefault(x => x.Type == "idUser").Value);
			}
		}
	}
}