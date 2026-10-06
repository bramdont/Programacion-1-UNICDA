internal class Program
{
    private static void Main(string[] args)
    {
        // Para que se vean bien los acentos y la "ñ" en la consola.
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        List<Estudiante> estudiantes = new List<Estudiante>();

        Console.WriteLine("*********************************************************");
        Console.WriteLine("**              Colegio Dios es bueno                  **");
        Console.WriteLine("**           Registro de calificaciones                **");
        Console.WriteLine("*********************************************************");

        bool continuar;
        do
        {
            Console.WriteLine();
            estudiantes.Add(PedirEstudiante());
            continuar = DeseaContinuar();
        } while (continuar);

        MostrarReporte(estudiantes);
    }

    private static Estudiante PedirEstudiante()
    {
        string nombre = PedirTexto("Nombre: ");
        string apellido = PedirTexto("Apellido: ");

        int[] notas = new int[Estudiante.CantidadNotas];
        for (int i = 0; i < notas.Length; i++)
        {
            notas[i] = PedirNota(i + 1);
        }

        return new Estudiante(nombre, apellido, notas);
    }

    private static string PedirTexto(string mensaje)
    {
        string? entrada;

        do
        {
            Console.Write(mensaje);
            entrada = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(entrada))
            {
                Console.WriteLine("Este campo no puede estar vacío.");
            }
        } while (string.IsNullOrWhiteSpace(entrada));

        return entrada.Trim();
    }

    private static int PedirNota(int numeroNota)
    {
        int nota;

        while (true)
        {
            Console.Write($"Nota {numeroNota} (0-100): ");

            if (int.TryParse(Console.ReadLine(), out nota) && nota >= 0 && nota <= 100)
            {
                return nota;
            }

            Console.WriteLine("Entrada inválida. Ingrese un número entero entre 0 y 100.");
        }
    }

    private static bool DeseaContinuar()
    {
        while (true)
        {
            Console.Write("¿Desea ingresar otro estudiante? (S/N): ");
            string? respuesta = Console.ReadLine();

            if (respuesta != null)
            {
                respuesta = respuesta.Trim().ToUpper();

                if (respuesta == "S")
                {
                    return true;
                }

                if (respuesta == "N")
                {
                    return false;
                }
            }

            Console.WriteLine("Respuesta inválida. Escriba S para continuar o N para terminar.");
        }
    }

    private static void MostrarReporte(List<Estudiante> estudiantes)
    {
        const string formatoFila = "{0,-15}{1,-15}{2,6}{3,6}{4,6}{5,6}{6,10:0.##}{7,9}";
        string separador = new string('=', 73);

        Console.WriteLine();
        Console.WriteLine("                    Colegio Dios es bueno.");
        Console.WriteLine("                Calificaciones del cuatrimestre");
        Console.WriteLine(separador);
        Console.WriteLine(formatoFila, "Nombre", "Apellido", "Nota1", "Nota2", "Nota3", "Nota4", "Promedio", "Literal");
        Console.WriteLine(separador);

        // Se ordena una copia de la lista, por apellido y, si coincide, por nombre.
        List<Estudiante> ordenados = estudiantes.OrderBy(e => e.Apellido).ThenBy(e => e.Nombre).ToList();

        foreach (Estudiante estudiante in ordenados)
        {
            Console.WriteLine(
                formatoFila,
                estudiante.Nombre,
                estudiante.Apellido,
                estudiante.Notas[0],
                estudiante.Notas[1],
                estudiante.Notas[2],
                estudiante.Notas[3],
                estudiante.Promedio,
                estudiante.Literal);
        }

        MostrarTotales(estudiantes);
    }

    private static void MostrarTotales(List<Estudiante> estudiantes)
    {
        string[] literales = { "A", "B", "C", "D", "F" };

        Console.WriteLine();
        Console.WriteLine("Total de estudiantes por literal:");

        foreach (string literal in literales)
        {
            int total = estudiantes.Count(e => e.Literal == literal);
            Console.WriteLine($"  Literal {literal}: {total}");
        }
    }
}
