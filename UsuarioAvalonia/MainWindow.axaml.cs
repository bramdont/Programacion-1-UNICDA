using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using UsuarioAvalonia.Datos;
using UsuarioAvalonia.Modelo;

namespace UsuarioAvalonia
{
    public partial class MainWindow : Window
    {
        private const int EdadMaxima = 120;

        private ObservableCollection<UsuarioModel> tabla = new ObservableCollection<UsuarioModel>();
        private Usuario dato = new Usuario();

        public MainWindow()
        {
            InitializeComponent();
            Iniciar();
        }

        /// <summary>
        /// Asigna la colección de usuarios como fuente de datos de la grilla y
        /// carga los registros cuando la ventana ya está visible.
        /// </summary>
        private void Iniciar()
        {
            grilla.ItemsSource = tabla;
            Opened += async (_, _) => await Ejecutar(Consultar);
        }

        /// <summary>
        /// Ejecuta una operación contra la base de datos deshabilitando los botones mientras dura
        /// (evita dobles clics) y mostrando al usuario cualquier error en lugar de cerrar la aplicación.
        /// </summary>
        private async Task Ejecutar(Func<Task> operacion)
        {
            panelBotones.IsEnabled = false;
            try
            {
                await operacion();
            }
            catch (Exception ex)
            {
                await Mensaje.Mostrar(this, $"No se pudo completar la operación con la base de datos:\n{ex.Message}", "Error");
            }
            finally
            {
                panelBotones.IsEnabled = true;
            }
        }

        /// <summary>
        /// Valida los campos del formulario. Si son válidos devuelve el nombre y la edad; si no, muestra el aviso.
        /// </summary>
        private async Task<(bool valido, string nombre, int edad)> Validar()
        {
            string nombre = txtNombre.Text?.Trim() ?? string.Empty;

            if (nombre.Length == 0)
            {
                await Mensaje.Mostrar(this, "Por favor, ingrese un nombre.", "Atención");
                return (false, nombre, 0);
            }

            if (!int.TryParse(txtEdad.Text, out int edad) || edad < 0 || edad > EdadMaxima)
            {
                await Mensaje.Mostrar(this, $"Por favor, ingrese una edad válida (número entre 0 y {EdadMaxima}).", "Atención");
                return (false, nombre, 0);
            }

            return (true, nombre, edad);
        }

        /// <summary>
        /// Crea un objeto UsuarioModel con la información ingresada y llama al método Guardar de la capa Datos.
        /// </summary>
        private async Task Guardar()
        {
            var (valido, nombre, edad) = await Validar();
            if (!valido)
            {
                return;
            }

            await dato.Guardar(new UsuarioModel { Nombre = nombre, Edad = edad });

            Limpiar();
            await Consultar();
        }

        /// <summary>
        /// Actualiza el usuario seleccionado en la grilla con los datos del formulario.
        /// </summary>
        private async Task Actualizar()
        {
            if (grilla.SelectedItem is not UsuarioModel seleccionado)
            {
                await Mensaje.Mostrar(this, "Seleccione un usuario de la grilla para actualizarlo.", "Atención");
                return;
            }

            var (valido, nombre, edad) = await Validar();
            if (!valido)
            {
                return;
            }

            bool actualizado = await dato.Actualizar(new UsuarioModel { Id = seleccionado.Id, Nombre = nombre, Edad = edad });

            if (!actualizado)
            {
                await Mensaje.Mostrar(this, "El usuario ya no existe en la base de datos.", "Atención");
            }

            Limpiar();
            await Consultar();
        }

        /// <summary>
        /// Desactiva (Estatus = 0) el usuario seleccionado; deja de mostrarse en la grilla.
        /// </summary>
        private async Task Eliminar()
        {
            if (grilla.SelectedItem is not UsuarioModel seleccionado)
            {
                await Mensaje.Mostrar(this, "Seleccione un usuario de la grilla para eliminarlo.", "Atención");
                return;
            }

            if (!await Mensaje.Confirmar(this, $"¿Desea eliminar a {seleccionado.Nombre}?", "Confirmar"))
            {
                return;
            }

            await dato.Eliminar(seleccionado.Id);

            Limpiar();
            await Consultar();
        }

        /// <summary>
        /// Consulta los usuarios activos y llena la grilla con los registros.
        /// </summary>
        private async Task Consultar()
        {
            var usuarios = await dato.Consultar();

            tabla.Clear();
            foreach (var item in usuarios)
            {
                tabla.Add(item);
            }
        }

        /// <summary>
        /// Limpia los campos de texto del formulario y la selección de la grilla.
        /// </summary>
        private void Limpiar()
        {
            grilla.SelectedItem = null;
            txtNombre.Text = string.Empty;
            txtEdad.Text = string.Empty;
            txtNombre.Focus();
        }

        private async void btnGuardar_Click(object? sender, RoutedEventArgs e)
        {
            await Ejecutar(Guardar);
        }

        private async void btnActualizar_Click(object? sender, RoutedEventArgs e)
        {
            await Ejecutar(Actualizar);
        }

        private async void btnEliminar_Click(object? sender, RoutedEventArgs e)
        {
            await Ejecutar(Eliminar);
        }

        private void btnLimpiar_Click(object? sender, RoutedEventArgs e)
        {
            Limpiar();
        }

        /// <summary>
        /// Al seleccionar una fila se cargan sus datos en el formulario para poder editarlos.
        /// </summary>
        private void grilla_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (grilla.SelectedItem is UsuarioModel seleccionado)
            {
                txtNombre.Text = seleccionado.Nombre;
                txtEdad.Text = seleccionado.Edad.ToString();
            }
        }
    }
}
