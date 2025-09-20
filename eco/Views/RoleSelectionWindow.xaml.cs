using System.Windows;
using eco.Services;

namespace eco.Views
{
    public partial class RoleSelectionWindow : Window
    {
        private readonly AuthService _authService;
        public bool IsUserInterface { get; private set; }
        public bool IsAdminInterface { get; private set; }

        public RoleSelectionWindow(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            if (_authService.CurrentUser != null)
            {
                WelcomeText.Text = $"Добро пожаловать, {_authService.CurrentUser.FullName}!";
            }

            // Показываем доступные интерфейсы
            if (_authService.IsAdmin)
            {
                AdminCard.Visibility = Visibility.Visible;
            }
        }

        private void UserInterfaceButton_Click(object sender, RoutedEventArgs e)
        {
            IsUserInterface = true;
            DialogResult = true;
            Close();
        }

        private void AdminInterfaceButton_Click(object sender, RoutedEventArgs e)
        {
            IsAdminInterface = true;
            DialogResult = true;
            Close();
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
                DialogResult = false;
                Close();
            }
        }
    }
}
