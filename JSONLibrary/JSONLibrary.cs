using System.Text;

namespace JSONLibrary
{
    /// <summary>
    /// Структура, представляющая достижение с различными полями.
    /// Реализует интерфейс IJSONObject.
    /// </summary>
    public struct Achievement : IJSONObject
    {
        // Словарь для хранения полей объекта
        private Dictionary<string, string> Fields = new Dictionary<string, string>();
        /// <summary>
        /// Конструктор структуры Achievement. Инициализирует поля объекта с дефолтными значениями.
        /// </summary>
        public Achievement()
        {
            SetField("Category", "noCategory");
            SetField("Label", "");
            SetField("Id", "");
        }

        /// <summary>
        /// Возвращает имена всех полей объекта.
        /// </summary>
        /// <returns>Перечисление ключей словаря, представляющих имена полей.</returns>
        public IEnumerable<string> GetAllFields()
        {
            return Fields.Keys;
        }

        /// <summary>
        /// Возвращает значение поля по его имени.
        /// </summary>
        /// <param name="fieldName">Имя поля, значение которого нужно получить.</param>
        /// <returns>Значение поля, если оно существует, иначе null.</returns>
        public string? GetField(string fieldName)
        {
            fieldName = fieldName.ToLower(); // Приводим имя поля к нижнему регистру.
            if (Fields.ContainsKey(fieldName))
            {
                return Fields[fieldName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Устанавливает значение поля по его имени.
        /// </summary>
        /// <param name="fieldName">Имя поля, которое нужно установить.</param>
        /// <param name="value">Новое значение поля.</param>
        public void SetField(string fieldName, string value)
        {
            fieldName = fieldName.ToLower(); // Приводим имя поля к нижнему регистру.
            Fields[fieldName] = value;
        }
        /// <summary>
        /// Переопределенный метод ToString, который возвращает строковое представление объекта Achievement.
        /// </summary>
        /// <returns>Строка с данными поля Id, Category, Label и DescriptionUnlocked.</returns>
        public override string ToString()
        {
            return $"Id: {GetField("Id")} | Category: {GetField("Category")} | Label: {GetField("Label")} | Desription: {GetField("DescriptionUnlocked") ?? "Нет описания"}";
        }
    }
    /// <summary>
    /// Класс, представляющий категорию с различными полями.
    /// Реализует интерфейс IJSONObject.
    /// </summary>
    public class Category : IJSONObject
    {
        // Словарь для хранения полей объекта
        private Dictionary<string, string> Fields = new Dictionary<string, string>();
        /// <summary>
        /// Свойство, указывающее, развернута ли категория.
        /// </summary>
        public bool isExpanded { get; set; }
        /// <summary>
        /// Конструктор класса Category. Инициализирует поля объекта с дефолтными значениями.
        /// </summary>
        public Category()
        {
            SetField("isCategory", "true");
            SetField("Label", "");
        }
        /// <summary>
        /// Возвращает имена всех полей объекта.
        /// </summary>
        /// <returns>Перечисление ключей словаря, представляющих имена полей.</returns>
        public IEnumerable<string> GetAllFields()
        {
            return Fields.Keys;
        }
        /// <summary>
        /// Возвращает значение поля по его имени.
        /// </summary>
        /// <param name="fieldName">Имя поля, значение которого нужно получить.</param>
        /// <returns>Значение поля, если оно существует, иначе null.</returns>
        public string GetField(string fieldName)
        {
            fieldName = fieldName.ToLower(); // Приводим имя поля к нижнему регистру.
            if (Fields.ContainsKey(fieldName))
            {
                return Fields[fieldName];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Устанавливает значение поля по его имени.
        /// </summary>
        /// <param name="fieldName">Имя поля, которое нужно установить.</param>
        /// <param name="value">Новое значение поля.</param>
        public void SetField(string fieldName, string value)
        {
            fieldName = fieldName.ToLower(); // Приводим имя поля к нижнему регистру.
            Fields[fieldName] = value;
        }
        /// <summary>
        /// Переопределенный метод ToString, который возвращает строковое представление объекта Category.
        /// </summary>
        /// <returns>Строка с данными поля Id и Label.</returns>
        public override string ToString()
        {
            return $"Категория id: {GetField("Id")} label: {GetField("Label")}";
        }
    }
    /// <summary>
    /// Статический класс для парсинга JSON данных, связанный с категориями и достижениями.
    /// </summary>
    public static class JsonParser
    {
        /// <summary>
        /// Читает JSON-строку и парсит её в списки категорий и достижений.
        /// </summary>
        /// <param name="json">Строка JSON для парсинга.</param>
        /// <returns>Кортеж, содержащий два списка: категорий и достижений.</returns>
        public static (List<Category>, List<Achievement>) ReadJsonFromText(string json)
        {
            try
            {
                return ParseJson(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                return (new List<Category>(), new List<Achievement>());
            }
        }
        /// <summary>
        /// Читает JSON-данные из файла и парсит их в списки категорий и достижений.
        /// </summary>
        /// <param name="filePath">Путь к файлу, содержащему JSON.</param>
        /// <returns>Кортеж, содержащий два списка: категорий и достижений.</returns>
        public static (List<Category>, List<Achievement>) ReadJsonFromFile(string filePath)
        {
            try
            {
                string json = "";
                using (StreamReader fileStream = new StreamReader(filePath))
                {

                    // Перенаправляем стандартный ввод на поток чтения из файла.
                    Console.SetIn(fileStream);

                    // Читаем строку за строкой, пока не достигнем конца файла.
                    string line;
                    while ((line = Console.ReadLine()) != null)
                    {
                        json += line;
                    }
                }

                // Восстанавливаем стандартный ввод.
                Console.SetIn(new StreamReader(Console.OpenStandardInput()));
                
                return ParseJson(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                return (new List<Category>(), new List<Achievement>());
            }
        }
        /// <summary>
        /// Парсит строку JSON в списки категорий и достижений.
        /// </summary>
        /// <param name="json">Строка JSON для парсинга.</param>
        /// <returns>Кортеж, содержащий два списка: категорий и достижений.</returns>
        private static (List<Category>, List<Achievement>) ParseJson(string json)
        {           
            var categories = new List<Category>(); // Список категорий.
            var achievements = new List<Achievement>(); // Список достижений.

            // Создаем категорию "Другие достижения"
            Category noCategory = new Category();
            noCategory.SetField("Id", "noCategory");
            noCategory.SetField("Label", "Другие достижения");
            categories.Add(noCategory);
                        
            int index = 0;

            // Ищем начало массива achievements
            index = json.IndexOf("\"achievements\":", index);
            if (index == -1) return (categories, achievements); // Возвращаем пустые списки, если некорректный json. 

            index = json.IndexOf('[', index);
            if (index == -1) return (categories, achievements); // Возвращаем пустые списки, если некорректный json. 

            // Парсим массив achievements
            while (index < json.Length)
            {
                index = json.IndexOf('{', index);
                if (index == -1) break;

                // Проверяем, является ли объект категорией
                if (json.IndexOf("\"isCategory\": true", index) < json.IndexOf('}', index) && json.IndexOf("\"isCategory\": true", index) != -1)
                {
                    var category = ParseCategory(json, ref index);
                    if (category != null && category.GetField("Id") != "noCategory")
                    {
                        categories.Add((Category)category);
                    }
                }
                else
                {
                    var achievement = ParseAchievement(json, ref index);
                    if (achievement != null)
                    {
                        achievements.Add((Achievement)achievement);
                    }
                }
            }

            return (categories, achievements);
        }

        /// <summary>
        /// Парсит объект достижения из JSON строки.
        /// </summary>
        /// <param name="json">Строка JSON для парсинга.</param>
        /// <param name="index">Индекс в строке JSON, с которого начинаем парсинг.</param>
        /// <returns>Объект достижения.</returns>
        private static Achievement? ParseAchievement(string json, ref int index)
        {
            var achievement = new Achievement();
            while (index < json.Length)
            {
                index = json.IndexOf('"', index) + 1;
                if (index == 0) break;

                string key = ReadJsonKey(json, ref index); // Парсим ключ.
                string? value = ReadJsonValue(json, ref index); // Парсим значение.
                index++;

                achievement.SetField(key, value); // Устанавливаем значение поля.

                if ((json.IndexOf('"', index) > json.IndexOf('}', index)) || (json.IndexOf('"', index) == -1))
                {
                    index = json.IndexOf('}', index) + 1;
                    break;
                }
            }

            return achievement;
        }
        /// <summary>
        /// Парсит объект категории из JSON строки.
        /// </summary>
        /// <param name="json">Строка JSON для парсинга.</param>
        /// <param name="index">Индекс в строке JSON, с которого начинаем парсинг.</param>
        /// <returns>Объект категории.</returns>
        private static Category? ParseCategory(string json, ref int index)
        {
            var category = new Category();
            while (index < json.Length)
            {
                index = json.IndexOf('"', index) + 1;
                if (index == 0) break;
                
                string key = ReadJsonKey(json, ref index); // Парсим ключ.
                string? value = ReadJsonValue(json, ref index); // Парсим значение.
                index++;

                category.SetField(key, value); // Устанавливаем значение поля.

                if ((json.IndexOf('"', index) > json.IndexOf('}', index)) || (json.IndexOf('"', index) == -1))
                {
                    index = json.IndexOf('}', index) + 1;
                    break;
                }
            }

            return category;
        }
        /// <summary>
        /// Читает ключ из строки JSON.
        /// </summary>
        /// <param name="json">Строка JSON для парсинга.</param>
        /// <param name="index">Индекс в строке JSON, с которого начинаем парсинг.</param>
        /// <returns>Ключ в виде строки.</returns>
        private static string ReadJsonKey(string json, ref int index)
        {
            int start = index;
            while (index < json.Length && json[index] != '"')
            {
                index++;
            }
            return json.Substring(start, index - start);
        }
        /// <summary>
        /// Читает значение из строки JSON.
        /// </summary>
        /// <param name="json">Строка JSON для парсинга.</param>
        /// <param name="index">Индекс в строке JSON, с которого начинаем парсинг.</param>
        /// <returns>Значение в виде строки или null, если это массив.</returns>
        private static string? ReadJsonValue(string json, ref int index)
        {
            index = json.IndexOf(':', index) + 1;
            while (index < json.Length && char.IsWhiteSpace(json[index]))
            {
                index++;
            }

            if (json[index] == '"') // Парсим значения в ковычках.
            {
                index++;
                int start = index;
                while (index < json.Length && json[index] != '"')
                {
                    index++;
                }
                return json.Substring(start, index - start);
            }
            else if (json[index] == '{' || json[index] == '[')
            {
                // Пропускаем вложенные объекты и массивы
                int depth = 1;
                index++;
                while (index < json.Length && depth > 0)
                {
                    if (json[index] == '{' || json[index] == '[') depth++;
                    if (json[index] == '}' || json[index] == ']') depth--;
                    index++;
                }
                return null;
            }
            else
            {
                // Парсим простые значения (числа, true/false, null)
                int start = index;
                while (index < json.Length && !char.IsWhiteSpace(json[index]) && json[index] != ',' && json[index] != '}')
                {
                    index++;
                }
                return json.Substring(start, index - start);
            }

        }
        /// <summary>
        /// Записывает JSON строку в файл или выводит в консоль.
        /// </summary>
        /// <param name="output_json">Строка JSON для записи.</param>
        /// <param name="filePath">Путь к файлу для сохранения (если не указан, выводит в консоль).</param>
        public static void WriteJson(string output_json, string filePath = null)
        {
            if (filePath == null)
            {
                Console.WriteLine(output_json);
            }
            else
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        Console.OutputEncoding = Encoding.UTF8;
                        Console.SetOut(writer);
                        Console.WriteLine(output_json);
                        // Восстановление стандартного потока вывода
                        StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
                        standardOutput.AutoFlush = true;
                        Console.SetOut(standardOutput);
                        

                        Console.WriteLine("Данные успешно сохранены в файл.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
                }
            }
        }
    }
}
