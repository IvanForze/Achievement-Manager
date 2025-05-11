namespace JSONLibrary
{
    /// <summary>
    /// Интерфейс, представляющий объект JSON.
    /// </summary>
    public interface IJSONObject
    {
        /// <summary>
        /// Возвращает все поля JSON объекта.
        /// </summary>
        /// <returns>Перечисление всех полей JSON объекта.</returns>
        IEnumerable<string> GetAllFields();
        /// <summary>
        /// Получает значение поля по его имени.
        /// </summary>
        /// <param name="fieldName">Имя поля, значение которого нужно получить.</param>
        /// <returns>Значение поля как строка.</returns>
        string GetField(string fieldName);
        /// <summary>
        /// Устанавливает значение поля JSON объекта.
        /// </summary>
        /// <param name="fieldName">Имя поля, которое нужно установить.</param>
        /// <param name="value">Значение для поля.</param>
        void SetField(string fieldName, string value);
    }
}
