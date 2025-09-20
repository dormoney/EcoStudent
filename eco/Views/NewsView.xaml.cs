using System.Windows;
using System.Windows.Controls;
using eco.Data;
using eco.Models;
using Microsoft.EntityFrameworkCore;

namespace eco.Views
{
    public partial class NewsView : UserControl
    {
        private readonly EcoDbContext _context;

        public NewsView(EcoDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadNews();
        }

        private async void LoadNews()
        {
            try
            {
                var news = await _context.News
                    .OrderByDescending(n => n.PublishDate)
                    .ToListAsync();

                NewsItemsControl.ItemsSource = news;
                NoNewsCard.Visibility = news.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки новостей: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is News news)
            {
                var newsDetailWindow = new NewsDetailWindow(news);
                newsDetailWindow.ShowDialog();
            }
        }
    }
}
