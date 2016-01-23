using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Entity;
using ImageGallery.Models;
using System.IO;
using System.Net;

namespace ImageGallery
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            //HttpListener listener =(HttpListener)builder.Properties["System.Net.HttpListener"];
            //listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;

            if (env.IsDevelopment())
            {
                /// This reads the configuration keys from the secret store.
                /// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }

            Configuration = builder.Build();

        }

        public static IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.            
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));
            //// Configure Auth
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(
            //        "ManageStore",
            //        authBuilder =>
            //        {
            //            authBuilder.RequireClaim("ManageStore", "Allowed");
            //        });
            //});

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            //// Set up NTLM authentication for WebListener like below.
            //// For IIS and IISExpress: Use inetmgr to setup NTLM authentication on the application vDir or
            //// modify the applicationHost.config to enable NTLM.
            //var listener = app.ServerFeatures.Get<WebListener>();
            //if (listener != null)
            //{
            //    listener.AuthenticationManager.AuthenticationSchemes = System.Net.AuthenticationSchemes.Ntlm;
            //}

            //string mapPath =  Path.Combine(env.WebRootPath,"Temp");
            //if (Directory.Exists(mapPath))
            //{
            //    try
            //    {
            //        DirectoryInfo dir = new DirectoryInfo(mapPath);
            //        foreach (FileInfo fi in dir.GetFiles())
            //        {
            //            fi.IsReadOnly = false;
            //            fi.Delete();
            //        }
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}
            //else
            //{
            //    try
            //    {
            //        Directory.CreateDirectory(mapPath);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}


        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
