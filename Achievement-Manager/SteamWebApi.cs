using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Project3_1
{
    /// <summary>
    /// Класс для представления информации о достижении в Steam, включая его название и процент выполнения.
    /// </summary>
    public class SteamAchievement
    {
        /// <summary>
        /// Название достижения.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Процент выполнения достижения.
        /// </summary>
        public double Percent { get; set; }
    }

    /// <summary>
    /// Класс для взаимодействия с API Steam, получения данных о достижениях.
    /// </summary>
    public class SteamWebAPI
    {
        // Создаем статичный HttpClient для отправки HTTP-запросов
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Асинхронный метод для получения данных о достижениях игры через API Steam.
        /// </summary>
        /// <param name="appId">Идентификатор приложения в Steam (ID игры).</param>
        /// <returns>Список достижений игры с их процентами выполнения.</returns>
        public static async Task<List<SteamAchievement>> GetAchievementPercentagesAsync(int appId)
        {
            var achievements = new List<SteamAchievement>();

            // Формируем URL для запроса
            string url = $"https://api.steampowered.com/ISteamUserStats/GetGlobalAchievementPercentagesForApp/v0002/?gameid={appId}";

            try
            {
                // Отправляем GET-запрос
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Читаем JSON-ответ
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Парсим JSON вручную
                    achievements = ParseSteamJson(jsonResponse);
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
            }

            return achievements;
        }

        /// <summary>
        /// Метод для парсинга JSON-ответа от Steam API.
        /// </summary>
        /// <param name="json">JSON строка, полученная от Steam API.</param>
        /// <returns>Список достижений с их данными (название и процент выполнения).</returns>
        private static List<SteamAchievement> ParseSteamJson(string json)
        {
            var achievements = new List<SteamAchievement>();

            try
            {
                // Ищем начало массива achievements
                int startIndex = json.IndexOf("\"achievements\":") + "\"achievements\":".Length;
                int endIndex = json.IndexOf("]", startIndex);

                if (startIndex >= 0 && endIndex >= 0)
                {
                    string achievementsJson = json.Substring(startIndex, endIndex - startIndex + 1);

                    // Разделяем JSON на отдельные объекты
                    var achievementEntries = achievementsJson.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                    

                    foreach (var entry in achievementEntries)
                    {
                        if (entry.Contains("name") && entry.Contains("percent"))
                        {
                            string name = ExtractValue(entry, "name");
                            string percentStr = ExtractValue(entry, "percent");
                            if (double.TryParse(percentStr.Replace(".", ","), out double percent))
                            {
                                achievements.Add(new SteamAchievement
                                {
                                    Name = name,
                                    Percent = percent
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при парсинге JSON: {ex.Message}");
            }

            return achievements;
        }

        /// <summary>
        /// Метод для извлечения значения из JSON-строки по ключу.
        /// </summary>
        /// <param name="json">JSON строка для извлечения значения.</param>
        /// <param name="key">Ключ, значение которого нужно извлечь.</param>
        /// <returns>Значение по ключу или null, если ключ не найден.</returns>
        private static string? ExtractValue(string json, string key)
        {
            int keyIndex = json.IndexOf($"\"{key}\":");
            if (keyIndex >= 0)
            {
                int startIndex = json.IndexOf(':', keyIndex + key.Length + 2) + 1;
                while (startIndex < json.Length && char.IsWhiteSpace(json[startIndex]))
                {
                    startIndex++;
                }

                if (startIndex >= json.Length)
                {
                    return null;
                }

                // Если значение в кавычках (строка)
                if (json[startIndex] == '"')
                {
                    startIndex++;
                    int endIndex = json.IndexOf('"', startIndex);
                    return json.Substring(startIndex, endIndex - startIndex);
                }
                // Если значение не в кавычках (число, булево значение и т.д.)
                else
                {
                    int endIndex = startIndex;
                    while (endIndex < json.Length && !char.IsWhiteSpace(json[endIndex]) && json[endIndex] != ',' && json[endIndex] != '}')
                    {
                        endIndex++;
                    }
                    return json.Substring(startIndex, endIndex - startIndex);
                }
            }
            return null;
        }
    }
}
