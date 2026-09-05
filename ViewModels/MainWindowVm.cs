#nullable enable

namespace dLab.General.ElementsReport.ViewModels;

using Autodesk.Revit.DB;
using dLab.General.ElementsReport.Abstractions;
using dLab.General.ElementsReport.Commands;
using dLab.General.ElementsReport.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LangResources = dLab.General.ElementsReport.Lang.ElementsReport;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Модель представления главного окна: строки ведомости, итог и сообщение об ошибке.
/// О Revit не знает, данные получает от <see cref="IElementsReportService"/>.
/// </summary>
public class MainWindowVm : ObservableBase
{
    /// <summary>
    /// Сервис построения ведомости.
    /// </summary>
    private readonly IElementsReportService _reportService;

    /// <summary>
    /// Команда загрузки данных, вызывается при открытии окна.
    /// </summary>
    private ICommand? _initializeCommand;

    private ReportCategoryOption _selectedCategory;

    private bool _isInitialized;

    private readonly List<ElementRow> _allRows = new List<ElementRow>();
    private string _minAreaText = string.Empty;

    /// <summary>
    /// Creates the view model of the main window.
    /// </summary>
    /// <param name="reportService">Сервис построения ведомости.</param>
    public MainWindowVm(IElementsReportService reportService)
    {
        _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));

        Rows = new ObservableCollection<ElementRow>();

        Categories = new ObservableCollection<ReportCategoryOption>
    {
        new ReportCategoryOption(ReportCategory.Walls, LangResources.CategoryWalls),
        new ReportCategoryOption(ReportCategory.Floors, LangResources.CategoryFloors),
        new ReportCategoryOption(ReportCategory.Roofs, LangResources.CategoryRoofs)
    };

        _selectedCategory = Categories[0];
    }

    /// <summary>
    /// Строки ведомости.
    /// </summary>
    public ObservableCollection<ElementRow> Rows { get; }

    /// <summary>
    /// Доступные категории элементов.
    /// </summary>
    public ObservableCollection<ReportCategoryOption> Categories { get; }

    /// <summary>
    /// Выбранная категория элементов.
    /// </summary>
    public ReportCategoryOption SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (ReferenceEquals(_selectedCategory, value))
                return;

            _selectedCategory = value;
            OnPropertyChanged();

            if (_isInitialized)
                Load();
        }
    }

    /// <summary>
    /// Минимальная площадь для фильтрации.
    /// </summary>
    public string MinAreaText
    {
        get => _minAreaText;
        set
        {
            if (_minAreaText == value)
                return;

            _minAreaText = value;
            OnPropertyChanged();

            if (_isInitialized)
                ApplyFilter();
        }
    }

    /// <summary>
    /// Команда загрузки данных, привязана к открытию окна.
    /// </summary>
    public ICommand InitializeCommand =>
        _initializeCommand ??= new RelayCommand(Load, onError: HandleError);

    /// <summary>
    /// Суммарная площадь по строкам ведомости в квадратных метрах.
    /// </summary>
    public double TotalArea { get; private set; }

    /// <summary>
    /// Текст сообщения об ошибке. Пустая строка означает, что ошибки нет.
    /// </summary>
    public string ErrorMessage { get; private set; } = string.Empty;

    /// <summary>
    /// Загружает ведомость и обновляет содержимое окна.
    /// </summary>
    private void Load()
    {
        ErrorMessage = string.Empty;

        var report = _reportService.BuildReport(SelectedCategory.Category);

        _allRows.Clear();
        _allRows.AddRange(report.Rows);

        _isInitialized = true;

        ApplyFilter();
    }

    

    /// <summary>
    /// Фильтрует элементы по минимальной площади и пересчитывает итоговую площадь.
    /// </summary>
    private void ApplyFilter()
    {
        Rows.Clear();

        // Если фильтр не задан или введено некорректное значение,
        // минимальная площадь считается равной нулю.
        var minArea = 0.0;

        if (!string.IsNullOrWhiteSpace(MinAreaText))
        {
            var text = MinAreaText.Trim();

            // Сначала пытаемся распознать число с учетом текущих
            // региональных настроек Windows, например "12,5".
            if (!double.TryParse(
                    text,
                    NumberStyles.Float,
                    CultureInfo.CurrentCulture,
                    out minArea))
            {
                // Дополнительно поддерживаем ввод с точкой,
                // например "12.5", даже если текущая культура использует запятую.
                var normalizedText = text.Replace(',', '.');

                if (!double.TryParse(
                        normalizedText,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out minArea))
                {
                    // Некорректный ввод не должен приводить к ошибке приложения.
                    // В этом случае показываем все элементы.
                    minArea = 0.0;
                }
            }
        }

        var totalArea = 0.0;

        foreach (var row in _allRows)
        {
            // Не отображаем элементы, площадь которых меньше заданного значения.
            if (row.AreaSqM < minArea)
                continue;

            Rows.Add(row);

            // Итоговая площадь считается только по отображаемым элементам.
            totalArea += row.AreaSqM;
        }

        TotalArea = totalArea;
    }

    /// <summary>
    /// Показывает пользователю сообщение об ошибке, возникшей в команде.
    /// </summary>
    /// <param name="exception">Возникшее исключение.</param>
    private void HandleError(Exception exception) => ErrorMessage = exception.Message;
}
