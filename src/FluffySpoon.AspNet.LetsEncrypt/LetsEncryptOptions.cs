﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Certes;

namespace FluffySpoon.AspNet.LetsEncrypt
{
	public class LetsEncryptOptions
	{
		public IEnumerable<string> Domains { get; set; }

		/// <summary>
		/// Used only for LetsEncrypt to contact you when the domain is about to expire - not actually validated.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// The amount of time before the expiry date of the certificate that a new one is created. Defaults to 30 days.
		/// </summary>
		public TimeSpan TimeUntilExpiryBeforeRenewal { get; set; } = TimeSpan.FromDays(30);

		/// <summary>
		/// Recommended while testing - increases your rate limit towards LetsEncrypt. Defaults to false.
		/// </summary>
		public bool UseStaging { get; set; }

		/// <summary>
		/// Required. Sent to LetsEncrypt to let them know what details you want in your certificate. Some of the properties are optional.
		/// </summary>
		public CsrInfo CertificateSigningRequest { get; set; }
	}
}
