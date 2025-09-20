using System.Globalization;
using System.Windows.Data;

namespace eco.Converters
{
    public class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Для разных контекстов можем возвращать разные строки
                if (parameter != null)
                {
                    var parameters = parameter.ToString()?.Split('|');
                    if (parameters != null && parameters.Length == 2)
                    {
                        return boolValue ? parameters[0] : parameters[1];
                    }
                }

                // По умолчанию для статуса верификации
                return boolValue ? "Проверено" : "Ожидает проверки";
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ChallengeStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted ? "Завершен" : "Участвовать";
            }
            return "Участвовать";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
