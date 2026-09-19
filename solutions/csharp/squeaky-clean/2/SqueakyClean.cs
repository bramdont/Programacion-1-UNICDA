using System.Text;

public static class Identifier
{
    private const string ControlReplacement = "CTRL";

    public static string Clean(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            return string.Empty;
        }

        // Reserva la capacidad inicial para evitar reasignaciones del buffer interno.
        var builder = new StringBuilder(identifier.Length);
        var capitalizeNext = false;

        foreach (var character in identifier)
        {
            switch (character)
            {
                case ' ':
                    builder.Append('_');
                    break;

                case '-':
                    capitalizeNext = true;
                    break;

                case var c when char.IsControl(c):
                    builder.Append(ControlReplacement);
                    break;

                case >= 'α' and <= 'ω':
                    break;

                case var c when char.IsLetter(c):
                    builder.Append(capitalizeNext ? char.ToUpperInvariant(c) : c);
                    capitalizeNext = false;
                    break;
            }
        }

        return builder.ToString();
    }
}
