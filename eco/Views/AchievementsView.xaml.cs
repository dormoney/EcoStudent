using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using eco.Data;
using eco.Services;
using eco.Models;
using Microsoft.EntityFrameworkCore;

namespace eco.Views
{
    public partial class AchievementsView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;
        private List<AchievementProgress> _allAchievements = new();
        private ObservableCollection<AchievementProgress> _filteredAchievements = new();

        public class AchievementProgress
        {
            public Achievement Achievement { get; set; } = null!;
            public bool IsEarned { get; set; }
            public double Progress { get; set; }
            public string ProgressText { get; set; } = string.Empty;
            public DateTime? DateEarned { get; set; }
        }

        public AchievementsView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;
            
            AchievementsItemsControl.ItemsSource = _filteredAchievements;
            LoadAchievements();
        }

        private async void LoadAchievements()
        {
            try
            {
                var achievements = await _context.Achievements.ToListAsync();
                var userAchievements = new List<UserAchievement>();
                var userPoints = 0;

                if (_authService.CurrentUser != null)
                {
                    userAchievements = await _context.UserAchievements
                        .Where(ua => ua.UserId == _authService.CurrentUser.UserId)
                        .ToListAsync();
                    
                    userPoints = _authService.CurrentUser.TotalPoints;
                }

                _allAchievements.Clear();

                foreach (var achievement in achievements)
                {
                    var userAchievement = userAchievements.FirstOrDefault(ua => ua.AchievementId == achievement.AchievementId);
                    var isEarned = userAchievement != null;
                    
                    var progress = userPoints >= achievement.RequiredPoints ? 100.0 : 
                                  (double)userPoints / achievement.RequiredPoints * 100.0;
                    
                    var progressText = isEarned ? 
                        $"Получено {userAchievement!.DateEarned:dd.MM.yyyy}" :
                        $"{userPoints}/{achievement.RequiredPoints} баллов";

                    _allAchievements.Add(new AchievementProgress
                    {
                        Achievement = achievement,
                        IsEarned = isEarned,
                        Progress = progress,
                        ProgressText = progressText,
                        DateEarned = userAchievement?.DateEarned
                    });
                }

                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки достижений: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterRadio_Checked(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = _allAchievements.AsEnumerable();

            if (EarnedRadio?.IsChecked == true)
            {
                filtered = filtered.Where(a => a.IsEarned);
            }
            else if (AvailableRadio?.IsChecked == true)
            {
                filtered = filtered.Where(a => !a.IsEarned);
            }

            _filteredAchievements.Clear();
            
            // Сортируем: сначала полученные (по дате), потом доступные (по прогрессу)
            var sortedAchievements = filtered
                .OrderByDescending(a => a.IsEarned)
                .ThenByDescending(a => a.DateEarned ?? DateTime.MinValue)
                .ThenByDescending(a => a.Progress);

            foreach (var achievement in sortedAchievements)
            {
                _filteredAchievements.Add(achievement);
            }

            NoAchievementsCard.Visibility = _filteredAchievements.Count == 0 ? 
                Visibility.Visible : Visibility.Collapsed;
        }
    }
}
