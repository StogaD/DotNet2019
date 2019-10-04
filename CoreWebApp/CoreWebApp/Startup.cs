using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreWebApp.Api.Infrastructure;
using CoreWebApp.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Serilog;
//using Microsoft.OpenApi.Models;  -> is used in preview version


namespace CoreWebApp
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddOptions<Parameters>().Configure(o => o.Version = 2);
            services.Configure<Parameters>(Configuration.GetSection("Parameters"));
            services.PostConfigure<Parameters>(x => x.Speed = x.Speed * 2);

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var paramtersRetrivedFromIOptions = scope.ServiceProvider.GetService<IOptions<Parameters>>();
            }

            var url = Configuration.GetValue<string>("DescriptionUrl");
            var missingUrl = Configuration.GetValue<string>("url", "http://localhost:9200");

            var speed = Configuration.GetValue<int>("Parameters:Speed");

            var section = Configuration.GetSection("Parameters");
            if (section.Exists())
            {
                var getValue = section.GetValue(typeof(int), "Speed");
                var bindValue = new Parameters();
                section.Bind(bindValue);
            };

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Contact = new Contact { Email = "xxx@o2.pl", Name = "DawidS", Url = "http://google.pl" },
                        Description = "Example API",
                        License = new License { Name = "Licence name" },
                        Title = "My API",
                        Version = "Version 1"
                    });
                c.OperationFilter<AuthorizationHeaderOperationFilter>("myname");
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });



            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default30",
                    new CacheProfile
                    {
                        Duration = 30
                    });
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
           {
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI");
               c.RoutePrefix = string.Empty;
           });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
