#nullable enable

namespace dLab.General.ElementsReport.Models;

/// <summary>
/// Строка ведомости: один элемент модели, его уровень и площадь.
/// </summary>
public class ElementRow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElementRow"/> class.
    /// </summary>
    /// <param name="name">Имя типа элемента.</param>
    /// <param name="levelName">Имя уровня элемента.</param>
    /// <param name="areaSqM">Площадь элемента в квадратных метрах.</param>
    public ElementRow(string name, string levelName, double areaSqM)
    {
        Name = name;
        LevelName = levelName;
        AreaSqM = areaSqM;
    }

    /// <summary>
    /// Имя типа элемента.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Имя уровня элемента.
    /// </summary>
    public string LevelName { get; }

    /// <summary>
    /// Площадь элемента в квадратных метрах.
    /// </summary>
    public double AreaSqM { get; }
}