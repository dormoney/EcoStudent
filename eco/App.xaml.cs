using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eco.Data;
using eco.Services;
using System.IO;

namespace eco
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public IConfiguration Configuration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Обработка неперехваченных исключений
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += App_UnhandledException;

            // Отключаем автоматическое завершение приложения при закрытии всех окон
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            base.OnStartup(e);

            // Показываем окно авторизации вместо главного окна
            ShowLoginWindow();
        }

        private async void ShowLoginWindow()
        {
            try
            {
                var authService = ServiceProvider.GetRequiredService<AuthService>();
                var loginWindow = new Views.LoginWindow(authService);
                
                var loginResult = loginWindow.ShowDialog();
                
                if (loginResult == true)
                {
                    try
                    {
                        // Если авторизация успешна, создаем EcoDbContext напрямую
                        var optionsBuilder = new DbContextOptionsBuilder<EcoDbContext>();
                        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                        var context = new EcoDbContext(optionsBuilder.Options);
                        
                        // Инициализируем пункты сдачи
                        var recyclingPointService = new RecyclingPointService(context);
                        await recyclingPointService.EnsureRecyclingPointsExistAsync();
                        
                        // Инициализируем акции
                        var promotionService = new PromotionService(context);
                        await promotionService.EnsurePromotionsExistAsync();
                        
                        // Показываем окно выбора роли
                        var roleSelectionWindow = new Views.RoleSelectionWindow(authService);
                        var roleResult = roleSelectionWindow.ShowDialog();
                        
                        if (roleResult == true)
                        {
                            Window mainWindow;
                            
                            if (roleSelectionWindow.IsUserInterface)
                            {
                                mainWindow = new MainWindow(authService, context);
                            }
                            else if (roleSelectionWindow.IsAdminInterface)
                            {
                                mainWindow = new Views.AdminMainWindow(authService, context);
                            }
                            else
                            {
                                mainWindow = new MainWindow(authService, context);
                            }
                            
                            MainWindow = mainWindow; // Устанавливаем как главное окно приложения
                            
                            // Теперь завершаем приложение при закрытии главного окна
                            ShutdownMode = ShutdownMode.OnMainWindowClose;
                            
                            mainWindow.Show();
                        }
                        else
                        {
                            // Если пользователь закрыл окно выбора роли, закрываем приложение
                            Shutdown();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при создании главного окна:\n{ex.Message}", 
                                      "Ошибка", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Error);
                        System.Diagnostics.Debug.WriteLine($"Ошибка при создании MainWindow: {ex}");
                        Shutdown();
                    }
                }
                else
                {
                    // Если пользователь закрыл окно авторизации, закрываем приложение
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EcoDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<AuthService>();
            services.AddScoped<RecyclingPointService>();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Произошла неожиданная ошибка:\n{e.Exception.Message}\n\nПриложение будет закрыто.", 
                          "Критическая ошибка", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
            
            System.Diagnostics.Debug.WriteLine($"DispatcherUnhandledException: {e.Exception}");
            e.Handled = true;
            Shutdown();
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            MessageBox.Show($"Произошла критическая ошибка:\n{exception?.Message ?? "Неизвестная ошибка"}\n\nПриложение будет закрыто.", 
                          "Критическая ошибка", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
            
            System.Diagnostics.Debug.WriteLine($"UnhandledException: {exception}");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }

}
