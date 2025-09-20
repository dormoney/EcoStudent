using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using eco.Data;
using eco.Services;
using Microsoft.EntityFrameworkCore;

namespace eco.Views
{
    public partial class UserProfileView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;

        public class MaterialStatistic
        {
            public string MaterialType { get; set; } = string.Empty;
            public double Weight { get; set; }
            public int Points { get; set; }
            public double Percentage { get; set; }
        }

        public UserProfileView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;

            LoadUserProfile();
        }

        private async void LoadUserProfile()
        {
            if (_authService.CurrentUser == null) return;

            try
            {
                var user = await _context.Users
                    .Include(u => u.Group)
                    .FirstOrDefaultAsync(u => u.UserId == _authService.CurrentUser.UserId);

                if (user == null) return;

                // Базовая информация
                UserNameTextBlock.Text = user.FullName;
                UserEmailTextBlock.Text = user.Email;
                UserGroupTextBlock.Text = user.Group?.GroupName ?? "Группа не указана";
                RegistrationDateTextBlock.Text = user.RegistrationDate.ToString("dd.MM.yyyy");

                // Загружаем статистику
                await LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки профиля: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadStatistics()
        {
            var userId = _authService.CurrentUser!.UserId;

            var submissions = await _context.RecyclingSubmissions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            // Пересчитываем баллы из базы данных
            var totalPoints = submissions.Sum(s => s.PointsAwarded);
            var totalWeight = submissions.Sum(s => s.WeightKg);
            var submissionsCount = submissions.Count;

            TotalPointsTextBlock.Text = totalPoints.ToString();
            SubmissionsCountTextBlock.Text = submissionsCount.ToString();
            TotalWeightTextBlock.Text = $"{totalWeight:F1} кг";
        }

        private async Task LoadMaterialStatistics()
        {
            var userId = _authService.CurrentUser!.UserId;

            var materialStats = await _context.RecyclingSubmissions
                .Where(s => s.UserId == userId)
                .GroupBy(s => s.MaterialType)
                .Select(g => new MaterialStatistic
                {
                    MaterialType = g.Key,
                    Weight = g.Sum(s => s.WeightKg),
                    Points = g.Sum(s => s.PointsAwarded)
                })
                .ToListAsync();

            if (materialStats.Any())
            {
                var maxWeight = materialStats.Max(m => m.Weight);
                foreach (var stat in materialStats)
                {
                    stat.Percentage = maxWeight > 0 ? (stat.Weight / maxWeight) * 100 : 0;
                }


            }
        }

        

    }
}
