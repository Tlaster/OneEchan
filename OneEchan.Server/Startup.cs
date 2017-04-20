using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using OneEchan.Server.Data;
using OneEchan.Server.Models;
using cloudscribe.Core.Storage.EFCore.Common;
using OneEchan.Server.Controllers;
using Microsoft.Extensions.Localization;
using OneEchan.Server.Models.Aliyun;

namespace OneEchan.Server
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("cloudoptions.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Environment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // string pathToCryptoKeys = Path.Combine(environment.ContentRootPath, "dp_keys");
            services.AddDataProtection()
                // .PersistKeysToFileSystem(new System.IO.DirectoryInfo(pathToCryptoKeys))
                ;


            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
            });
            services.AddMemoryCache();
            // we currently only use session for alerts, so we can fire an alert on the next request
            // if session is disabled this feature fails quietly with no errors
            services.AddSession();

            ConfigureAuthPolicy(services);

            services.AddOptions();


            var cloudscribe = Configuration.GetConnectionString("CloudscribeConnection");
            services.AddCloudscribeCoreEFStorageMSSQL(cloudscribe);
            services.AddCloudscribeLoggingEFStorageMSSQL(cloudscribe);

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("VideoConnection")));
            services.AddDbContext<TranscodeDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TranscodeConnection")));
            //services.AddCloudscribeCoreNoDbStorage();
            //services.AddCloudscribeLoggingNoDbStorage(Configuration);


            services.AddCloudscribeLogging();
            services.AddCloudscribeCore(Configuration);
            services.AddCloudscribePagination();


            services.Configure<GlobalResourceOptions>(Configuration.GetSection("GlobalResourceOptions"));
            services.AddSingleton<IStringLocalizerFactory, GlobalResourceManagerStringLocalizerFactory>();

            // you must configure a folder where the resx files will live, it can be named as you like
            services.AddLocalization(options => options.ResourcesPath = "GlobalResources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-us"),
                    new CultureInfo("zh-cn"),
                    //new CultureInfo("ja-jp")
                };

                // State what the default culture for your application is. This will be used if no specific culture
                // can be determined for a given request.
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
                options.SupportedUICultures = supportedCultures;

                // You can change which providers are configured to determine the culture for requests, or even add a custom
                // provider with your own logic. The providers will be asked in order to provide a culture for each request,
                // and the first to provide a non-null result that is in the configured supported cultures list will be used.
                // By default, the following built-in providers are configured:
                // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
                // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
                // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header
                //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
                //{
                //  // My custom request culture logic
                //  return new ProviderCultureResult("en");
                //}));
            });


            var useSsl = Configuration.GetValue<bool>("AppSettings:UseSsl");
            services.Configure<MvcOptions>(options =>
            {
                if (useSsl)
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                }

            });

            services.Configure<AliyunOptions>(Configuration.GetSection("Aliyun"));

            services.AddMvc()
                .AddRazorOptions(options =>
                {
                    options.AddCloudscribeViewLocationFormats();

                    options.AddEmbeddedViewsForNavigation();
                    options.AddEmbeddedBootstrap3ViewsForCloudscribeCore();
                    options.AddEmbeddedViewsForCloudscribeLogging();
                    ;

                    options.ViewLocationExpanders.Add(new cloudscribe.Core.Web.Components.SiteViewLocationExpander());
                })
                    ;
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // you can add things to this method signature and they will be injected as long as they were registered during 
        // ConfigureServices
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IOptions<cloudscribe.Core.Models.MultiTenantOptions> multiTenantOptionsAccessor,
            IServiceProvider serviceProvider,
            cloudscribe.Logging.Web.ILogRepository logRepo
            )
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            ConfigureLogging(loggerFactory, serviceProvider, logRepo);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseForwardedHeaders();
            app.UseStaticFiles();

            // custom 404 and error page - this preserves the status code (ie 404)
            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

            app.UseSession();
            
            app.UseMultitenancy<cloudscribe.Core.Models.SiteContext>();

            var multiTenantOptions = multiTenantOptionsAccessor.Value;

            app.UsePerTenant<cloudscribe.Core.Models.SiteContext>((ctx, builder) =>
            {
                builder.UseCloudscribeCoreDefaultAuthentication(
                    loggerFactory,
                    multiTenantOptions,
                    ctx.Tenant);
            });

            UseMvc(app, multiTenantOptions.Mode == cloudscribe.Core.Models.MultiTenantMode.FolderName);

            // this creates ensures the database is created and initial data
            CoreEFStartup.InitializeDatabaseAsync(app.ApplicationServices).Wait();

            // this one is only needed if using cloudscribe Logging with EF as the logging storage
            LoggingEFStartup.InitializeDatabaseAsync(app.ApplicationServices).Wait();

            InitData(app.ApplicationServices).Wait();

        }

        private async Task InitData(IServiceProvider applicationServices)
        {
            using (var serviceScope = applicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var transcodeDb = serviceScope.ServiceProvider.GetService<TranscodeDbContext>();
                await transcodeDb.Database.EnsureCreatedAsync();
                var applicationDb = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var coreDb = serviceScope.ServiceProvider.GetService<ICoreDbContext>();
                if (await applicationDb.Database.EnsureCreatedAsync())
                {
                    var random = new Random();
                    var number = random.Next().ToString();
                    if (!await applicationDb.Category.AnyAsync())
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            await applicationDb.Category.AddAsync(new Category
                            {
                                SiteId = (await coreDb.Sites.FirstOrDefaultAsync()).Id
                            });
                        }
                        await applicationDb.SaveChangesAsync();
                    }
                    if (!await applicationDb.CategoryName.AnyAsync())
                    {
                        foreach (var item in applicationDb.Category)
                        {
                            await applicationDb.CategoryName.AddAsync(new CategoryName
                            {
                                Category = item,
                                Language = Languages.AllLanguage[0].Id,
                                Name = $"Name {number} {Languages.AllLanguage[0].Id}",
                            });
                        }
                        await applicationDb.SaveChangesAsync();
                    }
                    if (!await applicationDb.Video.AnyAsync())
                    {
                        foreach (var item in applicationDb.Category)
                        {
                            for (var i = 0; i < 10; i++)
                            {
                                await applicationDb.Video.AddAsync(new Video
                                {
                                    Category = item,
                                    Description = $"Desc {number}",
                                    Language = Languages.AllLanguage[0].Id,
                                    Title = $"Title {number}",
                                    PostState = Post.State.Published,
                                    FileName = $"{number}.mp4",
                                    UploaderId = (await coreDb.Users.FirstOrDefaultAsync()).Id,
                                    Ip = "111.111.111.111"
                                });
                            }
                        }
                        await applicationDb.SaveChangesAsync();
                    }
                    if (!await applicationDb.VideoUrl.AnyAsync())
                    {
                        foreach (var item in applicationDb.Video)
                        {
                            await applicationDb.VideoUrl.AddAsync(new VideoUrl
                            {
                                Duration = TimeSpan.Zero,
                                QualityInfo = "h265 1080P",
                                Url = "http://www.w3school.com.cn/i/movie.ogg",
                                Video = item,
                                Thumb = "http://www.w3school.com.cn/i/movie.ogg",
                                Type = "video/ogg"
                            });
                            await applicationDb.VideoUrl.AddAsync(new VideoUrl
                            {
                                Duration = TimeSpan.Zero,
                                QualityInfo = "h265 720P",
                                Url = "http://www.w3school.com.cn/i/movie.ogg",
                                Video = item,
                                Thumb = "http://www.w3school.com.cn/i/movie.ogg",
                                Type = "video/ogg"
                            });
                        }
                        await applicationDb.SaveChangesAsync();
                    }
                    if (!await applicationDb.Comment.AnyAsync())
                    {
                        foreach (var item in applicationDb.Video)
                        {
                            await applicationDb.Comment.AddAsync(new Comment
                            {
                                Content = $"Content {number}",
                                Language = Languages.AllLanguage[0].Id,
                                Post = item,
                                UploaderId = (await coreDb.Users.FirstOrDefaultAsync()).Id,
                                Ip = "111.111.111.111"
                            });
                        }
                        await applicationDb.SaveChangesAsync();
                    }
                    if (!await applicationDb.Article.AnyAsync())
                    {
                        foreach (var item in applicationDb.Category)
                        {
                            for (var i = 0; i < 10; i++)
                            {
                                var article = new Article
                                {
                                    Category = item,
                                    Content = $"Content {number}",
                                    Language = Languages.AllLanguage[0].Id,
                                    Title = $"Title {number}",
                                    PostState = Post.State.Published,
                                    UploaderId = (await coreDb.Users.FirstOrDefaultAsync()).Id,
                                    Ip = "111.111.111.111"
                                };
                                await applicationDb.Article.AddAsync(article);
                                await applicationDb.Comment.AddAsync(new Comment
                                {
                                    Content = $"Content {number}",
                                    Language = Languages.AllLanguage[0].Id,
                                    Post = article,
                                    UploaderId = (await coreDb.Users.FirstOrDefaultAsync()).Id,
                                    Ip = "111.111.111.111"
                                });
                            }
                        }
                        await applicationDb.SaveChangesAsync();
                    }
                }
            }
        }

        private void ConfigureLogging(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider
            , cloudscribe.Logging.Web.ILogRepository logRepo
            )
        {
            // a customizable filter for logging
            LogLevel minimumLevel;
            if (Environment.IsProduction())
            {
                minimumLevel = LogLevel.Warning;
            }
            else
            {
                minimumLevel = LogLevel.Information;
            }


            // add exclusions to remove noise in the logs
            var excludedLoggers = new List<string>
            {
                "Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware",
                "Microsoft.AspNetCore.Hosting.Internal.WebHost",
            };

            Func<string, LogLevel, bool> logFilter = (string loggerName, LogLevel logLevel) =>
            {
                if (logLevel < minimumLevel)
                {
                    return false;
                }

                if (excludedLoggers.Contains(loggerName))
                {
                    return false;
                }

                return true;
            };

            loggerFactory.AddDbLogger(serviceProvider, logFilter, logRepo);
        }

        private void UseMvc(IApplicationBuilder app, bool useFolders)
        {
            app.UseMvc(routes =>
            {
                if (useFolders)
                {
                    routes.MapRoute(
                        name: "folderdefault",
                        template: "{sitefolder}/{controller}/{action}/{id?}",
                        defaults: new { controller = "Home", action = "Index" },
                        constraints: new { name = new cloudscribe.Core.Web.Components.SiteFolderRouteConstraint() }
                        );

                }

                routes.MapRoute(
                    name: "errorhandler",
                    template: "{controller}/{action}/{statusCode}");

                routes.MapRoute(
                    name: "video",
                    template: "v/{id}",
                    defaults: new { controller = nameof(VideoController).Replace("Controller", "") , action = nameof(VideoController.Details) });

                routes.MapRoute(
                    name: "article",
                    template: "p/{id}",
                    defaults: new { controller = nameof(ArticleController).Replace("Controller", ""), action = nameof(ArticleController.Details) });

                //routes.MapRoute(
                //    name: "accountmanagemant",
                //    template: "my/{action}",
                //    defaults: new { controller = nameof(AccountController).Replace("Controller", ""), action = nameof(AccountController.Index) });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");

                routes.MapRoute(
                    name: "api",
                    template: "api/{controller}/{action}");

            });
        }


        private void ConfigureAuthPolicy(IServiceCollection services)
        {
            //https://docs.asp.net/en/latest/security/authorization/policies.html

            services.AddAuthorization(options =>
            {
                options.AddCloudscribeCoreDefaultPolicies();
                options.AddCloudscribeLoggingDefaultPolicy();

                // add other policies here 

            });

        }
    }
}
