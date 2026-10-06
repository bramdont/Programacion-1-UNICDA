internal partial class Program
{
    private static void Main(string[] args)
    {
        Menu menu = new Menu();
        Calculator calculator = new Calculator();

        menu.MostrarMenu();
        int selectedOption = menu.SelectOption();
        
        switch (selectedOption)
        {
            case 1:
                int[] sumNumbers = GetNumber();
                int sumResult = calculator.Add(sumNumbers[0], sumNumbers[1]);
                Console.WriteLine($"The result of the addition is: {sumResult}");
                break;
            case 2:
                int[] subtractNumbers = GetNumber();
                int subtractResult = calculator.Subtract(subtractNumbers[0], subtractNumbers[1]);
                Console.WriteLine($"The result of the subtraction is: {subtractResult}");
                break;
            case 3:
                int[] multiplyNumbers = GetNumber();
                int multiplyResult = calculator.Multiply(multiplyNumbers[0], multiplyNumbers[1]);
                Console.WriteLine($"The result of the multiplication is: {multiplyResult}");
                break;
            case 4:
                int[] divideNumbers = GetNumber();
                int divideResult = calculator.Divide(divideNumbers[0], divideNumbers[1]);
                Console.WriteLine($"The result of the division is: {divideResult}");

                break;
            case 5:
                Console.WriteLine("Please enter the number to calculate its square root: ");
                int numberForSquareRoot;
                while (!int.TryParse(Console.ReadLine(), out numberForSquareRoot))
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                }
                
                double squareRootResult = calculator.Square(numberForSquareRoot);
                Console.WriteLine($"The square root of {numberForSquareRoot} is: {squareRootResult}");
                break;
        }
        
    }

    public static int[] GetNumber()
    {
        int firstNumber;
        int secondNumber;

        while (true)
        {
            Console.WriteLine("Please enter the first number: ");
            string? firstInput = Console.ReadLine();

            if (int.TryParse(firstInput, out firstNumber))
            {
                break;
            }

            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }

        while (true)
        {
            Console.WriteLine("Please enter the second number: ");
            string? secondInput = Console.ReadLine();

            if (int.TryParse(secondInput, out secondNumber))
            {
                return [firstNumber, secondNumber];
            }

            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
    }
}

