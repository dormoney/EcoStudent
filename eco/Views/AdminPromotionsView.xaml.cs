using eco.Data;
using eco.Services;

namespace eco.Views
{
    public partial class AdminPromotionsView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;

        public AdminPromotionsView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;
        }
    }
}
