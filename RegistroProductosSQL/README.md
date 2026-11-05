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
   - Las tablas `Usuario`, `Productos`, `Factura` y `DetalleFactura`.
   - Las tablas nuevas `Cliente` y `CuentaPorCobrar`.
   - Los procedimientos almacenados del sistema (productos, sesión, cajeros, facturas y reportes).
   - Los usuarios de prueba por defecto.
4. La facturación con cajeros, cuentas por cobrar y reportes se instala ejecutando **`BaseDatos/Facturacion.sql`** en SSMS (o Azure Data Studio). El script solo **agrega** (no borra ni renombra nada) y se puede ejecutar varias veces sin problema.

---

## 🔐 Usuarios y Roles

| Usuario | Contraseña | Estado / Rol | Permisos |
| :--- | :--- | :--- | :--- |
| **`bramdont`** | `P@ssw0rd` | **`Administrador`** | Productos, crear cajeros, reportes. **No factura.** |
| **`usuario`** | `12345` | **`Invitado`** | Solo consulta y búsqueda de productos |
| *(los crea el administrador)* | | **`Cajero`** (turno Mañana, Tarde o Noche) | Solo **factura**, y únicamente dentro de su turno |

Turnos (se cambian en `Clases/Turnos.cs`): **Mañana** 06:00-14:00, **Tarde** 14:00-22:00, **Noche** 22:00-06:00.


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

1. Inicia sesión como **cajero** y pulsa **Facturar** (solo funciona dentro de su turno).
2. Escribe el nombre y la cédula del cliente, elige un artículo (o escribe su código), la cantidad y pulsa **Agregar**.
3. Elige el tipo de factura:
   - **Contado:** escribe el monto pagado; la pantalla muestra la devuelta (el pago debe cubrir el total).
   - **Crédito:** la factura se registra en **cuentas por cobrar** del cliente.
4. **Guardar factura** guarda todo en una sola transacción, descuenta el stock y genera el PDF.
5. El PDF queda en `Documentos/Facturas/Factura-00001.pdf` y se abre solo. **F5** lo vuelve a abrir.

- Numeración de 5 dígitos, consecutiva y sin repetidos (`00001`, `00002`, ...), asignada por la base de datos.
- El ITBIS (18%) se define en `Modelo/Factura.cs` (`ItbisPorcentaje`). En la BD el campo `iva` guarda el **monto** del ITBIS (por línea en `DetalleFactura` y total en `Factura`).
- Total bruto + ITBIS = Total a pagar.

## 📊 Reportes (solo administrador)

Cuadre del día, ventas por cajero, inventario en stock, facturas a crédito o contado (con filtro), los 5 clientes con más ventas y cuentas por cobrar pendientes.

## 📄 Librería del PDF

Se usa **QuestPDF** (licencia Community). Ya está en el `.csproj`; solo hay que restaurar paquetes:

```bash
dotnet restore
```

(o, en un proyecto nuevo: `dotnet add package QuestPDF`).

---

## 🛠️ Estructura del Código

- **`BaseDatos/Facturacion.sql`**: Tablas, columnas y procedimientos de la facturación (idempotente).
- **`Clases/Conexion.cs`**: Manejo de conexión a la BD mediante `SqlConnection`.
- **`Clases/Variables.cs`**: Sesión del usuario (rol, turno).
- **`Clases/Turnos.cs`**: Horarios de los turnos y validación de "dentro de turno".
- **`Clases/MetodosProducto.cs`**: Procedimientos de productos e inicio de sesión.
- **`Clases/MetodosUsuario.cs`**: Crear y listar cajeros.
- **`Clases/MetodosFactura.cs`**: Guardado de la factura (cliente, cabecera, líneas, cuenta por cobrar) en una transacción.
- **`Clases/MetodosReporte.cs`**: Consultas de los reportes.
- **`Clases/ImpresionFactura.cs`**: PDF de la factura con QuestPDF.
- **`Modelo/`**: `Producto`, `Cajero`, `Factura` y `LineaFactura`.
- **`Navegacion.cs`**, **`Mensajes.cs`**: Cambio de ventana principal y diálogos de aviso/confirmación.
- **`FormLogin`**: Login; según el rol abre `FormAdmin`, `FormCajero` o `FormProductos` (Invitado).
- **`FormAdmin`**: Menú del administrador (Productos, Cajeros, Reportes).
- **`FormCajero`**: Menú del cajero (Facturar).
- **`FormProductos`**: CRUD de productos y búsquedas.
- **`FormCajeros`**: Alta de cajeros con su turno.
- **`FormFactura`**: Ventana de facturación.
- **`FormReportes`**: Reportes.
