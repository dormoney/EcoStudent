using System.Windows;
using System.Windows.Controls;
using eco.Data;
using eco.Models;
using Microsoft.EntityFrameworkCore;

namespace eco.Views
{
    public partial class RecyclingPointsView : UserControl
    {
        private readonly EcoDbContext _context;

        public RecyclingPointsView(EcoDbContext context)
        {
            InitializeComponent();
            _context = context;
            
            
            LoadRecyclingPoints();
        }

        private async void LoadRecyclingPoints()
        {
            try
            {
                var recyclingPoints = await _context.RecyclingPoints.OrderBy(p => p.Name).ToListAsync();
                RecyclingPointsItemsControl.ItemsSource = recyclingPoints;
                
                // Показываем сообщение, если нет пунктов
                NoResultsCard.Visibility = recyclingPoints.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пунктов сдачи: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
