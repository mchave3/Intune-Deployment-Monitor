using Microsoft.Windows.ApplicationModel.Resources;

namespace Intune_Group_Assignments.Helpers;

public static class ResourceExtensions
{
    private static readonly ResourceLoader _resourceLoader = new();

    public static string GetLocalized(this string resourceKey) => _resourceLoader.GetString(resourceKey);
}
