using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using eco.Data;
using eco.Services;
using eco.Models;
using Microsoft.EntityFrameworkCore;

namespace eco.Views
{
    public partial class ChallengesView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;

        public class ChallengeProgress
        {
            public Challenge Challenge { get; set; } = null!;
            public UserChallenge? UserChallenge { get; set; }
            public double Progress { get; set; }
            public string ProgressText { get; set; } = string.Empty;
            public bool IsCompleted { get; set; }
            public bool CanJoin { get; set; }
            public DateTime? CompletionDate { get; set; }
        }

        public ChallengesView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;
            
            LoadChallenges();
        }

        private async void LoadChallenges()
        {
            try
            {
                var activeChallenges = new ObservableCollection<ChallengeProgress>();
                var completedChallenges = new ObservableCollection<ChallengeProgress>();

                if (_authService.CurrentUser != null)
                {
                    var userId = _authService.CurrentUser.UserId;
                    var now = DateTime.Now;

                    // Загружаем активные челленджи
                    var challenges = await _context.Challenges
                        .Where(c => c.IsActive && c.EndDate > now)
                        .ToListAsync();

                    var userChallenges = await _context.UserChallenges
                        .Where(uc => uc.UserId == userId)
                        .Include(uc => uc.Challenge)
                        .ToListAsync();

                    foreach (var challenge in challenges)
                    {
                        var userChallenge = userChallenges.FirstOrDefault(uc => uc.ChallengeId == challenge.ChallengeId);
                        var progress = 0.0;
                        var progressText = "0";
                        var isCompleted = userChallenge?.IsCompleted ?? false;
                        var canJoin = userChallenge == null;

                        if (userChallenge != null)
                        {
                            progress = Math.Min(100, (userChallenge.CurrentValue / challenge.TargetValue) * 100);
                            progressText = $"{userChallenge.CurrentValue:F1}/{challenge.TargetValue:F1}";
                        }

                        activeChallenges.Add(new ChallengeProgress
                        {
                            Challenge = challenge,
                            UserChallenge = userChallenge,
                            Progress = progress,
                            ProgressText = progressText,
                            IsCompleted = isCompleted,
                            CanJoin = canJoin
                        });
                    }

                    // Загружаем завершенные челленджи
                    var completedUserChallenges = userChallenges
                        .Where(uc => uc.IsCompleted)
                        .OrderByDescending(uc => uc.CompletionDate);

                    foreach (var userChallenge in completedUserChallenges)
                    {
                        completedChallenges.Add(new ChallengeProgress
                        {
                            Challenge = userChallenge.Challenge,
                            UserChallenge = userChallenge,
                            Progress = 100,
                            ProgressText = "Завершено",
                            IsCompleted = true,
                            CanJoin = false,
                            CompletionDate = userChallenge.CompletionDate
                        });
                    }
                }

                ActiveChallengesItemsControl.ItemsSource = activeChallenges;
                CompletedChallengesItemsControl.ItemsSource = completedChallenges;

                NoChallengesCard.Visibility = activeChallenges.Count == 0 && completedChallenges.Count == 0 ?
                    Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки челленджей: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void JoinChallengeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ChallengeProgress challengeProgress)
            {
                if (!challengeProgress.CanJoin)
                    return;

                try
                {
                    var userChallenge = new UserChallenge
                    {
                        UserId = _authService.CurrentUser!.UserId,
                        ChallengeId = challengeProgress.Challenge.ChallengeId,
                        CurrentValue = 0,
                        IsCompleted = false
                    };

                    _context.UserChallenges.Add(userChallenge);
                    await _context.SaveChangesAsync();

                    MessageBox.Show($"Вы присоединились к челленджу \"{challengeProgress.Challenge.Title}\"!", 
                                  "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Перезагружаем список
                    LoadChallenges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка присоединения к челленджу: {ex.Message}", 
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
