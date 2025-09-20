using eco.Models;
using eco.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace eco.Services
{
    public class AuthService
    {
        private readonly EcoDbContext _context;
        private readonly IConfiguration _configuration;
        public User? CurrentUser { get; private set; }
        public bool IsAdmin { get; private set; }

        public AuthService(EcoDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Group)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    CurrentUser = user;
                    
                    // Проверяем админские права
                    var adminEmails = _configuration.GetSection("AdminEmails").Get<string[]>() ?? new string[0];
                    
                    IsAdmin = adminEmails.Contains(email, StringComparer.OrdinalIgnoreCase);
                    
                    return true;
                }

                return false;
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                // Логируем ошибку SQL
                System.Diagnostics.Debug.WriteLine($"Ошибка SQL при авторизации: {sqlEx.Message}");
                throw new Exception("Ошибка подключения к базе данных. Проверьте подключение к интернету.");
            }
            catch (Exception ex)
            {
                // Логируем общую ошибку
                System.Diagnostics.Debug.WriteLine($"Ошибка при авторизации: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                throw new Exception($"Произошла ошибка при авторизации: {ex.Message}");
            }
        }

        public async Task<bool> RegisterAsync(string fullName, string email, string password, int? groupId = null)
        {
            try
            {
                // Проверяем, существует ли пользователь с таким email
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                    return false;

                var user = new User
                {
                    FullName = fullName,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    GroupId = groupId,
                    RegistrationDate = DateTime.Now,
                    TotalPoints = 0
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // НЕ устанавливаем CurrentUser - пользователь должен войти отдельно
                return true;
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                // Логируем ошибку SQL
                System.Diagnostics.Debug.WriteLine($"Ошибка SQL при регистрации: {sqlEx.Message}");
                
                if (sqlEx.Number == 2) // Timeout
                    throw new Exception("Превышено время ожидания подключения к базе данных.");
                else if (sqlEx.Number == 18456) // Login failed
                    throw new Exception("Ошибка доступа к базе данных.");
                else
                    throw new Exception("Ошибка подключения к базе данных. Проверьте подключение к интернету.");
            }
            catch (Exception ex)
            {
                // Логируем общую ошибку
                System.Diagnostics.Debug.WriteLine($"Ошибка при регистрации: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                throw new Exception($"Произошла ошибка при регистрации: {ex.Message}");
            }
        }

        public void Logout()
        {
            CurrentUser = null;
            IsAdmin = false;
        }

        public bool IsLoggedIn => CurrentUser != null;
    }
}
