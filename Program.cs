using Microsoft.EntityFrameworkCore;
using NatureNexus;
using NatureNexus.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace NatureNexus
{
    public class Program
    {
        public IConfiguration Configuration { get; }

        public Program(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host);

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<NatureNexusContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
         .ConfigureWebHostDefaults(webBuilder =>
         {
             webBuilder.ConfigureServices((hostingContext, services) =>
             {
                 var configuration = hostingContext.Configuration;
                 services.AddRazorPages().AddRazorRuntimeCompilation();
                 services.AddControllersWithViews();
                 services.AddDbContext<NatureNexusContext>(options => options.UseSqlServer(configuration.GetConnectionString("NatureNexusContext")));
                 services.AddRazorPages()
                     .AddMvcOptions(options =>
                     {
                         options.MaxModelValidationErrors = 50;
                         options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                             _ => "The field is required.");
                     });
             })
             .Configure(app =>
             {
                 var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                 if (env.IsDevelopment())
                 {
                     app.UseDeveloperExceptionPage();
                 }
                 else
                 {
                     app.UseExceptionHandler("/Home/Error");
                 }
                 app.UseStaticFiles();

                 app.UseRouting();

                 app.UseAuthorization();

                 app.UseEndpoints(endpoints =>
                 {
                     endpoints.MapControllerRoute(
                         name: "default",
                         pattern: "{controller=Home}/{action=Index}/{id?}");
                 });
             });
         });

    }
}
