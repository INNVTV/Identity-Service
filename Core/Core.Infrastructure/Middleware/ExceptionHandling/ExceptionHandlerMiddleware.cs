﻿using Core.Common.Response;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Middleware.ExceptionHandling
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Hande exceptions conditionally by type:

            if (exception.GetType() == typeof(ArgumentException))
            {
                //Track the user that ran into the exception (for our structured logs)
                //var user = new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Smith" };

                // Log our exception using Serilog.
                // Use structured logging to capture the full exception object.
                // Serilog provides the @ destructuring operator to help preserve object structure for our logs.
                Log.Error("Argument exception caught {@exception}", exception);

                var code = HttpStatusCode.OK; //<-- we do not respond with "InternalServerError" - but rather with isSuccess = false with exception message (this also avoids issues with OpenAPI/Swagger throwing an exception due to server response codes).
                var result = JsonConvert.SerializeObject(new { isSuccess = false, exceptionType = exception.GetType().ToString(), message = exception.Message });

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)code;
                return context.Response.WriteAsync(result);

            }
            else
            {
                //Track the user that ran into the exception (for our structured logs)
                //var user = new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Smith" };

                // Log our exception using Serilog.
                // Use structured logging to capture the full exception object.
                // Serilog provides the @ destructuring operator to help preserve object structure for our logs.
                Log.Error("Exception caught {@exception}", exception);

                //var code = HttpStatusCode.InternalServerError;
                var code = HttpStatusCode.OK; //<-- we do not respond with "InternalServerError" - but rather with isSuccess = false with exception message (this also avoids issues with OpenAPI/Swagger throwing an exception due to server response codes).
                //var result = JsonConvert.SerializeObject(exception, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore } );
                var result = JsonConvert.SerializeObject( new { isSuccess = false, exceptionType = exception.GetType().ToString(), message = exception.Message });

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)code;
                return context.Response.WriteAsync(result);
            }

        }
    }
}
