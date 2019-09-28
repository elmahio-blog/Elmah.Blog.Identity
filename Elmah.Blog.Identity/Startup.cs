namespace Elmah.Blog.Identity.UI
{
    using Elmah.Blog.Identity.UI.Data;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// The configure services routine.
        /// </summary>
        /// <param name="services">
        /// A collection of services used in the application.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    this.Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication().AddFacebook(o =>
                {
                    o.AppId = this.Configuration["Authentication:CoderPro:Facebook:AppId"];
                    o.AppSecret = this.Configuration["Authentication:CoderPro:Facebook:AppSecret"];
                });

            // Add Email Service
            services.AddTransient<IEmailSender, Services.EmailSender>(
                implementation => new Services.EmailSender(
                    this.Configuration["EmailSender:Host"],
                    this.Configuration.GetValue<int>("EmailSender:Port"),
                    this.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    this.Configuration["EmailSender:UserName"],
                    this.Configuration["EmailSender:Password"],
                    this.Configuration["EmailSender:SenderEmail"]));

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        /// <summary>
        /// This routine configures the application.
        /// </summary>
        /// <param name="app">
        /// The app builder.
        /// </param>
        /// <param name="env">
        /// The hosting environment.
        /// </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

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
