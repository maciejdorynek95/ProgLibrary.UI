using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProgLibrary.Core.DAL;
using ProgLibrary.Core.Domain;
using ProgLibrary.Core.Repositories;
using ProgLibrary.Infrastructure.Mappers;
using ProgLibrary.Infrastructure.Middlewares;
using ProgLibrary.Infrastructure.Repositories;
using ProgLibrary.Infrastructure.Services;
using ProgLibrary.Infrastructure.Services.JwtToken;
using ProgLibrary.Infrastructure.Settings.JwtToken;
using System;
using System.Text;

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
            //services.AddMvc();
       

            services.AddSession(options =>
            {
                options.Cookie.Name = "ProgLibraryUI.Session";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.IOTimeout = TimeSpan.FromSeconds(60);
                //options.Cookie.IsEssential = true;

            });

           

            services.AddDbContext<AuthenticationDbContext>();
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(Configuration.GetSection("JWT")); // Bindowanie z sekcji konfiguracji JwtConfig - appsetings.json"   
           
            services.AddSingleton<IJwtHandler, JwtHandler>(); //JwtBearer Tokens Handler
            services.AddSingleton<IBrokerService, BrokerService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddSingleton(AutoMapperConfig.Initialize()); // zwraca IMapper z AutoMapperConfig


            services.AddDbContext<LibraryDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("LibraryDBContext"), options => options.MigrationsAssembly("ProgLibrary.Core")));

            services.AddHttpClient("api", c =>
            {
                c.BaseAddress = new Uri(Configuration["API:Addres"]);
                //c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer");
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.Audience = Configuration["JWT:Audience"];
                options.Authority = Configuration["JWT:Authority"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:SecretKey"]))
                };
            });
          
            //services.AddSingleton<IJwtHandler, JwtHandler>(); //JwtBearer Tokens Handler
            services.AddAuthorization(policies =>
            {
                policies.AddPolicy("HasAdminRole", role => role.RequireRole("admin"));
                policies.AddPolicy("HasUserRole", role => role.RequireRole("user"));
                policies.AddPolicy("HasSuperAdminRole", role => role.RequireRole("superadmin"));
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                   JwtBearerDefaults.AuthenticationScheme);
                policies.DefaultPolicy = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser().Build();
            });

            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
                //etc
            });

            services.AddControllersWithViews();
            //var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            //    //.RequireAuthenticatedUser()
            //    .Build();
            //options.Filters.Add(new AuthorizeFilter(policy));
          
            //services.AddRazorPages(options =>
            //{
            //    //options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            //    //options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            //});

            services.AddRazorPages()
                 //.AddMicrosoftIdentityUI()
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true); // poprawa formatowania json
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
            //app.UseMiddleware<PreRequestModifications>();
            
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
