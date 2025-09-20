using System.Windows;
using eco.Services;
using eco.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eco.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _authService;
        private readonly EcoDbContext _context;

        public RegisterWindow(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            _context = App.ServiceProvider.GetRequiredService<EcoDbContext>();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Скрываем предыдущие ошибки
            HideError();
            
            // Валидация полей
            if (!ValidateRegistrationForm())
            {
                return;
            }

            // Отключаем кнопку регистрации
            RegisterButton.IsEnabled = false;

            try
            {
                int? groupId = await GetOrCreateGroupId(GroupTextBox.Text.Trim());
                
                bool success = await _authService.RegisterAsync(
                    FullNameTextBox.Text.Trim(), 
                    EmailTextBox.Text.Trim(), 
                    PasswordBox.Password,
                    groupId);
                
                if (success)
                {
                    DialogResult = false; // Возвращаемся к окну входа
                    Close();
                }
                else
                {
                    ShowError("Пользователь с таким email уже существует. Попробуйте другой email.");
                    EmailTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    EmailTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                // Возвращаем состояние кнопки
                RegisterButton.IsEnabled = true;
            }
        }

        private bool ValidateRegistrationForm()
        {
            // Валидация ФИО
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                ShowError("Введите ваше ФИО.");
                FullNameTextBox.Focus();
                return false;
            }

            if (FullNameTextBox.Text.Trim().Length < 2)
            {
                ShowError("ФИО должно содержать минимум 2 символа.");
                FullNameTextBox.Focus();
                return false;
            }

            // Валидация Email
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowError("Введите email адрес.");
                EmailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(EmailTextBox.Text.Trim()))
            {
                ShowError("Введите корректный email адрес.");
                EmailTextBox.Focus();
                return false;
            }

            // Валидация пароля
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowError("Введите пароль.");
                PasswordBox.Focus();
                return false;
            }

            if (PasswordBox.Password.Length < 6)
            {
                ShowError("Пароль должен содержать минимум 6 символов.");
                PasswordBox.Focus();
                return false;
            }

            if (!ContainsLetter(PasswordBox.Password) || !ContainsDigit(PasswordBox.Password))
            {
                ShowError("Пароль должен содержать как минимум одну букву и одну цифру.");
                PasswordBox.Focus();
                return false;
            }

            // Валидация подтверждения пароля
            if (string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password))
            {
                ShowError("Подтвердите пароль.");
                ConfirmPasswordBox.Focus();
                return false;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ShowError("Пароли не совпадают.");
                ConfirmPasswordBox.Focus();
                HighlightPasswordFields();
                return false;
            }

            // Валидация группы (если указана)
            if (!string.IsNullOrWhiteSpace(GroupTextBox.Text) && GroupTextBox.Text.Trim().Length < 2)
            {
                ShowError("Название группы должно содержать минимум 2 символа.");
                GroupTextBox.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Простая проверка регулярным выражением
                return System.Text.RegularExpressions.Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private bool ContainsLetter(string password)
        {
            return password.Any(char.IsLetter);
        }

        private bool ContainsDigit(string password)
        {
            return password.Any(char.IsDigit);
        }

        private void HighlightPasswordFields()
        {
            PasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
            ConfirmPasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
        }

        private void ClearFieldHighlights()
        {
            FullNameTextBox.ClearValue(System.Windows.Controls.TextBox.BorderBrushProperty);
            EmailTextBox.ClearValue(System.Windows.Controls.TextBox.BorderBrushProperty);
            PasswordBox.ClearValue(System.Windows.Controls.PasswordBox.BorderBrushProperty);
            ConfirmPasswordBox.ClearValue(System.Windows.Controls.PasswordBox.BorderBrushProperty);
            GroupTextBox.ClearValue(System.Windows.Controls.TextBox.BorderBrushProperty);
        }

        private async Task<int?> GetOrCreateGroupId(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return null; // Группа необязательна
            }

            try
            {
                // Ищем существующую группу по названию
                var existingGroup = await _context.Groups
                    .FirstOrDefaultAsync(g => g.GroupName.ToLower() == groupName.ToLower());

                if (existingGroup != null)
                {
                    return existingGroup.GroupId;
                }

                // Создаем новую группу, если не найдена
                var newGroup = new Models.Group
                {
                    GroupName = groupName,
                    Faculty = "Не указан" // Можно позже добавить поле для факультета
                };

                _context.Groups.Add(newGroup);
                await _context.SaveChangesAsync();

                return newGroup.GroupId;
            }
            catch (Exception)
            {
                // В случае ошибки возвращаем null
                return null;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void HideError()
        {
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            ClearFieldHighlights();
        }

        // Обработчики событий для очистки ошибок при фокусе на полях
        private void FullNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearFieldHighlights();
            HideError();
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearFieldHighlights();
            HideError();
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearFieldHighlights();
            HideError();
        }

        private void ConfirmPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearFieldHighlights();
            HideError();
        }

        private void GroupTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearFieldHighlights();
            HideError();
        }
    }
}
