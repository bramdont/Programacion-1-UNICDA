class Curso
{
    public string Nombre { get; set; }
    public List<Estudiante> Estudiantes { get; } = new List<Estudiante>();

    public Curso(string nombre)
    {
        Nombre = nombre;
    }

    // Busca un curso por nombre, sin distinguir mayúsculas de minúsculas.
    public static Curso? Buscar(List<Curso> cursos, string nombre)
    {
        return cursos.FirstOrDefault(c => c.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
    }

    // Devuelve el curso si ya existe; si no, lo crea y lo agrega a la lista.
    public static Curso ObtenerOCrear(List<Curso> cursos, string nombre)
    {
        Curso? curso = Buscar(cursos, nombre);

        if (curso == null)
        {
            curso = new Curso(nombre);
            cursos.Add(curso);
        }

        return curso;
    }

    public bool ExisteEstudiante(string nombre, string apellido)
    {
        return Estudiantes.Any(e =>
            e.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) &&
            e.Apellido.Equals(apellido, StringComparison.OrdinalIgnoreCase));
    }

    // Estudiantes ordenados por apellido y, si coincide, por nombre.
    public List<Estudiante> EstudiantesOrdenados()
    {
        return Estudiantes.OrderBy(e => e.Apellido).ThenBy(e => e.Nombre).ToList();
    }
}
