using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using eco.Data;
using eco.Models;
using eco.Services;
using Microsoft.EntityFrameworkCore;

namespace eco.Views
{
    public partial class PromotionsView : UserControl
    {
        private readonly EcoDbContext _context;
        private readonly AuthService _authService;
        private ObservableCollection<PromotionViewModel> _availablePromotions = new();
        private ObservableCollection<PurchasedPromotionViewModel> _purchasedPromotions = new();

        public class PromotionViewModel
        {
            public int PromotionId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int PointsCost { get; set; }
            public string Category { get; set; } = string.Empty;
            public string DiscountValue { get; set; } = string.Empty;
            public bool CanBuy { get; set; }
            public string StatusText { get; set; } = string.Empty;
            public bool IsPurchased { get; set; }
        }

        public class PurchasedPromotionViewModel
        {
            public int UserPromotionId { get; set; }
            public int PromotionId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string DiscountValue { get; set; } = string.Empty;
            public DateTime PurchaseDate { get; set; }
            public DateTime? UsedDate { get; set; }
            public bool IsUsed { get; set; }
            public bool CanUse { get; set; }
            public string StatusText { get; set; } = string.Empty;
        }

        public PromotionsView(EcoDbContext context, AuthService authService)
        {
            InitializeComponent();
            _context = context;
            _authService = authService;
            
            AvailablePromotionsItemsControl.ItemsSource = _availablePromotions;
            PurchasedPromotionsItemsControl.ItemsSource = _purchasedPromotions;
            
            LoadPromotions();
        }

        private async void LoadPromotions()
        {
            if (_authService.CurrentUser == null) return;

            try
            {
                // Обновляем баллы пользователя
                UserPointsTextBlock.Text = _authService.CurrentUser.TotalPoints.ToString();

                // Загружаем доступные акции
                await LoadAvailablePromotions();
                
                // Загружаем купленные акции
                await LoadPurchasedPromotions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки акций: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAvailablePromotions()
        {
            // Загружаем активные акции
            var activePromotions = await _context.Promotions
                .Where(p => p.IsActive)
                .OrderBy(p => p.PointsCost)
                .ToListAsync();

            // Загружаем НЕиспользованные купленные акции пользователя
            var unusedPurchasedPromotionIds = await _context.UserPromotions
                .Where(up => up.UserId == _authService.CurrentUser!.UserId && !up.IsUsed)
                .Select(up => up.PromotionId)
                .ToListAsync();

            _availablePromotions.Clear();

            foreach (var promotion in activePromotions)
            {
                var isUnusedPurchased = unusedPurchasedPromotionIds.Contains(promotion.PromotionId);
                var canBuy = !isUnusedPurchased && _authService.CurrentUser!.TotalPoints >= promotion.PointsCost;

                var viewModel = new PromotionViewModel
                {
                    PromotionId = promotion.PromotionId,
                    Title = promotion.Title,
                    Description = promotion.Description,
                    PointsCost = promotion.PointsCost,
                    Category = promotion.Category ?? "Общее",
                    DiscountValue = promotion.DiscountValue ?? "Скидка",
                    CanBuy = canBuy,
                    IsPurchased = isUnusedPurchased,
                    StatusText = isUnusedPurchased ? "Куплено" : (canBuy ? "Доступно" : "Недостаточно баллов")
                };

                _availablePromotions.Add(viewModel);
            }

            
        }

        private async Task LoadPurchasedPromotions()
        {
            var purchasedPromotions = await _context.UserPromotions
                .Include(up => up.Promotion)
                .Where(up => up.UserId == _authService.CurrentUser!.UserId)
                .OrderByDescending(up => up.PurchaseDate)
                .ToListAsync();

            _purchasedPromotions.Clear();

            foreach (var userPromotion in purchasedPromotions)
            {
                var viewModel = new PurchasedPromotionViewModel
                {
                    UserPromotionId = userPromotion.UserPromotionId,
                    PromotionId = userPromotion.PromotionId,
                    Title = userPromotion.Promotion.Title,
                    Description = userPromotion.Promotion.Description,
                    Category = userPromotion.Promotion.Category ?? "Общее",
                    DiscountValue = userPromotion.Promotion.DiscountValue ?? "Скидка",
                    PurchaseDate = userPromotion.PurchaseDate,
                    UsedDate = userPromotion.UsedDate,
                    IsUsed = userPromotion.IsUsed,
                    CanUse = !userPromotion.IsUsed,
                    StatusText = userPromotion.IsUsed ? 
                        $"Использовано {userPromotion.UsedDate?.ToString("dd.MM.yyyy")}" : 
                        "Доступно для использования"
                };

                _purchasedPromotions.Add(viewModel);
            }

        }

        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is PromotionViewModel promotion)
            {
                if (_authService.CurrentUser == null) return;

                // Проверяем, достаточно ли баллов
                if (_authService.CurrentUser.TotalPoints < promotion.PointsCost)
                {
                    MessageBox.Show("Недостаточно баллов для покупки этой акции!",
                                  "Недостаточно баллов", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    // Создаем запись о покупке
                    var userPromotion = new UserPromotion
                    {
                        UserId = _authService.CurrentUser.UserId,
                        PromotionId = promotion.PromotionId,
                        PurchaseDate = DateTime.Now,
                        IsUsed = false
                    };

                    _context.UserPromotions.Add(userPromotion);

                    // Списываем баллы
                    _authService.CurrentUser.TotalPoints -= promotion.PointsCost;
                    _context.Users.Update(_authService.CurrentUser);

                    await _context.SaveChangesAsync();

                    // Обновляем интерфейс
                    LoadPromotions();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при покупке акции: {ex.Message}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void UseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is PurchasedPromotionViewModel purchasedPromotion)
            {
                if (purchasedPromotion.IsUsed)
                {
                    MessageBox.Show("Эта акция уже использована!",
                                  "Акция использована", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                try
                {
                    // Помечаем акцию как использованную
                    var userPromotion = await _context.UserPromotions
                        .FirstOrDefaultAsync(up => up.UserPromotionId == purchasedPromotion.UserPromotionId);

                    if (userPromotion != null)
                    {
                        userPromotion.IsUsed = true;
                        userPromotion.UsedDate = DateTime.Now;
                        
                        _context.UserPromotions.Update(userPromotion);
                        await _context.SaveChangesAsync();

                        // Обновляем интерфейс
                        await LoadPurchasedPromotions();
                        await LoadAvailablePromotions(); // Обновляем доступные акции - теперь использованная акция снова доступна для покупки
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при использовании акции: {ex.Message}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
