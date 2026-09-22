static class Badge
{
    private const string OwnerDepartment = "OWNER";

    public static string Print(int? id, string name, string? department)
    {
        var idPrefix = id == null ? "" : $"[{id}] - ";
        var departmentLabel = department?.ToUpperInvariant() ?? OwnerDepartment;

        return $"{idPrefix}{name} - {departmentLabel}";
    }
}
