using System;
using CoreWebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CoreWebApp.Areas.Identity.IdentityHostingStartup))]
namespace CoreWebApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<CoreWebAppContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("CoreWebAppContextConnection")));

                services.AddIdentity<IdentityUser, IdentityRole>()
             //   .AddDefaultUI()
                .AddEntityFrameworkStores<CoreWebAppContext>();
            });
        }
    }
}