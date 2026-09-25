using System.Linq;

public static class DialingCodes
{
    public static Dictionary<int, string> GetEmptyDictionary()
    {
        return new Dictionary<int, string>();
    }

    public static Dictionary<int, string> GetExistingDictionary()
    {
        return new Dictionary<int, string>
        {
            { 1, "United States of America" },
            { 55, "Brazil" },
            { 91, "India" }
        };
    }

    public static Dictionary<int, string> AddCountryToEmptyDictionary(int countryCode, string countryName)
    {
        var dictionary = GetEmptyDictionary();
        dictionary.Add(countryCode, countryName);
        return dictionary;
    }

    public static Dictionary<int, string> AddCountryToExistingDictionary(
        Dictionary<int, string> existingDictionary, int countryCode, string countryName)
    {
        existingDictionary.Add(countryCode, countryName);
        return existingDictionary;
    }

    public static string GetCountryNameFromDictionary(
        Dictionary<int, string> existingDictionary, int countryCode)
    {
        return existingDictionary.TryGetValue(countryCode, out var countryName) ? countryName : string.Empty;
    }

    public static bool CheckCodeExists(Dictionary<int, string> existingDictionary, int countryCode)
    {
        return existingDictionary.ContainsKey(countryCode);
    }

    public static Dictionary<int, string> UpdateDictionary(
        Dictionary<int, string> existingDictionary, int countryCode, string countryName)
    {
        if (existingDictionary.ContainsKey(countryCode))
        {
            existingDictionary[countryCode] = countryName;
        }
        return existingDictionary;
    }

    public static Dictionary<int, string> RemoveCountryFromDictionary(
        Dictionary<int, string> existingDictionary, int countryCode)
    {
        existingDictionary.Remove(countryCode);
        return existingDictionary;
    }

    public static string FindLongestCountryName(Dictionary<int, string> existingDictionary)
    {
        if (existingDictionary.Count == 0)
        {
            return string.Empty;
        }
        return existingDictionary.Values.OrderByDescending(name => name.Length).First();
    }
}