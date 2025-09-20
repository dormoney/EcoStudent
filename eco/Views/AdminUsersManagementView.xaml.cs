using eco.Data;
using eco.Services;

namespace eco.Views
{
    public partial class AdminUsersManagementView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;

        public AdminUsersManagementView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;
        }
    }
}
