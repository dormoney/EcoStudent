using eco.Data;
using eco.Models;
using Microsoft.EntityFrameworkCore;

namespace eco.Services
{
    public class PromotionService
    {
        private readonly EcoDbContext _context;

        public PromotionService(EcoDbContext context)
        {
            _context = context;
        }

        public async Task EnsurePromotionsExistAsync()
        {
            if (!await _context.Promotions.AnyAsync())
            {
                var defaultPromotions = new List<Promotion>
                {
                    new Promotion
                    {
                        Title = "Скидка 30% в столовой",
                        Description = "Получите скидку 30% на любой обед в университетской столовой. Покажите QR-код кассиру при оплате.",
                        PointsCost = 50,
                        Category = "Питание",
                        DiscountValue = "30%",
                        IsActive = true
                    },
                    new Promotion
                    {
                        Title = "Бесплатная печать 10 страниц",
                        Description = "Распечатайте до 10 страниц бесплатно в библиотеке университета. Действует в течение месяца с момента покупки.",
                        PointsCost = 30,
                        Category = "Услуги",
                        DiscountValue = "Бесплатно",
                        IsActive = true
                    },
                    new Promotion
                    {
                        Title = "Скидка 50% на кофе",
                        Description = "Получите скидку 50% на любой кофе в кафе университета. Акция действует до конца семестра.",
                        PointsCost = 25,
                        Category = "Питание",
                        DiscountValue = "50%",
                        IsActive = true
                    }
                };

                await _context.Promotions.AddRangeAsync(defaultPromotions);
                await _context.SaveChangesAsync();
            }
        }
    }
}
