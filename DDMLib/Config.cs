using System;
using System.Data;
using System.IO;
using IniParser;
using IniParser.Model;
using MySql.Data.MySqlClient;

namespace DDMLib
{
    public static class Config
    {
        private static readonly string IniFilePath = "config.ini";

        public static string ConnectionString { get; private set; }

        static Config()
        {
            InitializeConfiguration();
        }

        private static void InitializeConfiguration()
        {
            if (!File.Exists(IniFilePath))
            {
                throw new FileNotFoundException(
                    $"Конфигурационный файл {IniFilePath} не найден. " +
                    "Создайте файл config.ini с необходимыми настройками.");
            }

            try
            {
                var parser = new FileIniDataParser();
                var iniData = parser.ReadFile(IniFilePath);

                ConnectionString = iniData["Database"]?["ConnectionString"]?.Trim();

                if (string.IsNullOrWhiteSpace(ConnectionString))
                {
                    throw new ArgumentException(
                        "Строка подключения к базе данных не указана в конфигурационном файле.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Ошибка чтения конфигурационного файла: {ex.Message}", ex);
            }
        }

        public static bool TestDatabaseConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}