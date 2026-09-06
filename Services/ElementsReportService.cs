#nullable enable

namespace dLab.General.ElementsReport.Services;

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using dLab.General.ElementsReport.Abstractions;
using dLab.General.ElementsReport.Helpers;
using dLab.General.ElementsReport.Models;

/// <inheritdoc />
public class ElementsReportService : IElementsReportService
{

    /// <summary>
    /// Документ, по которому строится ведомость.
    /// </summary>
    private readonly Document _document;

    /// <summary>
    /// Creates the report building service.
    /// </summary>
    /// <param name="document">Документ открытой модели.</param>
    public ElementsReportService(Document document)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
    }

    /// <inheritdoc />
    public ElementsReportResult BuildReport(ReportCategory category)
    {
        var rows = new List<ElementRow>();
        double totalArea = 0;

        foreach (var element in CollectElements(category))
        {
            var area = GetArea(element);
            if (area <= 0)
                continue;

            var areaSquareMeters = UnitHelper.ToSquareMeters(area);
            var levelName = GetLevelName(element);

            rows.Add(new ElementRow(element.Name, levelName, areaSquareMeters));
            totalArea += areaSquareMeters;
        }

        return new ElementsReportResult(rows, totalArea);
    }

    /// <summary>
    /// Возвращает элементы модели, попадающие в ведомость.
    /// </summary>
    /// <returns>Экземпляры элементов заданной категории.</returns>
    private IEnumerable<Element> CollectElements(ReportCategory category)
    {
        var revitCategory = category switch
        {
            ReportCategory.Walls => BuiltInCategory.OST_Walls,
            ReportCategory.Floors => BuiltInCategory.OST_Floors,
            ReportCategory.Roofs => BuiltInCategory.OST_Roofs,
            _ => throw new ArgumentOutOfRangeException(nameof(category))
        };

        return new FilteredElementCollector(_document)
            .OfCategory(revitCategory)
            .WhereElementIsNotElementType()
            .ToElements();
    }

    /// <summary>
    /// Возвращает имя уровня, к которому относится элемент.
    /// </summary>
    /// <param name="element">Элемент модели.</param>
    /// <returns>Имя уровня или локализованный текст, если уровень определить невозможно.</returns>
    private string GetLevelName(Element element)
    {
        var levelId = element.LevelId;

        if (levelId != ElementId.InvalidElementId)
        {
            var level = _document.GetElement(levelId) as Level;

            if (level is not null)
                return level.Name;
        }

        return Lang.ElementsReport.LevelNotSpecified;
    }

    /// <summary>
    /// Возвращает площадь элемента во внутренних единицах Revit.
    /// </summary>
    /// <param name="element">Элемент модели.</param>
    /// <returns>Площадь во внутренних единицах или ноль, если параметр не заполнен.</returns>
    private double GetArea(Element element)
    {
        var parameter = element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
        if (parameter is null || !parameter.HasValue)
            return 0;

        return parameter.AsDouble();
    }
}
