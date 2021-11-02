using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProgLibrary.Core.DAL;
using ProgLibrary.Core.Domain;
using ProgLibrary.Infrastructure.Mappers;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.Services.JwtToken;
using ProgLibrary.Infrastructure.Settings.JwtToken;
using System;

namespace ProgLibrary.UI
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
            services.AddDbContext<AuthenticationDbContext>();
            services.AddIdentity<User,Role>()
                .AddEntityFrameworkStores<AuthenticationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders()      
                .AddDefaultUI();

                     //.AddRoles<Role>()




            services.Configure<JwtSettings>(Configuration.GetSection("JWT")); // Bindowanie z sekcji konfiguracji JwtConfig - appsetings.json"         
            services.AddSingleton<IJwtHandler, JwtHandler>(); //JwtBearer Tokens Handler
            services.AddScoped<IBrokerService, BrokerService>();
            services.AddSingleton(AutoMapperConfig.Initialize()); // zwraca IMapper z AutoMapperConfig

            services.AddHttpClient("api", c =>
            {
                c.BaseAddress = new Uri(Configuration["API:Addres"]);
            });
            services.AddAuthentication().AddCookie(options =>
            {
                //options.LoginPath = "/Account/Unauthorized/";
                //options.AccessDeniedPath = "/Account/Forbidden/";
                
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });



            services.AddControllersWithViews();

            services.AddAuthorization(policies =>
            {
                policies.AddPolicy("HasAdminRole", role => role.RequireRole("admin"));
                policies.AddPolicy("HasUserRole", role => role.RequireRole("user"));
                policies.AddPolicy("HasSuperAdminRole", role => role.RequireRole("superadmin"));
                
                
             
            });

            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();

            });



            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                
            }).AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);       
            services.AddSession();
            services.AddHttpContextAccessor();
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

              
            });
        }
    }
}
