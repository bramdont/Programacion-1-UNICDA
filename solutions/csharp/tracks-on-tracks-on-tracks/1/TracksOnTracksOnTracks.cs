public static class Languages
{
    private const string Favorite = "C#";

    public static List<string> NewList() => new();

    public static List<string> GetExistingLanguages() => new() { "C#", "Clojure", "Elm" };

    public static List<string> AddLanguage(List<string> languages, string language)
    {
        languages.Add(language);
        return languages;
    }

    public static int CountLanguages(List<string> languages) => languages.Count;

    public static bool HasLanguage(List<string> languages, string language) =>
        languages.Contains(language);

    public static List<string> ReverseList(List<string> languages)
    {
        languages.Reverse();
        return languages;
    }

    public static bool IsExciting(List<string> languages) =>
        languages.Count switch
        {
            0 => false,
            1 => languages[0] == Favorite,
            2 or 3 => languages[0] == Favorite || languages[1] == Favorite,
            _ => languages[0] == Favorite
        };

    public static List<string> RemoveLanguage(List<string> languages, string language)
    {
        languages.Remove(language);
        return languages;
    }

    public static bool IsUnique(List<string> languages) =>
        new HashSet<string>(languages).Count == languages.Count;
}
