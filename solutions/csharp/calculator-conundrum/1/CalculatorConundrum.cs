public static class SimpleCalculator
{
    public static string Calculate(int operand1, int operand2, string? operation)
    {
        if (operation is null)
        {
            throw new ArgumentNullException(nameof(operation));
        }
        if (operation == "")
        {
            throw new ArgumentException("Operation cannot be empty.", nameof(operation));
        }

        try
        {
            int result = operation switch
            {
                "+" => SimpleOperation.Addition(operand1, operand2),
                "*" => SimpleOperation.Multiplication(operand1, operand2),
                "/" => SimpleOperation.Division(operand1, operand2),
                _ => throw new ArgumentOutOfRangeException(nameof(operation), $"Operation '{operation}' is not supported.")
            };
            return $"{operand1} {operation} {operand2} = {result}";
        }
        catch (DivideByZeroException)
        {
            return "Division by zero is not allowed.";
        }
    }
}
