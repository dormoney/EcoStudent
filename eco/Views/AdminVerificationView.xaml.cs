using System.Windows;
using System.Windows.Controls;
using eco.Data;
using eco.Services;
using eco.Models;
using Microsoft.EntityFrameworkCore;

namespace eco.Views
{
    public partial class AdminVerificationView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;
        private RecyclingSubmission? _selectedSubmission;

        public AdminVerificationView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;
            
            LoadData();
        }

        private async void LoadData()
        {
            await LoadMaterialTypes();
            await LoadSubmissions();
        }

        private async Task LoadMaterialTypes()
        {
            try
            {
                var materialTypes = await _context.RecyclingSubmissions
                    .Select(s => s.MaterialType)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToListAsync();

                MaterialFilterComboBox.ItemsSource = materialTypes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов материалов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadSubmissions()
        {
            try
            {
                var query = _context.RecyclingSubmissions
                    .Include(s => s.User)
                    .Include(s => s.RecyclingPoint)
                    .AsQueryable();

                // Применяем фильтры
                if (StatusFilterComboBox.SelectedItem is ComboBoxItem statusItem)
                {
                    var status = statusItem.Tag?.ToString();
                    if (status == "Pending")
                        query = query.Where(s => !s.IsVerified);
                    else if (status == "Verified")
                        query = query.Where(s => s.IsVerified);
                }

                if (MaterialFilterComboBox.SelectedItem is string materialType)
                {
                    query = query.Where(s => s.MaterialType == materialType);
                }

                if (DateFilterPicker.SelectedDate.HasValue)
                {
                    var selectedDate = DateFilterPicker.SelectedDate.Value.Date;
                    query = query.Where(s => s.SubmissionDate.Date == selectedDate);
                }

                var submissions = await query
                    .OrderByDescending(s => s.SubmissionDate)
                    .ToListAsync();

                SubmissionsDataGrid.ItemsSource = submissions;
                SubmissionsCountText.Text = $"({submissions.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сдач: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSubmissions();
        }

        private void SubmissionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSubmission = SubmissionsDataGrid.SelectedItem as RecyclingSubmission;
            
            bool hasSelection = _selectedSubmission != null;
            ApproveButton.IsEnabled = hasSelection && !_selectedSubmission.IsVerified;
            RejectButton.IsEnabled = hasSelection && !_selectedSubmission.IsVerified;
            ViewDetailsButton.IsEnabled = hasSelection;
        }

        private async void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubmission == null) return;

            try
            {
                _selectedSubmission.IsVerified = true;
                
                // Обновляем общие баллы пользователя
                _selectedSubmission.User.TotalPoints += _selectedSubmission.PointsAwarded;
                
                await _context.SaveChangesAsync();
                
                MessageBox.Show($"Сдача подтверждена! Пользователю {_selectedSubmission.User.FullName} начислено {_selectedSubmission.PointsAwarded} баллов.", 
                              "Подтверждение", MessageBoxButton.OK, MessageBoxImage.Information);
                
                await LoadSubmissions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подтверждении сдачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubmission == null) return;

            var result = MessageBox.Show("Вы уверены, что хотите отклонить эту сдачу?", 
                                       "Подтверждение отклонения", 
                                       MessageBoxButton.YesNo, 
                                       MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.RecyclingSubmissions.Remove(_selectedSubmission);
                    await _context.SaveChangesAsync();
                    
                    MessageBox.Show($"Сдача отклонена и удалена из системы.", 
                                  "Отклонение", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    await LoadSubmissions();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отклонении сдачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubmission == null) return;

            var detailsWindow = new SubmissionDetailsWindow(_selectedSubmission);
            detailsWindow.ShowDialog();
        }
    }
}
