using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using eco.Services;
using eco.Data;
using eco.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eco
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AuthService _authService;
        private readonly EcoDbContext _context;

        public MainWindow(AuthService authService, EcoDbContext context)
        {
            InitializeComponent();
            _authService = authService;
            _context = context;
            
            InitializeUserInterface();
        }

        private void InitializeUserInterface()
        {
            if (_authService.CurrentUser != null)
            {
                UserNameText.Text = _authService.CurrentUser.FullName;
            }
            
            LoadHomeDataAsync();
        }

        



        private async Task LoadHomeDataAsync()
        {
            try
            {
                if (_authService.IsLoggedIn && _authService.CurrentUser != null)
                {
                    var userId = _authService.CurrentUser.UserId;
                    
                    // Загружаем статистику пользователя
                    var submissions = await _context.RecyclingSubmissions
                        .Where(s => s.UserId == userId)
                        .ToListAsync();

                var totalWeight = submissions.Sum(s => s.WeightKg);
                var totalPoints = _authService.CurrentUser.TotalPoints;
                var achievementsCount = await _context.UserAchievements
                    .CountAsync(ua => ua.UserId == userId);

                if (UserPointsText != null) UserPointsText.Text = $"{totalPoints} баллов";
                if (SubmissionsCountText != null) SubmissionsCountText.Text = $"{submissions.Count} сдач";
                if (TotalWeightText != null) TotalWeightText.Text = $"{totalWeight:F1} кг";

                // Загружаем последнюю активность
                LoadRecentActivity(submissions.OrderByDescending(s => s.SubmissionDate).Take(5));
            }
            else
            {
                if (UserPointsText != null) UserPointsText.Text = "0 баллов";
                if (SubmissionsCountText != null) SubmissionsCountText.Text = "0 сдач";
                if (TotalWeightText != null) TotalWeightText.Text = "0 кг";
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecentActivity(IEnumerable<Models.RecyclingSubmission> recentSubmissions)
        {
            if (RecentActivityPanel == null) return;
            
            RecentActivityPanel.Children.Clear();

            if (!recentSubmissions.Any())
            {
                RecentActivityPanel.Children.Add(new TextBlock 
                { 
                    Text = "Пока нет активности. Сдайте первое вторсырье!", 
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Opacity = 0.7 
                });
                return;
            }

            foreach (var submission in recentSubmissions)
            {
                var border = new Border
                {
                    BorderBrush = (Brush)FindResource("MaterialDesignDivider"),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Margin = new Thickness(0, 0, 0, 8),
                    Padding = new Thickness(0, 0, 0, 8)
                };

                var panel = new StackPanel();
                panel.Children.Add(new TextBlock 
                { 
                    Text = $"{submission.MaterialType} - {submission.WeightKg:F1} кг",
                    FontWeight = FontWeights.Bold 
                });
                panel.Children.Add(new TextBlock 
                { 
                    Text = $"{submission.SubmissionDate:dd.MM.yyyy HH:mm} • +{submission.PointsAwarded} баллов",
                    Opacity = 0.7,
                    FontSize = 12 
                });

                border.Child = panel;
                RecentActivityPanel.Children.Add(border);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?", 
                                       "Подтверждение выхода", 
                                       MessageBoxButton.YesNo, 
                                       MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _authService.Logout();
                // Закрываем главное окно и показываем окно входа
                this.Hide();
                
                var loginWindow = new Views.LoginWindow(_authService);
                
                if (loginWindow.ShowDialog() == true)
                {
                    // Показываем окно выбора роли
                    var roleSelectionWindow = new Views.RoleSelectionWindow(_authService);
                    var roleResult = roleSelectionWindow.ShowDialog();
                    
                    if (roleResult == true)
                    {
                        if (roleSelectionWindow.IsUserInterface)
                        {
                            // Остаемся в пользовательском интерфейсе
                            _ = LoadHomeDataAsync();
                            this.Show();
                        }
                        else if (roleSelectionWindow.IsAdminInterface)
                        {
                            // Переключаемся на админский интерфейс
                            var adminWindow = new Views.AdminMainWindow(_authService, _context);
                            adminWindow.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        // Если пользователь закрыл окно выбора роли, закрываем приложение
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    // Если пользователь закрыл окно входа, закрываем приложение
                    Application.Current.Shutdown();
                }
            }
        }

        private void NavigationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavigationListBox?.SelectedIndex < 0 || ContentArea == null) return;

            switch (NavigationListBox.SelectedIndex)
            {
                case 0: // Главная
                    if (HomeGrid != null)
                    {
                        ContentArea.Content = HomeGrid;
                        _ = LoadHomeDataAsync();
                    }
                    break;
                case 1: // Сдать вторсырье
                    ContentArea.Content = new SubmissionView(_context, _authService);
                    break;
                case 2: // Пункты сдачи
                    ContentArea.Content = new RecyclingPointsView(_context);
                    break;
                case 3: // Акции
                    ContentArea.Content = new PromotionsView(_context, _authService);
                    break;
                case 4: // Мой профиль
                    ContentArea.Content = new UserProfileView(_context, _authService);
                    break;
                case 5: // Достижения
                    ContentArea.Content = new AchievementsView(_context, _authService);
                    break;
                case 6: // Челленджи
                    ContentArea.Content = new ChallengesView(_context, _authService);
                    break;
                case 7: // Новости
                    ContentArea.Content = new NewsView(_context);
                    break;
            }
        }
    }
}