using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

namespace DropTheMicCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
		}
		private static readonly string secretKey = "MISUPEREXTRADIFFICULTKEY";

		public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

			services.AddSession();
			#region Api Keys
			byte[] keyByteArray = Encoding.ASCII.GetBytes(secretKey);
			SymmetricSecurityKey signingKey = new SymmetricSecurityKey(keyByteArray);

			services.AddAuthorization(auth =>
			{
				auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
					.RequireAuthenticatedUser()
					.Build());
			});

			TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
			{
				// LLave de firma
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = signingKey,

				// iss Issuer
				ValidateIssuer = true,
				ValidIssuer = "DropTheMicCore",

				// aud Audiencia
				ValidateAudience = true,
				ValidAudience = "DropTheMic",

				// Expiración
				ValidateLifetime = true,

				// ClockDrift
				ClockSkew = TimeSpan.Zero
			};
			//Agregamos nuestra autentificacion
			services.AddAuthentication(options => {
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = tokenValidationParameters;
				});

			#endregion

			services.AddMvc().AddJsonOptions(options => {
				options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				})
				.AddSessionStateTempDataProvider(); ;
			services.AddLogging();
			#region Swagger
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Drop The Mic API", Version = "v1" });
				var basePath = PlatformServices.Default.Application.ApplicationBasePath;
				var xmlPath = Path.Combine(basePath, "DropTheMicCore.xml");
				c.IncludeXmlComments(xmlPath);
			});
			#endregion
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			app.UseStaticFiles();

			app.UseSession();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
				routes.MapRoute(
					name: "apiRoute",
					template: "api/{controller:exists}"
					);
			});

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseAuthentication();
		}
    }
}
