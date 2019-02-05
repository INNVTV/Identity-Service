﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using MediatR;
using Core.Infrastructure.Persistence.DocumentDatabase;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence.StorageAccount;
using Core.Infrastructure.Persistence.RedisCache;
using Core.Infrastructure.Services.Email;
using Core.Infrastructure.Pipeline;
using MediatR.Pipeline;
using System.Reflection;
using Core.Application;
using Serilog;
using IdentityService.ServiceModels;
using AutoMapper;
using Swashbuckle.AspNetCore.Swagger;
using Core.Infrastructure.Middleware.ExceptionHandling;
using Core.Common.Response;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Commands.UpdateUserName;

namespace IdentityService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Setup Serilog for Logging

            // Core is set up to use the global, statically accessible logger from Serilog.
            // It must be set up in the main entrpoint and does not require a DI container

            // Create a logger with configured sinks, enrichers, and minimum level
            // Serilog's global, statically accessible logger, is set via Log.Logger and can be invoked using the static methods on the Log class.

            // File Sink is commented out and can be replaced with Serilogs vast library of available sinks

            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code) //<-- This will give us output to our Kestrel console
            //.WriteTo.File("_logs/log-.txt", rollingInterval: RollingInterval.Day) //<-- Write our logs to a local text file with rolling interval configuration
            .CreateLogger();

            Log.Information("The global/static Serilog logger has been configured.");

            #endregion

            /* -----------------------------------------------------------------
             * Default .Net Core IServiceCollection is used in this example.
             * You can switch to Autofaq, Ninject or any DI Container of your choice.
             * ------------------------------------------------------------------
             * Autofaq allows for automatic registration of Interfaces by using "Assembly Scanning":
             *     - builder.RegisterAssemblyTypes(dataAccess)
             *         .Where(t => t.Name.EndsWith("Repository"))
             *         .AsImplementedInterfaces();
             ---------------------------------------------------------------------*/


            #region Create our custom dependancies

            #region Initialize our ICoreConfiguration object

            ICoreConfiguration coreConfiguration = new CoreConfiguration(Configuration);

            #endregion

            #region Initialize our Persistence Layer objects

            IDocumentContext documentContext = new DocumentContext(Configuration);
            IStorageContext storageContext = new StorageContext(Configuration);
            IRedisContext redisContext = new RedisContext(Configuration);

            #endregion

            #region Initialize 3rd Party Service Dependencies

            IEmailService sendgridService = new SendGridEmailService(Configuration);

            #endregion

            #endregion

            #region Register our dependencies

            #region Inject our custom dependancies into the default WebAPI provider

            // Configuration
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<ICoreConfiguration>(coreConfiguration);

            // Persistence
            services.AddSingleton<IDocumentContext>(documentContext);
            services.AddSingleton<IStorageContext>(storageContext);
            services.AddSingleton<IRedisContext>(redisContext);

            // 3rd Party Services
            services.AddSingleton<IEmailService>(sendgridService);

            // TODO: Account/Platform Activity Logging
            // serviceCollection.AddSingleton<ICore(Account/Platform)ActivityLogger>(coreLogger);

            // MediatR Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));

            #endregion

            #region Register MediatR Commands, Notifications

            // We only need to register ONE command from the library and MediatR will resolve the rest
            services.AddMediatR(typeof(CreateUserCommand).GetTypeInfo().Assembly); //<-- Will find the rest of our commands within the Core.Application Assembly

            #endregion

            #endregion 




            #region Configure AutoMapper (Instance Version) for ServiceModels

            /*----------------------------------------
             * AutoMapper is also configured using the Static API within our Core library.
             * We also use the instance implementation seperatly here within the Services project.
             * ---------------------------------------
             * The Core classes will be a compiled DLL or could be a nuget package in the future.
             * The Core should have no knowledge of AutoMapper configurations in the layer above it.
             * --------------------------------------*/

            var config = new MapperConfiguration(cfg => {

                //Service Models
                cfg.CreateMap<CreateUserServiceModel, CreateUserCommand>();
                

                //GrpcProtobuffer Messages:
                /*
                cfg.CreateMap<Shared.GrpcClientLibrary.CreateAccountRequest, CreateAccountCommand>();
                cfg.CreateMap<CreateAccountCommandResponse, Shared.GrpcClientLibrary.CreateAccountResponse>();

                cfg.CreateMap<Shared.GrpcClientLibrary.CloseAccountRequest, CloseAccountCommand>();
                cfg.CreateMap<BaseResponse, Shared.GrpcClientLibrary.CloseAccountResponse>();

                cfg.CreateMap<Shared.GrpcClientLibrary.GetAccountListRequest, GetAccountListQuery>();
                cfg.CreateMap<AccountListViewItem, Shared.GrpcClientLibrary.GetAccountListResponse.Types.Account>();*/
            });

            var mapper = config.CreateMapper();
            // or...
            //IMapper mapper = new Mapper(config);
            //var dest = mapper.Map<Source, Dest>(new Source());

            //Add to our Service Provider:
            services.AddSingleton<IMapper>(mapper);

            #endregion

            #region Register Swagger Generator with Swashbuckle 

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Identity Service", Version = "v1", Description = "REST API for Identity Service" });
            });

            #endregion

            // Register default WebAPI dependancies
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                    options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            //Start the gRPC Server:
            GrpcServer.ServerInitializer.Initialize(Int32.Parse(Configuration.GetSection("gRPC").GetSection("Port").Value), services.BuildServiceProvider(), mapper);

            // Initialize Core.Startup
            Core.Startup.Routines.Initialize(services.BuildServiceProvider().GetService<IMediator>());

            Log.Information("Startup tasks complete.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region Exceptions Middleware

            // Exceptions will be caught, logged and json results will be returned to the caller based on exception type.

            app.UseExceptionHandlerMiddleware();

            #endregion

            #region Swagger Middleware

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core Services V1");
            });

            // Generated document describing the endpoints: http://localhost:<port>/swagger/v1/swagger.json
            // The Swagger UI can be found at: http://localhost:<port>/swagger

            #endregion

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
