using System.Windows;
using System.Windows.Controls;
using eco.Data;
using eco.Services;
using eco.Models;
using Microsoft.EntityFrameworkCore;

namespace eco.Views
{
    public partial class SubmissionView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;

        public SubmissionView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;
            
            LoadData();
        }

        private async void LoadData()
        {
            await LoadRecyclingPoints();
            await LoadSubmissionHistory();
        }

        private async Task LoadRecyclingPoints()
        {
            try
            {
                var points = await _context.RecyclingPoints.ToListAsync();
                RecyclingPointComboBox.ItemsSource = points;
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка загрузки пунктов сдачи: {ex.Message}", true);
            }
        }

        private async Task LoadSubmissionHistory()
        {
            try
            {
                if (_authService.CurrentUser != null)
                {
                    var submissions = await _context.RecyclingSubmissions
                        .Include(s => s.RecyclingPoint)
                        .Where(s => s.UserId == _authService.CurrentUser.UserId)
                        .OrderByDescending(s => s.SubmissionDate)
                        .ToListAsync();

                    SubmissionsDataGrid.ItemsSource = submissions;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка загрузки истории: {ex.Message}", true);
            }
        }


        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                var selectedPoint = RecyclingPointComboBox.SelectedItem as RecyclingPoint;
                var materialType = (MaterialTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                var weight = double.Parse(WeightTextBox.Text);

                // Рассчитываем баллы на основе типа материала
                int pointsAwarded = CalculatePoints(materialType!, weight);


                var submission = new RecyclingSubmission
                {
                    UserId = _authService.CurrentUser!.UserId,
                    PointId = selectedPoint!.PointId,
                    MaterialType = materialType!,
                    WeightKg = weight,
                    SubmissionDate = DateTime.Now,
                    PhotoPath = null,
                    IsVerified = false, // Требует верификации
                    PointsAwarded = pointsAwarded
                };

                _context.RecyclingSubmissions.Add(submission);

                // Обновляем общие баллы пользователя (предварительно, до верификации)
                _authService.CurrentUser.TotalPoints += pointsAwarded;
                _context.Users.Update(_authService.CurrentUser);

                await _context.SaveChangesAsync();

                ShowMessage($"Сдача успешно зарегистрирована! Начислено {pointsAwarded} баллов.", false);
                ClearForm();
                await LoadSubmissionHistory();
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при сдаче: {ex.Message}", true);
            }
        }

        private bool ValidateForm()
        {
            if (RecyclingPointComboBox.SelectedItem == null)
            {
                ShowMessage("Выберите пункт сдачи.", true);
                return false;
            }

            if (MaterialTypeComboBox.SelectedItem == null)
            {
                ShowMessage("Выберите тип материала.", true);
                return false;
            }

            if (string.IsNullOrWhiteSpace(WeightTextBox.Text) || 
                !double.TryParse(WeightTextBox.Text, out double weight) || 
                weight <= 0)
            {
                ShowMessage("Введите корректный вес (больше 0).", true);
                return false;
            }

            return true;
        }

        private int CalculatePoints(string materialType, double weight)
        {
            var pointsPerKg = materialType.ToLower() switch
            {
                "пластик" => 10,
                "бумага" => 5,
                "стекло" => 15,
                "металл" => 25,
                "батарейки" => 50,
                _ => 5
            };

            return (int)(weight * pointsPerKg);
        }


        private void ClearForm()
        {
            RecyclingPointComboBox.SelectedItem = null;
            MaterialTypeComboBox.SelectedItem = null;
            WeightTextBox.Text = "";
        }

        private void ShowMessage(string message, bool isError)
        {
            MessageTextBlock.Text = message;
            MessageTextBlock.Foreground = isError ? 
                (System.Windows.Media.Brush)FindResource("MaterialDesignValidationErrorBrush") :
                (System.Windows.Media.Brush)FindResource("PrimaryHueMidBrush");
            MessageTextBlock.Visibility = Visibility.Visible;
        }
    }
}
