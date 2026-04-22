using DRL.Core.Interface;
using DRL.Core.Service;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.Models;
using DRL.Model.UnitOfWork.Implementation;
using DRL.Model.UnitOfWork.Interface;

using Hangfire;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Swashbuckle.AspNetCore.Swagger;

using System;
using System.IO;
using System.Linq;

namespace DRL.API
{
    public class Startup
    {
        private static ILogManager loggerManager;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200")
                                 .AllowAnyMethod()
                                 .AllowAnyHeader()
                                 .AllowCredentials())
            );

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            //services.Configure<LdapConfig>(Configuration.GetSection("Logging").GetSection("ldap"));
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = System.Text.Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme); // ✅ Enable Windows Auth

            services.AddDbContextPool<DRLNewContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }, 10);

            services.AddMvc(option =>
            {
                option.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Core.IocConfig.NinjectConfiguration.Initialize();
            var fileInfo = new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"log4net.config");
            loggerManager = new Log4NetManager(fileInfo);
            services.AddSingleton(loggerManager);
            Core.Mapper.Configuration.Initialize();

            var logger = loggerManager.GetLogger(typeof(Startup));
            logger.Info("Log4NetManager Initialized");


            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "application/json",
                    "application/xml",
                    "text/plain"
                 });
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Fastest;
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "DRL API", Version = "v1" });

                //Set the comments path for the swagger json and ui.
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"\DRL.Api.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                //options.OperationFilter<RequiredOperation>(); //Register Required Operation Filter
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });
            services.AddSingleton(Configuration);
            return ConfigureServicesCollection(services);
        }

        private IServiceProvider ConfigureServicesCollection(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(20); // Adjust as necessary
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // ✅ Required for IMemoryCache
            services.AddMemoryCache(options =>
            {
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(20);
            });

            // ✅ Register the cache service wrapper as singleton
            services.AddSingleton<ICacheService, CacheService>();
            return DRL.Core.IocConfig.Configuration.Initialize(services);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("../swagger/v1/swagger.json", "DRL API V1"); });

            app.UseResponseCompression();

            app.UseCors("CorsPolicy");
            app.UseAuthentication(); // ✅ Enable Windows Authentication only in Production
            app.UseSession();
            app.UseMvc();
        }
    }
}
