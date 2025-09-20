using System.Windows;
using eco.Models;

namespace eco.Views
{
    public partial class SubmissionDetailsWindow : Window
    {
        private readonly RecyclingSubmission _submission;

        public SubmissionDetailsWindow(RecyclingSubmission submission)
        {
            InitializeComponent();
            _submission = submission;
            
            LoadSubmissionDetails();
        }

        private void LoadSubmissionDetails()
        {
            // User Information
            UserNameText.Text = _submission.User.FullName;
            UserEmailText.Text = _submission.User.Email;
            UserGroupText.Text = _submission.User.Group?.GroupName ?? "Не указана";

            // Submission Information
            SubmissionIdText.Text = _submission.SubmissionId.ToString();
            MaterialTypeText.Text = _submission.MaterialType;
            WeightText.Text = $"{_submission.WeightKg:F1} кг";
            RecyclingPointText.Text = _submission.RecyclingPoint.Name;
            SubmissionDateText.Text = _submission.SubmissionDate.ToString("dd.MM.yyyy HH:mm");
            PointsText.Text = $"{_submission.PointsAwarded} баллов";

            // Status Information
            if (_submission.IsVerified)
            {
                StatusIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckCircle;
                StatusText.Text = "Подтверждено";
                StatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                StatusIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Clock;
                StatusText.Text = "Ожидает подтверждения";
                StatusText.Foreground = System.Windows.Media.Brushes.Orange;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
