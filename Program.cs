using DbUp;
using Microsoft.Extensions.Configuration;
using QandA_App.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        //The lifetime of the DataRepository class is for that of an entire http request
        //Whenever IDataRepository is referenced in a constructor, replace it with an instance of DataRepository
        builder.Services.AddScoped<IDataRepository, DataRepository>();

        //Registering memory cache
        builder.Services.AddMemoryCache();
        //Setting the cache as a singleton
        builder.Services.AddSingleton<IQuestionCache, QuestionCache>();

        //Getting connection string to sql database from appsettings.json
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


        EnsureDatabase.For.SqlDatabase(connectionString);
        //create and configure an instance of the DbUp upgrader
        var upgrader = DeployChanges.To.SqlDatabase(connectionString, null).WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly()).WithTransaction().Build();
        //do a database migration if there are any pending SQL scripts
        if (upgrader.IsUpgradeRequired())
        {
            //do database migration
            upgrader.PerformUpgrade();
        }

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html"); ;

        app.Run();
    }
}