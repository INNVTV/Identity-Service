using Core.Common.Response;
using Core.Domain.Entities;
using Core.Infrastructure.Configuration;
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
        private string _primaryApiKey = string.Empty;
        private string _secondaryApiKey = string.Empty;
        private bool _forceSecureApiCalls = false;

        private readonly RequestDelegate next;

        public ApiKeyAuthenticationMiddleware(RequestDelegate next, ICoreConfiguration coreConfiguraton)
        {
            this.next = next;

            _primaryApiKey = coreConfiguraton.Security.PrimaryApiKey;
            _secondaryApiKey = coreConfiguraton.Security.SecondaryApiKey;
            _forceSecureApiCalls = coreConfiguraton.Security.ForceSecureApiCalls;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            bool validKey = false;

            if (context.Request.Path.HasValue)
            {
                // We only require api keys and secure calls on our API endpoints
                if(context.Request.Path.Value.StartsWith("/api"))
                {
                    
                    if(_forceSecureApiCalls && !context.Request.IsHttps)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync("Please only connect via HTTPS");
                    }

                    // We allow authentication, refresh token and public requests to pass through
                    if(    context.Request.Path.Value.StartsWith("/api/authenticate")
                        || context.Request.Path.Value.StartsWith("/api/authentication")
                        || context.Request.Path.Value.StartsWith("/api/public"))
                    {
                        validKey = true;
                    }
                    else
                    {
                        var apiKeyExists = context.Request.Headers.ContainsKey("X-API-KEY");
                        if (apiKeyExists)
                        {
                            if (context.Request.Headers["X-API-KEY"].Equals(_primaryApiKey) || context.Request.Headers["X-API-KEY"].Equals(_secondaryApiKey))
                            {
                                validKey = true;
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response.WriteAsync("Please add an ApiKey to you request header");
                        }
                    }

                }
                else
                {
                    validKey = true;
                }
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
