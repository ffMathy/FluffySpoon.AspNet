﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Certes;
using Certes.Acme;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FluffySpoon.AspNet.LetsEncrypt.Sample
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddFluffySpoonLetsEncrypt(new LetsEncryptOptions()
			{
				Email = "some-email@github.com",
				UseStaging = true,
				Domains = new[] { Program.DomainToUse },
				TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30),
				CertificateSigningRequest = new CsrInfo()
				{
					CountryName = "CountryNameStuff",
					Locality = "LocalityStuff",
					Organization = "OrganizationStuff",
					OrganizationUnit = "OrganizationUnitStuff",
					State = "StateStuff"
				}
			});
			services.AddFluffySpoonLetsEncryptFilePersistence();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			
			app.UseFluffySpoonLetsEncrypt();

			app.Run(async (context) =>
			{
				await context.Response.WriteAsync("Hello world");
			});
		}
	}
}
