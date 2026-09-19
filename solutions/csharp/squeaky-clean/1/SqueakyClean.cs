using System.Text;

public static class Identifier
{
    public static string Clean(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        var capitalizeNext = false;

        foreach (var character in identifier)
        {
            if (character == ' ')
            {
                builder.Append('_');
                continue;
            }

            if (char.IsControl(character))
            {
                builder.Append("CTRL");
                continue;
            }

            if (character == '-')
            {
                capitalizeNext = true;
                continue;
            }

            if (character >= 'α' && character <= 'ω')
            {
                continue;
            }

            if (!char.IsLetter(character))
            {
                continue;
            }

            if (capitalizeNext)
            {
                builder.Append(char.ToUpperInvariant(character));
                capitalizeNext = false;
            }
            else
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}
