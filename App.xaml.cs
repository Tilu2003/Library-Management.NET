using System.Windows;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.ViewModels;
using LibraryManagementSystem.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibraryManagementSystem;

/// <summary>
/// Application entry point.
/// Sets up Dependency Injection container (SOLID-D: Dependency Inversion).
/// </summary>
public partial class App : Application
{
    private IServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // ── Logging ───────────────────────────────────────────────────────────
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/library-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("Library Management System starting...");

        // ── Dependency Injection ───────────────────────────────────────────────
        var services = new ServiceCollection();

        // Database (replace connection string in appsettings or here)
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlServer(GetConnectionString()));

        // Repositories (Dependency Inversion: bind interfaces to implementations)
        services.AddScoped<IBookRepository,        BookRepository>();
        services.AddScoped<IMemberRepository,      MemberRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Services
        services.AddScoped<IBookService,        BookService>();
        services.AddScoped<IMemberService,      MemberService>();
        services.AddScoped<ITransactionService, TransactionService>();

        // ViewModels
        services.AddTransient<BookViewModel>();
        services.AddTransient<MemberViewModel>();
        services.AddTransient<TransactionViewModel>();
        services.AddTransient<MainViewModel>();

        // Windows
        services.AddTransient<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        // ── Run Migrations ────────────────────────────────────────────────────
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            db.Database.Migrate();
            Log.Information("Database migrated successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database migration failed. Ensure SQL Server is running and connection string is correct.");
            MessageBox.Show(
                $"Database connection failed:\n\n{ex.Message}\n\nPlease update the connection string in App.xaml.cs.",
                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        // ── Show Main Window ──────────────────────────────────────────────────
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Application exiting.");
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    // ── UPDATE THIS CONNECTION STRING ─────────────────────────────────────────
    private static string GetConnectionString() =>
        "Server=(localdb)\\mssqllocaldb;Database=LibraryManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true";
}
