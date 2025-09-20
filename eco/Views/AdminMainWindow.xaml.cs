using System.Windows;
using System.Windows.Controls;
using eco.Services;
using eco.Data;
using eco.Views;
using Microsoft.EntityFrameworkCore;

namespace eco.Views
{
    public partial class AdminMainWindow : Window
    {
        private readonly AuthService _authService;
        private readonly EcoDbContext _context;

        public AdminMainWindow(AuthService authService, EcoDbContext context)
        {
            InitializeComponent();
            _authService = authService;
            _context = context;
            
            InitializeAdminInterface();
        }

        private void InitializeAdminInterface()
        {
            if (_authService.CurrentUser != null)
            {
                AdminNameText.Text = _authService.CurrentUser.FullName;
            }
            
            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            try
            {
                // Загружаем статистику
                var totalUsers = await _context.Users.CountAsync();
                var totalSubmissions = await _context.RecyclingSubmissions.CountAsync();
                var pendingVerifications = await _context.RecyclingSubmissions
                    .CountAsync(s => !s.IsVerified);
                var totalPoints = await _context.RecyclingSubmissions
                    .SumAsync(s => s.PointsAwarded);

                TotalUsersText.Text = totalUsers.ToString();
                TotalSubmissionsText.Text = totalSubmissions.ToString();
                PendingVerificationsText.Text = pendingVerifications.ToString();
                TotalPointsText.Text = totalPoints.ToString();

                // Загружаем последние сдачи
                await LoadRecentSubmissions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadRecentSubmissions()
        {
            try
            {
                var recentSubmissions = await _context.RecyclingSubmissions
                    .Include(s => s.User)
                    .Include(s => s.RecyclingPoint)
                    .OrderByDescending(s => s.SubmissionDate)
                    .Take(10)
                    .ToListAsync();

                RecentSubmissionsPanel.Children.Clear();

                if (!recentSubmissions.Any())
                {
                    RecentSubmissionsPanel.Children.Add(new TextBlock 
                    { 
                        Text = "Пока нет сдач", 
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
                    
                    var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    headerPanel.Children.Add(new TextBlock 
                    { 
                        Text = $"{submission.User.FullName}",
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 8, 0)
                    });
                    headerPanel.Children.Add(new TextBlock 
                    { 
                        Text = submission.IsVerified ? "✓ Подтверждено" : "⏳ Ожидает",
                        Foreground = submission.IsVerified ? Brushes.Green : Brushes.Orange,
                        FontSize = 12
                    });
                    panel.Children.Add(headerPanel);
                    
                    panel.Children.Add(new TextBlock 
                    { 
                        Text = $"{submission.MaterialType} - {submission.WeightKg:F1} кг • {submission.RecyclingPoint.Name}",
                        Margin = new Thickness(0, 4, 0, 0)
                    });
                    panel.Children.Add(new TextBlock 
                    { 
                        Text = $"{submission.SubmissionDate:dd.MM.yyyy HH:mm} • +{submission.PointsAwarded} баллов",
                        Opacity = 0.7,
                        FontSize = 12,
                        Margin = new Thickness(0, 2, 0, 0)
                    });

                    border.Child = panel;
                    RecentSubmissionsPanel.Children.Add(border);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке последних сдач: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                this.Hide();
                
                var roleSelectionWindow = new RoleSelectionWindow(_authService);
                
                if (roleSelectionWindow.ShowDialog() == true)
                {
                    if (roleSelectionWindow.IsUserInterface)
                    {
                        // Переключаемся на пользовательский интерфейс
                        var mainWindow = new MainWindow(_authService, _context);
                        mainWindow.Show();
                        this.Close();
                    }
                    else if (roleSelectionWindow.IsAdminInterface)
                    {
                        // Остаемся в админском интерфейсе
                        this.Show();
                    }
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void NavigationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavigationListBox?.SelectedIndex < 0 || ContentArea == null) return;

            switch (NavigationListBox.SelectedIndex)
            {
                case 0: // Панель управления
                    if (DashboardGrid != null)
                    {
                        ContentArea.Content = DashboardGrid;
                        LoadDashboardData();
                    }
                    break;
                case 1: // Регистрация пользователей
                    ContentArea.Content = new AdminUserRegistrationView(_context, _authService);
                    break;
                case 2: // Управление пунктами
                    ContentArea.Content = new AdminRecyclingPointsView(_context, _authService);
                    break;
                case 3: // Подтверждение сдач
                    ContentArea.Content = new AdminVerificationView(_context, _authService);
                    break;
                case 4: // Управление пользователями
                    ContentArea.Content = new AdminUsersManagementView(_context, _authService);
                    break;
                case 5: // Управление акциями
                    ContentArea.Content = new AdminPromotionsView(_context, _authService);
                    break;
                case 6: // Статистика
                    ContentArea.Content = new AdminStatisticsView(_context, _authService);
                    break;
            }
        }
    }
}
