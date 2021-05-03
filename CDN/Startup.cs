using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace CDN
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddGrpc();

            services.AddOptions();
            services.AddDistributedMemoryCache();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory
      )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDirectoryBrowser();
            app.UseHttpsRedirection();


            //app.UseStaticFiles(
            //    CreateNewStaticfilesPath(env)
            //    ); ;
            //loggerFactory.add
            // app.UseMiddleware<BLL.MiddleWare.RequestRedirectionByLocation>();
            app.UseRewriter(new RewriteOptions().Add(new RedirectWwwRule()));
            app.UseStaticFiles(new StaticFileOptions()
            {
                HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(30)
                    };
                }
            });

            app.UseRouting();

            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
     

            });
        }

        private StaticFileOptions CreateNewStaticfilesPath(IWebHostEnvironment env)
        {
            return new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, "MyStaticFiles")),
                RequestPath = "/StaticFiles"
            };


        }

    }
}
