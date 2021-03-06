
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProgLibrary.Core.DAL;
using ProgLibrary.Core.Domain;

[assembly: HostingStartup(typeof(ProgLibrary.UI.Areas.Identity.IdentityHostingStartup))]
namespace ProgLibrary.UI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AuthenticationDbContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("LibraryDBContext")));

              
            });
        }
    }
}