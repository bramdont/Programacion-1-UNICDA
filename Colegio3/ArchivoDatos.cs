// Guarda y carga los cursos y sus estudiantes en un archivo de texto.
// Una línea por estudiante: Curso|Nombre|Apellido|Nota1|Nota2|Nota3|Nota4
static class ArchivoDatos
{
    public const char Separador = '|';

    public static List<Curso> Cargar(string ruta)
    {
        List<Curso> cursos = new List<Curso>();

        if (!File.Exists(ruta))
        {
            return cursos;
        }

        try
        {
            string[] lineas = File.ReadAllLines(ruta);

            for (int i = 0; i < lineas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lineas[i]))
                {
                    continue;
                }

                if (TryLeerLinea(lineas[i], out string nombreCurso, out Estudiante? estudiante))
                {
                    Curso.ObtenerOCrear(cursos, nombreCurso).Estudiantes.Add(estudiante!);
                }
                else
                {
                    Console.WriteLine($"Aviso: la línea {i + 1} del archivo no tiene un formato válido y se ignoró.");
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"No se pudo leer el archivo: {ex.Message}");
        }

        return cursos;
    }

    public static bool Guardar(string ruta, List<Curso> cursos)
    {
        List<string> lineas = new List<string>();

        foreach (Curso curso in cursos)
        {
            foreach (Estudiante estudiante in curso.Estudiantes)
            {
                List<string> campos = new List<string> { curso.Nombre, estudiante.Nombre, estudiante.Apellido };
                campos.AddRange(estudiante.Notas.Select(n => n.ToString()));

                lineas.Add(string.Join(Separador, campos));
            }
        }

        try
        {
            File.WriteAllLines(ruta, lineas);
            return true;
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            Console.WriteLine($"No se pudo guardar el archivo: {ex.Message}");
            return false;
        }
    }

    private static bool TryLeerLinea(string linea, out string nombreCurso, out Estudiante? estudiante)
    {
        nombreCurso = string.Empty;
        estudiante = null;

        string[] campos = linea.Split(Separador);

        if (campos.Length != 3 + Estudiante.CantidadNotas)
        {
            return false;
        }

        int[] notas = new int[Estudiante.CantidadNotas];
        for (int i = 0; i < notas.Length; i++)
        {
            if (!int.TryParse(campos[3 + i], out notas[i]) || notas[i] < 0 || notas[i] > 100)
            {
                return false;
            }
        }

        string curso = campos[0].Trim();
        string nombre = campos[1].Trim();
        string apellido = campos[2].Trim();

        if (curso == "" || nombre == "" || apellido == "")
        {
            return false;
        }

        nombreCurso = curso;
        estudiante = new Estudiante(nombre, apellido, notas);
        return true;
    }
}
