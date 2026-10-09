internal class Program
{
    private const string NombreArchivo = "estudiantes.txt";

    private static void Main(string[] args)
    {
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string ruta = Path.GetFullPath(NombreArchivo);
        List<Curso> cursos = ArchivoDatos.Cargar(ruta);

        Console.WriteLine("*********************************************************");
        Console.WriteLine("**              Colegio Dios es bueno                  **");
        Console.WriteLine("**           Registro de calificaciones                **");
        Console.WriteLine("*********************************************************");
        Console.WriteLine($"Archivo de datos: {ruta}");

        bool salir = false;
        do
        {
            MostrarMenu();

            switch (PedirOpcion("Seleccione una opción: ", 1, 4))
            {
                case 1:
                    AgregarEstudiante(cursos, ruta);
                    break;
                case 2:
                    EliminarEstudiante(cursos, ruta);
                    break;
                case 3:
                    MenuReportes(cursos);
                    break;
                case 4:
                    salir = true;
                    break;
            }
        } while (!salir);

        Console.WriteLine("Hasta luego.");
    }

    private static void MostrarMenu()
    {
        Console.WriteLine();
        Console.WriteLine("============== MENÚ PRINCIPAL ==============");
        Console.WriteLine("1. Agregar un estudiante");
        Console.WriteLine("2. Eliminar un estudiante");
        Console.WriteLine("3. Reportes");
        Console.WriteLine("4. Salir");
    }

    private static void MenuReportes(List<Curso> cursos)
    {
        bool volver = false;
        do
        {
            Console.WriteLine();
            Console.WriteLine("================ REPORTES ================");
            Console.WriteLine("1. Reporte de un curso");
            Console.WriteLine("2. Reporte de todos los cursos");
            Console.WriteLine("3. Volver al menú principal");

            switch (PedirOpcion("Seleccione una opción: ", 1, 3))
            {
                case 1:
                    ReporteDeUnCurso(cursos);
                    break;
                case 2:
                    ReporteGeneral(cursos);
                    break;
                case 3:
                    volver = true;
                    break;
            }
        } while (!volver);
    }

    // ---------------------------------------------------------------
    // Agregar y eliminar
    // ---------------------------------------------------------------

    private static void AgregarEstudiante(List<Curso> cursos, string ruta)
    {
        Console.WriteLine();
        Console.WriteLine("--- Agregar estudiante ---");

        if (cursos.Count > 0)
        {
            string existentes = string.Join(", ", cursos.Select(c => c.Nombre).OrderBy(n => n));
            Console.WriteLine($"Cursos existentes: {existentes}");
        }

        string nombreCurso = PedirTexto("Curso (si no existe, se crea): ");
        string nombre = PedirTexto("Nombre: ");
        string apellido = PedirTexto("Apellido: ");

        Curso? cursoExistente = Curso.Buscar(cursos, nombreCurso);
        if (cursoExistente != null && cursoExistente.ExisteEstudiante(nombre, apellido))
        {
            Console.WriteLine($"{nombre} {apellido} ya está registrado en el curso {cursoExistente.Nombre}.");
            return;
        }

        int[] notas = new int[Estudiante.CantidadNotas];
        for (int i = 0; i < notas.Length; i++)
        {
            notas[i] = PedirNota(i + 1);
        }

        Curso curso = Curso.ObtenerOCrear(cursos, nombreCurso);
        curso.Estudiantes.Add(new Estudiante(nombre, apellido, notas));

        if (ArchivoDatos.Guardar(ruta, cursos))
        {
            Console.WriteLine($"Estudiante agregado al curso {curso.Nombre}.");
        }
    }

    private static void EliminarEstudiante(List<Curso> cursos, string ruta)
    {
        Console.WriteLine();
        Console.WriteLine("--- Eliminar estudiante ---");

        Curso? curso = SeleccionarCurso(cursos);
        if (curso == null)
        {
            return;
        }

        List<Estudiante> ordenados = curso.EstudiantesOrdenados();

        Console.WriteLine();
        Console.WriteLine($"Estudiantes del curso {curso.Nombre}:");
        for (int i = 0; i < ordenados.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {ordenados[i].Apellido}, {ordenados[i].Nombre}");
        }

        int opcion = PedirOpcion("Número del estudiante a eliminar (0 para cancelar): ", 0, ordenados.Count);
        if (opcion == 0)
        {
            return;
        }

        Estudiante estudiante = ordenados[opcion - 1];
        if (!PedirSiNo($"¿Eliminar a {estudiante.Nombre} {estudiante.Apellido} del curso {curso.Nombre}? (S/N): "))
        {
            Console.WriteLine("Operación cancelada.");
            return;
        }

        curso.Estudiantes.Remove(estudiante);

        // Un curso sin estudiantes no se guarda en el archivo, así que también se quita de la lista.
        if (curso.Estudiantes.Count == 0)
        {
            cursos.Remove(curso);
        }

        if (ArchivoDatos.Guardar(ruta, cursos))
        {
            Console.WriteLine("Estudiante eliminado.");
        }
    }

    // Muestra los cursos numerados y devuelve el elegido, o null si no hay cursos o se cancela.
    private static Curso? SeleccionarCurso(List<Curso> cursos)
    {
        if (cursos.Count == 0)
        {
            Console.WriteLine("No hay estudiantes registrados.");
            return null;
        }

        List<Curso> ordenados = cursos.OrderBy(c => c.Nombre).ToList();

        Console.WriteLine("Cursos:");
        for (int i = 0; i < ordenados.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {ordenados[i].Nombre} ({ordenados[i].Estudiantes.Count} estudiantes)");
        }

        int opcion = PedirOpcion("Número del curso (0 para cancelar): ", 0, ordenados.Count);
        return opcion == 0 ? null : ordenados[opcion - 1];
    }

    // ---------------------------------------------------------------
    // Entrada de datos
    // ---------------------------------------------------------------

    private static string PedirTexto(string mensaje)
    {
        string? entrada;
        bool valido;

        do
        {
            Console.Write(mensaje);
            entrada = Console.ReadLine();

            valido = !string.IsNullOrWhiteSpace(entrada) && !entrada.Contains(ArchivoDatos.Separador);

            if (string.IsNullOrWhiteSpace(entrada))
            {
                Console.WriteLine("Este campo no puede estar vacío.");
            }
            else if (!valido)
            {
                Console.WriteLine($"Este campo no puede contener el carácter '{ArchivoDatos.Separador}'.");
            }
        } while (!valido);

        return entrada!.Trim();
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

    private static int PedirOpcion(string mensaje, int minimo, int maximo)
    {
        int opcion;

        while (true)
        {
            Console.Write(mensaje);

            if (int.TryParse(Console.ReadLine(), out opcion) && opcion >= minimo && opcion <= maximo)
            {
                return opcion;
            }

            Console.WriteLine($"Opción inválida. Ingrese un número entre {minimo} y {maximo}.");
        }
    }

    private static bool PedirSiNo(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
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

            Console.WriteLine("Respuesta inválida. Escriba S o N.");
        }
    }

    // ---------------------------------------------------------------
    // Reportes
    // ---------------------------------------------------------------

    private static void ReporteDeUnCurso(List<Curso> cursos)
    {
        Console.WriteLine();
        Curso? curso = SeleccionarCurso(cursos);

        if (curso != null)
        {
            MostrarReporteCurso(curso);
            MostrarTotales(curso.Estudiantes);
        }
    }

    private static void ReporteGeneral(List<Curso> cursos)
    {
        if (cursos.Count == 0)
        {
            Console.WriteLine("No hay estudiantes registrados.");
            return;
        }

        foreach (Curso curso in cursos.OrderBy(c => c.Nombre))
        {
            MostrarReporteCurso(curso);
            MostrarTotales(curso.Estudiantes);
        }

        Console.WriteLine();
        Console.WriteLine("=== Total general (todos los cursos) ===");
        MostrarTotales(cursos.SelectMany(c => c.Estudiantes).ToList());
    }

    private static void MostrarReporteCurso(Curso curso)
    {
        const string formatoFila = "{0,-15}{1,-15}{2,6}{3,6}{4,6}{5,6}{6,10:0.##}{7,9}";
        string separador = new string('=', 73);

        Console.WriteLine();
        Console.WriteLine("                    Colegio Dios es bueno.");
        Console.WriteLine("                Calificaciones del cuatrimestre");
        Console.WriteLine($"Curso: {curso.Nombre}");
        Console.WriteLine(separador);
        Console.WriteLine(formatoFila, "Nombre", "Apellido", "Nota1", "Nota2", "Nota3", "Nota4", "Promedio", "Literal");
        Console.WriteLine(separador);

        foreach (Estudiante estudiante in curso.EstudiantesOrdenados())
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
