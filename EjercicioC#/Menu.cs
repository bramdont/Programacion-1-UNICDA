class Menu
{
    public void MostrarMenu()
    {
        Console.WriteLine("*********************************************************");
        Console.WriteLine("**             Ejercicio C# - 2025-0006                **");
        Console.WriteLine("*********************************************************"); 
        Console.WriteLine("**       Welcome to the simple calculatior App         **");
        Console.WriteLine("**         Please select the desire operation:         **");
        Console.WriteLine("** 1 - To sum                                          **");
        Console.WriteLine("** 2 - To subtract                                     **");
        Console.WriteLine("** 3 - To multiply                                     **");
        Console.WriteLine("** 4 - To divide                                       **");
        Console.WriteLine("** 5 - To calculate square root                        **");
        Console.WriteLine("*********************************************************");
        Console.WriteLine("** I want to select option number: ");
    }

    public int SelectOption()
    {
        int option = 0;
        do
        {
            try
            {
                option = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter a valid integer and try again.");
                continue;
            }
            catch (OverflowException)
            {
                Console.WriteLine("Input is too large or too small. Please enter a valid integer and try again.");
                continue;
            }

            if (option < 1 || option > 5)
            {
                Console.WriteLine("Invalid option. Please select a number between 1 and 5 and try again.");
            }
        } while(option < 1 || option > 5);
        Console.WriteLine($"You selected option {option}.");

        return option;
    }

    
}
