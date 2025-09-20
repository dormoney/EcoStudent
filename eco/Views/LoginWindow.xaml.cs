using System.Windows;
using eco.Services;

namespace eco.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        public LoginWindow(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Скрываем предыдущие ошибки
            HideError();
            
            // Валидация полей
            if (!ValidateLoginForm())
            {
                return;
            }

            // Отключаем кнопку входа для предотвращения множественных нажатий
            LoginButton.IsEnabled = false;

            try
            {
                // Дополнительная валидация перед авторизацией
                if (!ValidateLoginForm())
                {
                    return;
                }

                bool success = await _authService.LoginAsync(EmailTextBox.Text.Trim(), PasswordBox.Password);
                
                if (success)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    ShowError("Неверный email или пароль. Проверьте правильность введенных данных.");
                    HighlightErrorFields();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                System.Diagnostics.Debug.WriteLine($"Ошибка в LoginButton_Click: {ex}");
            }
            finally
            {
                // Возвращаем состояние кнопки
                LoginButton.IsEnabled = true;
            }
        }

        private bool ValidateLoginForm()
        {
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

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowError("Введите пароль.");
                PasswordBox.Focus();
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

        private void HighlightErrorFields()
        {
            // Подсвечиваем поля с ошибками красной рамкой
            EmailTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            PasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
        }

        private void ClearFieldHighlights()
        {
            // Убираем красную рамку с полей
            EmailTextBox.ClearValue(System.Windows.Controls.TextBox.BorderBrushProperty);
            PasswordBox.ClearValue(System.Windows.Controls.PasswordBox.BorderBrushProperty);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_authService);
            registerWindow.ShowDialog();
            
            // После закрытия окна регистрации просто остаемся на окне входа
            // Пользователь должен войти самостоятельно
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void HideError()
        {
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            ClearFieldHighlights();
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
    }
}
