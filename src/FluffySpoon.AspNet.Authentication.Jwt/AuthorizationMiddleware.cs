﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Authentication.Jwt
{
  class AuthorizationMiddleware
  {
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(
      HttpContext context,
      IJwtTokenGenerator generator,
      IIdentityResolver identityResolver)
    {
      qwdkqwd //TODO: have a pre auth filter that can set the user before normal JWT middleware kicks in for the auth part.

      string token;
      if (context?.User?.Claims?.Any() == true)
      {
        token = generator.GenerateToken(context
          .User
          .Claims
          .ToArray());
      }
      else
      {
        var headers = (FrameRequestHeaders)context.Request.Headers;
        var authorization = headers.HeaderAuthorization.SingleOrDefault();

        const string authorizationPrefix = "FluffySpoon ";
        if (authorization != null && authorization.StartsWith(authorizationPrefix))
        {
          var credentialsCode = authorization.Substring(authorizationPrefix.Length);
          var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(credentialsCode));
          var credentials = JsonConvert.DeserializeObject<Credentials>(decoded);

          var claimsResult = await identityResolver.GetClaimsAsync(credentials);

          var claims = claimsResult
            .Claims
            .Select(x => new Claim(
              x.Key, 
              x.Value))
            .ToList();
          if (claims.Any(x => x.ValueType == ClaimTypes.Role))
          {
            throw new InvalidOperationException("Can't provide roles through claims. Use the roles array instead of the claims result.");
          }

          foreach (var role in claimsResult.Roles)
          {
            claims.Add(new Claim(
              ClaimTypes.Role,
              role));
          }

          claims.Add(new Claim(
            JwtRegisteredClaimNames.Sub, 
            credentials.Username));
          claims.Add(new Claim(
            "fluffy-spoon.authentication.jwt.username",
            credentials.Username));

          token = generator.GenerateToken(claims.ToArray());
        }
        else
        {
          token = generator.GenerateToken(
            new Claim("fluffy-spoon.authentication.jwt.isAnonymous", "true"));
        }
      }

      context
        .Response
        .Headers
        .Add("Token", token);

      await _next(context);
    }

  }
}
