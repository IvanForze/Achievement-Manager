using JSONLibrary;
using Project3_1;
using System.Text;
// Котов Иван БПИ246-1 Вариант 10.

/// <summary>
/// Главный класс программы, реализующий логику работы с достижениями и категориями.
/// </summary>
public class Program
{
    /// <summary>
    /// Список достижений.
    /// </summary>
    List<Achievement> achievements = new List<Achievement>();

    /// <summary>
    /// Список категорий.
    /// </summary>
    List<Category> categories = new List<Category>();

    /// <summary>
    /// Ширина карточки для вывода.
    /// </summary>
    int cardWidth = 40;

    /// <summary>
    /// Точка входа в программу.
    /// </summary>
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var program = new Program();
        program.Run();
    }

    /// <summary>
    /// Запускает цикл работы программы, предоставляя меню для выбора действий.
    /// </summary>
    void Run()
    {
        bool isRunning = true;

        while (isRunning)
        {
            Console.Clear(); // Очистка консоли перед выводом меню
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Ввести данные (консоль/файл)");
            Console.WriteLine("2. Отфильтровать данные");
            Console.WriteLine("3. Отсортировать данные");
            Console.WriteLine("4. Мои достижения");
            Console.WriteLine("5. Дополнительная задача");
            Console.WriteLine("6. Вывести данные (консоль/файл)");
            Console.WriteLine("7. Выход");
            Console.Write("Выберите пункт меню: ");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    InputData();
                    break;
                case "2":
                    FilterData();
                    break;
                case "3":
                    SortData();
                    break;
                case "4":
                    MainTask();
                    break;
                case "5":
                    AdditionalTask().GetAwaiter().GetResult();
                    break;
                case "6":
                    OutputData();
                    break;
                case "7":
                    isRunning = false;
                    Console.WriteLine("Выход из программы...");
                    break;
                default:
                    Console.WriteLine("Неверный ввод. Пожалуйста, выберите пункт от 1 до 7.");
                    break;
            }

            if (isRunning)
            {
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }
    }

    /// <summary>
    /// Обрабатывает ввод данных пользователем. Данные могут быть введены через консоль или загружены из файла.
    /// </summary>
    void InputData()
    {
        Console.Clear();
        Console.WriteLine("Выберите источник данных:");
        Console.WriteLine("1. Консоль");
        Console.WriteLine("2. Файл");
        Console.Write("Ваш выбор: ");
        string? choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.WriteLine("Введите данные в формате JSON (введите 'Ctrl+Z' для завершения):");
            // Чтение JSON из стандартного ввода (поток)
            using (var inputStream = Console.OpenStandardInput())
            using (var reader = new StreamReader(inputStream))
            {
                string json = reader.ReadToEnd();
                if (string.IsNullOrEmpty(json))
                {
                    Console.WriteLine("Ошибка: JSON не был введен.");
                    return;
                }

                var data = JsonParser.ReadJsonFromText(json);
                categories = data.Item1;
                achievements = data.Item2;

                PrintAchievements();
                CalculateCardWidth(data.Item2);
            }
        }
        else if (choice == "2")
        {
            Console.Write("Введите путь к файлу: ");
            string filePath = Console.ReadLine();
            var data = JsonParser.ReadJsonFromFile(filePath);
            categories = data.Item1;
            achievements = data.Item2;

            PrintAchievements();
            CalculateCardWidth(data.Item2);
        }
        else
        {
            Console.WriteLine("Неверный выбор.");
        }
    }

    /// <summary>
    /// Фильтрует достижения по выбранному полю и параметру.
    /// </summary>
    void FilterData()
    {
        Console.Clear();
        Console.WriteLine("Вы выбрали: Отфильтровать данные");
        if (achievements.Count == 0 || categories.Count == 0)
        {
            Console.WriteLine("Нет достижений");
            return;
        }

        Console.WriteLine("Выберите поле для фильтрации:");
        Console.WriteLine("1. Id");
        Console.WriteLine("2. Label");
        Console.WriteLine("3. Category");
        Console.WriteLine("4. Description");
        Console.Write("Ваш выбор: ");

        var menu_choice = 0;
        if (!Int32.TryParse(Console.ReadLine(), out menu_choice) || menu_choice > 4 || menu_choice < 1)
        {
            Console.WriteLine("Неверный ввод. Пожалуйста, выберите пункт от 1 до 4.");
            return;
        }

        Console.WriteLine("Введите параметр фильтрации (массив параметров, например: {\"A_CATEGORY_CS\", \"A_CATEGORY_EVERAFTER\"}):");
        var filter_choice = Console.ReadLine();
        if ((string.IsNullOrWhiteSpace(filter_choice)))
        {
            Console.Write("Введите корретные данные для фильтрации: ");
            return; 
        }

        switch (menu_choice) // Фильтруем определенное поле.
        {
            case 1:
                FilterAchievements("Id", filter_choice);
                break;
            case 2:
                FilterAchievements("Label", filter_choice);
                break;
            case 3:
                FilterAchievements("Category", filter_choice);
                break;
            case 4:
                FilterAchievements("Description", filter_choice);
                break;
        }
    }

    /// <summary>
    /// Фильтрует достижения по указанному полю.
    /// </summary>
    /// <param name="filter_id">Поле для фильтрации (например, Id, Label, Category, Description)</param>
    /// <param name="filter">Строка фильтра, представляющая массив значений для фильтрации.</param>
    void FilterAchievements(string filter_id, string filter)
    {
        var filterValues = filter.Trim('{', '}').Split(',');

        for (int i = 0; i < filterValues.Length; i++)
        {
            filterValues[i] = filterValues[i].Trim().Trim('\"', '\"');
        }
        // Создаем массив с отфильтрованными объектами.
        var temp_achievements = achievements.Where(a => filterValues.Contains(a.GetField(filter_id))).ToList();
        
        if (temp_achievements.Count == 0) // Если массив пустой, значит нет таких значений для фильтрации.
        {
            Console.WriteLine("Нет такого поля для фильтрации");
        }
        else
        {
            achievements = temp_achievements;
            PrintAchievements(); // Выводим достижения на экран.
        }
    }

    /// <summary>
    /// Сортирует достижения по выбранному полю и в выбранном порядке.
    /// </summary>
    void SortData()
    {
        Console.Clear();
        if (achievements.Count == 0 || categories.Count == 0)
        {
            Console.WriteLine("Нет достижений");
            return;
        }
        Console.WriteLine("Выберите параметр для сортировки:");
        Console.WriteLine("1. Id");
        Console.WriteLine("2. Label");
        Console.WriteLine("3. Category");
        Console.WriteLine("4. Description");
        Console.Write("Ваш выбор: ");

        var menu_choice = 0;
        var order_choice = 0;
        if (!Int32.TryParse(Console.ReadLine(), out menu_choice) || menu_choice > 4 || menu_choice < 1)
        {
            Console.WriteLine("Неверный ввод. Пожалуйста, выберите пункт от 1 до 4.");
            return;
        }

        Console.WriteLine("Выберите порядок сортировки:");
        Console.WriteLine("1. По возрастанию");
        Console.WriteLine("2. По убыванию");
        Console.Write("Ваш выбор: ");

        if (!Int32.TryParse(Console.ReadLine(), out order_choice) || order_choice > 2 || order_choice < 1)
        {
            Console.WriteLine("Неверный ввод. Пожалуйста, выберите пункт от 1 до 2.");
            return;
        }

        switch (menu_choice) // Сортируем выбранное поле в выбранном порядке.
        {
            case 1:
                SortAchievements("Id", order_choice);          
                break;
            case 2:
                SortAchievements("Label", order_choice);
                break;
            case 3:
                SortAchievements("Category", order_choice);
                break;
            case 4:
                SortAchievements("Description", order_choice);
                break;
        }
        PrintAchievements(); // Выводим достижения пользователю.
    }

    /// <summary>
    /// Сортирует достижения по указанному полю.
    /// </summary>
    /// <param name="sort_parameter">Поле, по которому будет выполнена сортировка (например, Id, Label)</param>
    /// <param name="order">Порядок сортировки (1 - по возрастанию, 2 - по убыванию)</param>
    void SortAchievements(string sort_parameter, int order)
    {
        if (order == 1)
        {
            // Сортируем по возрастанию список достижений.
            achievements = achievements.OrderBy(a => a.GetField(sort_parameter)).ToList();
        }
        else
        {
            // Сортируем по убыванию список достижений.
            achievements = achievements.OrderByDescending(a => a.GetField(sort_parameter)).ToList();
        }
    }

    /// <summary>
    /// Основная задача программы — отображение меню с достижениями и категориями.
    /// </summary>
    void MainTask()
    {
        if (achievements.Count == 0 || categories.Count == 0) // Возвращаемся в главное меню если нет достижений или категорий.
        {
            Console.WriteLine("Нет достижений");
            return;
        }

        ConsoleKeyInfo key; // Получение ввода пользователя
        int menu_index = 0;

        do
        {
            Console.Clear();

            Console.WriteLine("Вы выбрали: Меню достижений (нажмите 'Escape' для выхода в меню)");
            if (achievements.Count == 0 || categories.Count == 0) // Возвращаемся в главное меню если нет достижений или категорий.
            {
                Console.WriteLine("Нет достижений");
                break;
            }
            Console.WriteLine($"{menu_index + 1}/{ categories.Count} категория");
            Console.WriteLine();

            // Отображение категорий и достижений
            if (menu_index == 0) // Если начало меню, показываем первые три карточки.
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i >= 0 && i < categories.Count)
                    {
                        if (i == menu_index) // Рисуем тень под у выбранной карточки.
                        {
                            DrawCard(categories[i].GetField("Label"), true, categories[i].isExpanded, achievements.Where(a => a.GetField("Category") == categories[i].GetField("Id")).ToList());
                        }
                        else
                        {
                            DrawCard(categories[i].GetField("Label"), false, categories[i].isExpanded, achievements.Where(a => a.GetField("Category") == categories[i].GetField("Id")).ToList());
                        }
                    }
                }
                if (categories.Count > 3) { Console.WriteLine("<...>"); }                
            }
            else if (menu_index >= categories.Count - 3) // Если середина меню, показываем выбранную карточку и одну перед ней и одну после.
            {
                Console.WriteLine("<...>");
                for (int i = categories.Count - 3; i < categories.Count; i++)
                {
                    if (i >= 0 && i < categories.Count)
                    {
                        if (i == menu_index) // Рисуем тень под у выбранной карточки.
                        {
                            DrawCard(categories[i].GetField("Label"), true, categories[i].isExpanded, achievements.Where(a => a.GetField("Category") == categories[i].GetField("Id")).ToList());
                        }
                        else
                        {
                            DrawCard(categories[i].GetField("Label"), false, categories[i].isExpanded, achievements.Where(a => a.GetField("Category") == categories[i].GetField("Id")).ToList());
                        }
                    }
                }

            }
            else // Если конец меню, показываем последние три карточки.
            {
                for (int i = menu_index - 1; i < (menu_index + 2); i++)
                {
                    if (i >= 0 && i < categories.Count)
                    {
                        if (i > 0 && i < menu_index) { Console.WriteLine("<...>"); }
                        if (i == menu_index) // Рисуем тень под у выбранной карточки.
                        {
                            DrawCard(categories[i].GetField("Label"), true, categories[i].isExpanded, achievements.Where(a => a.GetField("Category") == categories[i].GetField("Id")).ToList());
                        }
                        else
                        {
                            DrawCard(categories[i].GetField("Label"), false, categories[i].isExpanded, achievements.Where(a => a.GetField("Category") == categories[i].GetField("Id")).ToList());
                        }
                        if ((i + 1) < categories.Count && i > menu_index) { Console.WriteLine("<...>"); }
                    }
                }
            }
            key = Console.ReadKey();

            // При нажатии на Enter раскрываем или закрываем карточку.
            if (key.Key == ConsoleKey.Enter) { categories[menu_index].isExpanded = !categories[menu_index].isExpanded; }

            // При нажатии на стрелочку вверх перемещаемся вверх по меню.
            if (key.Key == ConsoleKey.UpArrow && menu_index > 0) { menu_index--; }

            // При нажатии на стрелочку вниз перемещаемся вниз по меню.
            if (key.Key == ConsoleKey.DownArrow && menu_index < (categories.Count - 1)) { menu_index++; }

        } while (key.Key != ConsoleKey.Escape); // При нажатии на Escape выходим в главное меню.
    }

    /// <summary>
    /// Дополнительная задача: загрузка и вывод достижений игры "Cultist Simulator" с использованием Steam Web API.
    /// </summary>
    /// <returns>Задача, представляющая асинхронную операцию получения достижений.</returns>
    async Task AdditionalTask()
    {
        Console.Clear();
        Console.WriteLine("Вы выбрали: Дополнительная задача: данные о достижениях игры Cultist Simulator");

        int appId = 718670; // ID игры Cultist Simulator

        // Получаем данные о достижениях
        var steam_achievements = await SteamWebAPI.GetAchievementPercentagesAsync(appId);

        if (steam_achievements.Count > 0) // Если есть достижения, выводим их в консоль.
        {
            Console.WriteLine("Достижения и их процент выполнения:");
            foreach (var achievement in steam_achievements)
            {
                Console.WriteLine($"{achievement.Name}: {achievement.Percent.ToString()}%");
            }
        }
        else
        {
            Console.WriteLine("Не удалось получить данные о достижениях.");
        }
    }

    /// <summary>
    /// Метод для вывода данных о достижениях на консоль или в файл.
    /// </summary>
    void OutputData()
    {
        Console.Clear();
        
        if (achievements.Count == 0 || categories.Count == 0) // Возвращаемся в главное меню если нет достижений или категорий.
        {
            Console.WriteLine("Нет достижений");
            return;
        }
        Console.WriteLine("Вы выбрали: Вывести данные (консоль/файл)");
        Console.WriteLine("1. Консоль");
        Console.WriteLine("2. Файл");
        Console.Write("Ваш выбор: ");

        var menu_choice = 0;
        var order_choice = 0;

        string filePath = "";
        if (!Int32.TryParse(Console.ReadLine(), out menu_choice) || menu_choice > 2 || menu_choice < 1)
        {
            Console.WriteLine("Неверный ввод. Пожалуйста, выберите пункт от 1 до 4.");
            return;
        }

        if (menu_choice == 2)
        {
            Console.Write("Введите путь для сохранения:");
            filePath = Console.ReadLine();
            if ((string.IsNullOrWhiteSpace(filePath)))
            {
                Console.Write("Введите корретный путь для сохранения");
                return;
            }
        }

        string output_json;
        output_json = CreateOutputJson(); // Создаем json для вывода.

        if (menu_choice == 1)
        {
            JsonParser.WriteJson(output_json);
        }
        else
        {
            JsonParser.WriteJson(output_json, filePath);
        }
    }

    /// <summary>
    /// Создает строку JSON, представляющую данные о достижениях и категориях.
    /// </summary>
    /// <returns>Строка, содержащая данные в формате JSON.</returns>
    string CreateOutputJson()
    {
        string output_json = "{\"achievements\": [\n";
        bool temp_bool;
        for (int i = 0; i < categories.Count; i++)
        {
            output_json += "  {\n";
            foreach (string key in categories[i].GetAllFields())
            {
                if (bool.TryParse(categories[i].GetField(key), out temp_bool))
                {
                    if (key == "iscategory")
                    {
                        output_json += $"    \"isCategory\": {temp_bool.ToString().ToLower()}";
                    }
                    else
                    {
                        output_json += $"    \"{key}\": {temp_bool.ToString().ToLower()}";
                    }
                }
                else
                {
                    output_json += $"    \"{key}\": \"{categories[i].GetField(key)}\"";
                }
                if (key != categories[i].GetAllFields().Last())
                {
                    output_json += ",\n";
                }
                else
                {
                    output_json += "\n  }";
                }
            }
            output_json += ",\n";
        }
        for (int i = 0; i < achievements.Count; i++)
        {
            output_json += "  {\n";
            
            foreach (var key in achievements[i].GetAllFields())
            {
                if (bool.TryParse(achievements[i].GetField(key), out temp_bool))
                {
                    output_json += $"    \"{key}\": {temp_bool.ToString().ToLower()}";
                }
                else
                {
                    output_json += $"    \"{key}\": \"{achievements[i].GetField(key)}\"";
                }
                
                if (key != achievements[i].GetAllFields().Last()) 
                { 
                    output_json += ",\n";
                }
                else
                {
                    output_json += "\n  }";
                }
            }
            if (i != achievements.Count - 1) 
            { 
                output_json += ",\n"; 
            }
            
        }
        output_json += "\n]}";
        return output_json;
    }

    /// <summary>
    /// Рисует карточку с достижением или категорией, отображая информацию о достижениях, если они присутствуют.
    /// </summary>
    /// <param name="title">Название карточки (категории или достижения).</param>
    /// <param name="isSelected">Флаг, указывающий, выбрана ли эта карточка в меню.</param>
    /// <param name="isExpanded">Флаг, указывающий, развернута ли карточка для отображения достижений.</param>
    /// <param name="achievements">Список достижений, относящихся к данной категории. По-дефолту null.</param>
    void DrawCard(string title, bool isSelected, bool isExpanded, List<Achievement>? achievements = null)
    {
        string border = new string('─', cardWidth - 2);

        if (isSelected) // Рисуем карточку с тенью.
        {
            Console.WriteLine($"┌{border}┐");
            Console.WriteLine($"│ {title.PadRight(cardWidth - 4)} │▒");
            if (isExpanded && achievements.Count != 0) //  Рисуем раскрытую карточку.
            {
                Console.WriteLine($"├{border}┤▒");
                foreach (var achievement in achievements)
                {
                    Console.WriteLine($"│ - {achievement.GetField("Label").PadRight(cardWidth - 6)} │▒");

                }
            }
            Console.WriteLine($"└{border}┘▒");
            Console.WriteLine(" " + new string('▒', cardWidth));
        }
        else // Рисуем карточку без тени.
        {
            Console.WriteLine($"┌{border}┐");
            Console.WriteLine($"│ {title.PadRight(cardWidth - 4)} │");
            if (isExpanded && achievements.Count != 0) //  Рисуем раскрытую карточку.
            {
                Console.WriteLine($"├{border}┤");
                foreach (var achievement in achievements)
                {
                    Console.WriteLine($"│ - {achievement.GetField("Label").PadRight(cardWidth - 6)} │");

                }
            }
            Console.WriteLine($"└{border}┘");
        }
    }

    /// <summary>
    /// Выводит все достижения, сгруппированные по категориям.
    /// </summary>
    void PrintAchievements()
    {
        foreach (var category in categories)
        {
            Console.WriteLine($"Категория: {category.GetField("Label")}");
            foreach (var achievement in achievements)
            {
                if (achievement.GetField("Category") == category.GetField("Id"))
                {
                    Console.WriteLine($"  Достижение: {achievement}");
                }                
            }
        }
    }

    /// <summary>
    /// Рассчитывает необходимую ширину карточки для отображения всех достижений и категорий.
    /// </summary>
    /// <param name="achievements">Список достижений, для которых будет рассчитываться максимальная ширина.</param>
    void CalculateCardWidth(List<Achievement> achievements)
    {
        // Рассчитываем ширину карточки для достижений
        foreach (var achievement in achievements)
        {
            cardWidth = Math.Max(cardWidth, achievement.GetField("Label").Length + 6);
        }

        // Рассчитываем ширину карточки для категорий.
        foreach (var category in categories)
        {
            cardWidth = Math.Max(cardWidth, category.GetField("Label").Length + 4);
        }
    }
}
