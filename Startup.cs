using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Trans9.DataAccess;
using Trans9.Utilities;

namespace Trans9
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
            services.AddScoped<IViewRenderService, ViewRenderService>();

            services.AddDbContext<DataDbContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DevConnecttion"))
                .EnableDetailedErrors();
            });

            services.AddDbContext<AppUserDbContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DevConnecttion"))
                .EnableDetailedErrors();
            });

            services.AddDbContext<StoredProcedureDbContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DevConnecttion"))
                .EnableDetailedErrors();
            });

            services.AddControllersWithViews();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    option.LoginPath = "/Home/login";
                    option.AccessDeniedPath = "/Home/login";
                });
            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(30);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions()
                {
                    SourceCodeLineCount = 1
                });
            }
            else
            {
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                app.UseExceptionHandler("/Error");
            }
            //
            NLog.GlobalDiagnosticsContext.Set("logDirectory", env.ContentRootPath);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });
        }
    }
}
