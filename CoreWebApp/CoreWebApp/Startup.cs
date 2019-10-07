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
using CoreWebApp.Services;
using Polly;
using System.Net.Http;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Microsoft.Extensions.Logging;
using CoreWebApp.Repository;
using MediatR;
using System.Reflection;
using CoreWebApp.MediatR;
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

            DemoConfiguration(services);
            DemoSwagger(services);
            DemoHttpClientFactory(services);
            DemoPolly(services);

            var baseUrl = "https://jsonplaceholder.typicode.com";

            var restClient = RestEase.RestClient.For<ICommentRepository>(baseUrl);

            //not needed anymore - replaced by AddTypedClient - see in commit details
            //using (var scope = services.BuildServiceProvider().CreateScope())
            //{
            //    var cf = scope.ServiceProvider.GetService<IHttpClientFactory>();
            //    var httpClientPost = cf.CreateClient("jsonplaceholderClient");
            //    var restClientPost = RestEase.RestClient.For<IPostRepository>(httpClientPost);
            //    services.AddSingleton<IPostRepository>(restClientPost);
            //}



            services.AddSingleton<ICommentRepository>(p => restClient);
            services.AddTransient<ICommentService, CommentServiceWithRestEase>();

            services.AddSingleton(Log.Logger);

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(BehaviourPipelineFirst<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(BehaviourPipelineSecond<,>));

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

        private void DemoConfiguration(IServiceCollection services)
        {
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

        }

        private void DemoSwagger(IServiceCollection services)
        {
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
        }

        private void DemoHttpClientFactory(IServiceCollection services)
        {
            services.AddHttpClient();
            //  services.AddHttpClient<IAlbumService>(options => options.BaseAddress = new Uri("https://jsonplaceholder.typicode.com"));

            // moved to PollyDemo 
            //services.AddHttpClient<IAlbumService, AlbumServiceWithTypedClient>();
            services.AddHttpClient("jsonplaceholderClient", c =>
            {
                c.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            })
            .AddTypedClient(c => RestEase.RestClient.For<IPostRepository>(c));


            services.AddTransient<IUserService, UserServiceWithBasicClientUsage>();
            services.AddTransient<IPhotosService, PhotosServiceWithNamedClient>();
        }

        private void DemoPolly(IServiceCollection services)
        {
            //services.AddHttpClient<IAlbumService, AlbumServiceWithTypedClient>()
            //    .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // default 2
            //    .AddPolicyHandler(GetRetryPolicy());



            var shortTimeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(30);
            services.AddHttpClient<IAlbumService, AlbumServiceWithTypedClient>()
                .AddPolicyHandler((provider, request) =>
                HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(
                    new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(3)
                    },
                    onRetryAsync: async (outcome, timespan, retryAttempt, context) =>
                    {
                        var logger = provider.GetService<ILogger<HttpClient>>();
                        var logLevel = outcome.Exception != null ? LogLevel.Error : LogLevel.Warning;
                        logger.Log(
                            logLevel, outcome.Exception,
                            "Delaying for {delay}ms, then making retry {retry}.",
                            timespan.TotalMilliseconds,
                            retryAttempt);
                        await Task.CompletedTask;
                    }))
                    .AddPolicyHandler(shortTimeoutPolicy);
                    

        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {

            RetryPolicy retry = Policy
                .Handle<HttpRequestException>()  // 5xx or 408 (timeout)
                .Or<TimeoutRejectedException>() //describe
                .Retry(2);

            RetryPolicy retry2 = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetry(new []
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4)
                });

            //Other Polly.Contrib.WaitAndRetry!

            Random jitterer = new Random();
            //In prod use jitter from Polly.Contrib

            return HttpPolicyExtensions
                  .HandleTransientHttpError()
                  .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                  .WaitAndRetryAsync(6, retryAttemp =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttemp))
                        + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));
        }
    }
}
