using System.Windows;
using System.Windows.Controls;
using eco.Data;
using eco.Services;
using eco.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace eco.Views
{
    public partial class AdminUserRegistrationView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;

        public AdminUserRegistrationView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;
            
            LoadData();
        }

        private async void LoadData()
        {
            await LoadGroups();
            await LoadUsers();
        }

        private async Task LoadGroups()
        {
            try
            {
                var groups = await _context.Groups.ToListAsync();
                GroupComboBox.ItemsSource = groups;
                GroupComboBox.DisplayMemberPath = "GroupName";
                GroupComboBox.SelectedValuePath = "GroupId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadUsers()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Group)
                    .OrderBy(u => u.FullName)
                    .ToListAsync();
                
                UsersDataGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                var user = new User
                {
                    FullName = FullNameTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim().ToLower(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password),
                    GroupId = GroupComboBox.SelectedValue as int?,
                    RegistrationDate = DateTime.Now,
                    TotalPoints = 0
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                
                ClearForm();
                await LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("Введите полное имя пользователя", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                FullNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Введите email пользователя", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Введите пароль", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return false;
            }

            if (PasswordBox.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            FullNameTextBox.Clear();
            EmailTextBox.Clear();
            PasswordBox.Clear();
            GroupComboBox.SelectedIndex = -1;
            UserTypeComboBox.SelectedIndex = 0;
        }

        private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int userId)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого пользователя? Это действие нельзя отменить.", 
                                           "Подтверждение удаления", 
                                           MessageBoxButton.YesNo, 
                                           MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var user = await _context.Users.FindAsync(userId);
                        if (user != null)
                        {
                            _context.Users.Remove(user);
                            await _context.SaveChangesAsync();
                            
                            MessageBox.Show("Пользователь успешно удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadUsers();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
