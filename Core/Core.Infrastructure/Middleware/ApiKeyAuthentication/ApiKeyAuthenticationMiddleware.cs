using Core.Common.Response;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Middleware.ApiKeyAuthentication
{
    public class ApiKeyAuthenticationMiddleware
    {
        private const string ApiKeyToCheck = "k1234567891011121314151617181920";
        private readonly RequestDelegate next;

        public ApiKeyAuthenticationMiddleware(RequestDelegate next)
        {

            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            bool validKey = false;
            var apiKeyExists = context.Request.Headers.ContainsKey("X-API-KEY");
            if(apiKeyExists)
            {
                if(context.Request.Headers["X-API-KEY"].Equals(ApiKeyToCheck))
                {
                    validKey = true;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Please add an ApiKey to you request header");
            }

            if(!validKey)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Invalid ApiKey");
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}
