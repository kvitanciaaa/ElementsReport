#nullable enable

namespace dLab.General.ElementsReport.Lang;

using System.Globalization;
using System.Reflection;
using System.Resources;

/// <summary>
/// Строки локализации модуля.
/// Значения берутся из файлов ресурсов в папке <c>Lang</c> по текущей культуре интерфейса.
/// </summary>
public static class ElementsReport
{
    /// <summary>
    /// Менеджер ресурсов модуля.
    /// </summary>
    private static readonly ResourceManager ResourceManager = new ResourceManager(
        "dLab.General.ElementsReport.Lang.ElementsReport",
        Assembly.GetExecutingAssembly());

    /// <summary>
    /// Заголовок главного окна.
    /// </summary>
    public static string WindowTitle => GetString(nameof(WindowTitle));

    /// <summary>
    /// Заголовок над таблицей ведомости.
    /// </summary>
    public static string ElementsHeading => GetString(nameof(ElementsHeading));

    /// <summary>
    /// Заголовок колонки с именем типа элемента.
    /// </summary>
    public static string TypeColumn => GetString(nameof(TypeColumn));

    /// <summary>
    /// Заголовок колонки с площадью.
    /// </summary>
    public static string AreaColumn => GetString(nameof(AreaColumn));

    /// <summary>
    /// Подпись перед суммарной площадью.
    /// </summary>
    public static string TotalLabel => GetString(nameof(TotalLabel));

    /// <summary>
    /// Категория стен.
    /// </summary>
    public static string CategoryWalls => GetString(nameof(CategoryWalls));

    /// <summary>
    /// Категория перекрытий.
    /// </summary>
    public static string CategoryFloors => GetString(nameof(CategoryFloors));

    /// <summary>
    /// Категория крыш.
    /// </summary>
    public static string CategoryRoofs => GetString(nameof(CategoryRoofs));

    /// <summary>
    /// Заголовок колонки с уровнем.
    /// </summary>
    public static string LevelColumn => GetString(nameof(LevelColumn));

    /// <summary>
    /// Текст для элемента без указанного уровня.
    /// </summary>
    public static string LevelNotSpecified => GetString(nameof(LevelNotSpecified));

    /// <summary>
    /// Подпись поля минимальной площади.
    /// </summary>
    public static string MinAreaLabel => GetString(nameof(MinAreaLabel));

    /// <summary>
    /// Обозначение квадратных метров.
    /// </summary>

    public static string SquareMetersSuffix => GetString(nameof(SquareMetersSuffix));

    /// <summary>
    /// Сообщение о том, что команда запущена без открытой модели.
    /// </summary>
    public static string NoActiveDocumentMessage => GetString(nameof(NoActiveDocumentMessage));

    /// <summary>
    /// Возвращает строку ресурса по ключу для текущей культуры интерфейса.
    /// </summary>
    /// <param name="key">Ключ ресурса.</param>
    /// <returns>Строка ресурса или пустая строка, если ключ не найден.</returns>
    private static string GetString(string key) =>
        ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? string.Empty;
}
