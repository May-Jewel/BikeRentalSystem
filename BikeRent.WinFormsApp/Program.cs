namespace BikeRent.WinFormsApp;

using BikeRent.Database.AppDbContextModels;
using BikeRent.Domain.Services;
using Microsoft.EntityFrameworkCore;

public static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=LAPTOP-OI6UJSEI;Database=BikeRent;Trusted_Connection=True;TrustServerCertificate=True;");

        var dbContext = new AppDbContext(optionsBuilder.Options);
        var service = new BikeRentService(dbContext);

        Application.Run(new LoginForm(service));
    }
}
