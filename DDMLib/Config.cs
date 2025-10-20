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
        private static FileIniDataParser _iniParser;

        public static string ConnectionString { get; private set; }
        public static bool IsDatabaseConnected { get; private set; }

        static Config()
        {
            InitializeConfiguration();
            TestDatabaseConnection();
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
                _iniParser = new FileIniDataParser();
                var iniData = _iniParser.ReadFile(IniFilePath);

                ConnectionString = GetIniValue(iniData, "Database", "ConnectionString");

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

        private static string GetIniValue(IniData iniData, string section, string key)
        {
            var value = iniData[section]?[key];

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    $"Отсутствует обязательный параметр [{section}]/{key} в конфигурационном файле.");
            }

            return value.Trim();
        }

        private static void TestDatabaseConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    IsDatabaseConnected = true;
                }
            }
            catch (Exception ex)
            {
                IsDatabaseConnected = false;
                throw new InvalidOperationException(
                    $"Не удалось установить соединение с базой данных: {ex.Message}", ex);
            }
        }
    }
}