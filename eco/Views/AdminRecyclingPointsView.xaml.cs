using eco.Data;
using eco.Services;

namespace eco.Views
{
    public partial class AdminRecyclingPointsView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;

        public AdminRecyclingPointsView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;
        }
    }
}
