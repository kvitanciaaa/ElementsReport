#nullable enable

namespace dLab.General.ElementsReport.Models;

public class ReportCategoryOption
{
    public ReportCategoryOption(ReportCategory category, string displayName)
    {
        Category = category;
        DisplayName = displayName;
    }

    public ReportCategory Category { get; }

    public string DisplayName { get; }
}