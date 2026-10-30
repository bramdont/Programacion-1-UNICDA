# Registro de Productos en C# Avalonia y SQL Server

Este proyecto implementa el sistema completo de **Registro de Productos, Control de Usuarios y sistema de facturacion** 

---

## 🚀 Requisitos del Sistema

1. **Microsoft SQL Server** (2019 / 2022 / Express) con SQL Server Management Studio (SSMS).
2. **.NET 8.0 SDK** (o superior) instalado en tu computadora. Funciona en Windows, macOS y Linux.
3. **Visual Studio Code** o **Visual Studio 2022**.

---

## 🗄️ Pasos de Configuración de la Base de Datos

1. Abre **SQL Server Management Studio (SSMS)** y conéctate a tu servidor local o de azure.
2. Crea la estructura de la base de datos segun la necesidad.
3. Para este programa, estamos utilizando la base de datos personal de un servidor (Bramdont Garcia) por motivos instructivos unicamente:
   - La base de datos `programacion1`.
   - Las tablas `Usuario`, `Productos`, `Factura`y `DetalleFactura`, `Productos`.
   - Los **11 Procedimientos Almacenados** del sistema.
   - Los usuarios de prueba por defecto.

---

## 🔐 Usuarios de Prueba Creados

| Usuario | Contraseña | Estado / Rol | Permisos |
| :--- | :--- | :--- | :--- |
| **`bramdont`** | `P@ssw0rd` | **`Administrador`** | Acceso total (Nuevo, Insertar, Actualizar, Eliminar, Buscar) |
| **`usuario`** | `12345` | **`Invitado`** | Acceso restringido (Solo consulta y búsqueda) |

---

## ⚙️ Configuración de la Cadena de Conexión

Abre el archivo `Clases/Conexion.cs` y verifica la cadena de conexión:

```csharp
private static string cadenaConexion = @"Data Source=localhost;Initial Catalog=Almacen;Integrated Security=True;TrustServerCertificate=True;";
```

*Si usas SQL Express, cambia `localhost` por `.\SQLEXPRESS`.*

> **macOS / Linux:** `Integrated Security=True` solo funciona en Windows. Usa autenticación SQL, por ejemplo:
> `Data Source=localhost;Initial Catalog=Almacen;User Id=sa;Password=TuClave;TrustServerCertificate=True;`

---

## 💻 Cómo Ejecutar en Visual Studio Code

1. Abre VS Code y ve a `Archivo > Abrir carpeta...`.
2. Selecciona la carpeta **`RegistroProductosSQL`**.
3. Abre la terminal integrada (`Ctrl + ~`) y ejecuta:
   ```bash
   dotnet run
   ```
4. Ingresa con los usuarios de prueba (`bramdont` / `P@ssw0rd` o `usuario` / `12345`).

---

## 🧾 Facturación

1. En la ventana de productos pulsa **Facturar**.
2. Escribe el cliente, elige un artículo, la cantidad y pulsa **Agregar** (repite para más artículos; **Quitar** elimina la línea seleccionada).
3. **Guardar factura** registra la venta y descuenta el stock en una sola transacción.
4. Pulsa **F5** (o el botón *Imprimir*) para guardar la factura como **PDF**

El ITBIS (18%) se define en `Modelo/Factura.cs` (`ItbisPorcentaje`). Cualquier usuario (Administrador o Invitado) puede facturar.

---

## 🛠️ Estructura del Código

- **`Clases/Conexion.cs`**: Manejo de conexión a la BD mediante `SqlConnection`.
- **`Clases/Variables.cs`**: Almacena la sesión del usuario (Rol/Estado Administrador vs Invitado).
- **`Clases/MetodosProducto.cs`**: Ejecución de los 9 procedimientos almacenados.
- **`Modelo/Producto.cs`**: Modelo que convierte el `DataTable` de los SP en filas para el `DataGrid`.
- **`Clases/MetodosFactura.cs`** y **`Clases/ImpresionFactura.cs`**: Guardado de facturas y generación de TXT/PDF.
- **`FormFactura.axaml(.cs)`**: Ventana de facturación.
- **`Mensajes.cs`**: Diálogos de aviso/confirmación (reemplazo de `MessageBox`).
- **`FormLogin.axaml(.cs)`**: Pantalla de Login con validación de credenciales.
- **`FormProductos.axaml(.cs)`**: Ventana principal CRUD de productos y búsquedas dinámicas.
