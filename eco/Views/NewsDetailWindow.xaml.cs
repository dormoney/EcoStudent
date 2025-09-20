using System.Windows;
using eco.Models;

namespace eco.Views
{
    public partial class NewsDetailWindow : Window
    {
        public NewsDetailWindow(News news)
        {
            InitializeComponent();
            LoadNews(news);
        }

        private void LoadNews(News news)
        {
            TitleTextBlock.Text = news.Title;
            DateTextBlock.Text = news.PublishDate.ToString("dd MMMM yyyy");
            ContentTextBlock.Text = news.Content;

            if (!string.IsNullOrEmpty(news.ImagePath))
            {
                try
                {
                    NewsImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(news.ImagePath, UriKind.RelativeOrAbsolute));
                    ImageBorder.Visibility = Visibility.Visible;
                }
                catch
                {
                    ImageBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
