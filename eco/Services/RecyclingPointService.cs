using eco.Data;
using eco.Models;
using Microsoft.EntityFrameworkCore;

namespace eco.Services
{
    public class RecyclingPointService
    {
        private readonly EcoDbContext _context;

        public RecyclingPointService(EcoDbContext context)
        {
            _context = context;
        }

        public async Task EnsureRecyclingPointsExistAsync()
        {
            // Проверяем, есть ли уже пункты сдачи в базе
            var existingCount = await _context.RecyclingPoints.CountAsync();
            
            if (existingCount == 0)
            {
                // Если пунктов нет, добавляем их
                var recyclingPoints = new List<RecyclingPoint>
                {
                    new RecyclingPoint
                    {
                        Name = "Пункт сдачи \"У столовой\"",
                        Latitude = 55.7558,
                        Longitude = 37.6176,
                        Address = "Возле столовой университета",
                        WorkHours = "08:00-20:00",
                        AcceptedTypes = "Пластик, Бумага, Стекло, Металл, Батарейки",
                        ContactPhone = "+7 (495) 123-45-67"
                    },
                    new RecyclingPoint
                    {
                        Name = "Пункт сдачи \"У входа\"",
                        Latitude = 55.7500,
                        Longitude = 37.6200,
                        Address = "Возле главного входа в университет",
                        WorkHours = "07:00-22:00",
                        AcceptedTypes = "Пластик, Бумага, Стекло, Металл, Батарейки",
                        ContactPhone = "+7 (495) 234-56-78"
                    },
                    new RecyclingPoint
                    {
                        Name = "Пункт сдачи \"У библиотеки\"",
                        Latitude = 55.7600,
                        Longitude = 37.6150,
                        Address = "Возле библиотеки университета",
                        WorkHours = "08:00-21:00",
                        AcceptedTypes = "Пластик, Бумага, Стекло, Металл, Батарейки",
                        ContactPhone = "+7 (495) 345-67-89"
                    }
                };

                _context.RecyclingPoints.AddRange(recyclingPoints);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<RecyclingPoint>> GetAllRecyclingPointsAsync()
        {
            return await _context.RecyclingPoints.ToListAsync();
        }
    }
}
