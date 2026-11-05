-- ============================================================
-- FACTURACIÓN CON CAJEROS, CUENTAS POR COBRAR Y REPORTES
-- Base de datos: programacion1 (SQL Server / Azure SQL)
--
-- Script ADITIVO e IDEMPOTENTE: se puede ejecutar varias veces.
-- No borra datos ni renombra nada. El monto del ITBIS sigue
-- guardándose en los campos llamados "iva".
-- ============================================================

-- ------------------------------------------------------------
-- 1. Usuario: turno del cajero (estado nuevo: 'Cajero')
-- ------------------------------------------------------------
IF COL_LENGTH('Usuario', 'turno') IS NULL
    ALTER TABLE Usuario ADD turno VARCHAR(10) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Usuario_turno')
    ALTER TABLE Usuario ADD CONSTRAINT CK_Usuario_turno
        CHECK (turno IS NULL OR turno IN ('Mañana', 'Tarde', 'Noche'));
GO

-- Todo cajero debe tener turno
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Usuario_cajero_turno')
    ALTER TABLE Usuario ADD CONSTRAINT CK_Usuario_cajero_turno
        CHECK (estado <> 'Cajero' OR turno IS NOT NULL);
GO

-- ------------------------------------------------------------
-- 2. Cliente (la cédula identifica al cliente)
-- ------------------------------------------------------------
IF OBJECT_ID('Cliente', 'U') IS NULL
    CREATE TABLE Cliente (
        id_cliente INT IDENTITY(1,1) PRIMARY KEY,
        cedula     VARCHAR(20)  NOT NULL UNIQUE,
        nombre     VARCHAR(100) NOT NULL
    );
GO

-- ------------------------------------------------------------
-- 3. Factura: número de 5 dígitos, tipo, cliente y pago
-- ------------------------------------------------------------
IF COL_LENGTH('Factura', 'numero') IS NULL
    ALTER TABLE Factura ADD numero INT NULL;
IF COL_LENGTH('Factura', 'tipo') IS NULL
    ALTER TABLE Factura ADD tipo VARCHAR(10) NOT NULL CONSTRAINT DF_Factura_tipo DEFAULT 'Contado';
IF COL_LENGTH('Factura', 'id_cliente') IS NULL
    ALTER TABLE Factura ADD id_cliente INT NULL;
IF COL_LENGTH('Factura', 'monto_pagado') IS NULL
    ALTER TABLE Factura ADD monto_pagado DECIMAL(12,2) NULL;
IF COL_LENGTH('Factura', 'devuelta') IS NULL
    ALTER TABLE Factura ADD devuelta DECIMAL(12,2) NULL;
GO

-- Facturas anteriores: numeración consecutiva, contado y pagadas completas
;WITH ordenadas AS (
    SELECT id_factura, ROW_NUMBER() OVER (ORDER BY id_factura) AS n
    FROM Factura
    WHERE numero IS NULL
)
UPDATE f
SET numero = ISNULL((SELECT MAX(numero) FROM Factura), 0) + o.n
FROM Factura f
JOIN ordenadas o ON o.id_factura = f.id_factura;

UPDATE Factura SET monto_pagado = total, devuelta = 0
WHERE tipo = 'Contado' AND monto_pagado IS NULL;

-- Facturas anteriores: se crean los clientes a partir de su cédula
INSERT INTO Cliente (cedula, nombre)
SELECT f.cliente_cedula,
       (SELECT TOP 1 x.cliente_nombre FROM Factura x
        WHERE x.cliente_cedula = f.cliente_cedula ORDER BY x.id_factura)
FROM Factura f
WHERE f.cliente_cedula IS NOT NULL AND f.cliente_cedula <> ''
  AND NOT EXISTS (SELECT 1 FROM Cliente c WHERE c.cedula = f.cliente_cedula)
GROUP BY f.cliente_cedula;

UPDATE f SET id_cliente = c.id_cliente
FROM Factura f
JOIN Cliente c ON c.cedula = f.cliente_cedula
WHERE f.id_cliente IS NULL;
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('Factura') AND name = 'numero' AND is_nullable = 1)
    ALTER TABLE Factura ALTER COLUMN numero INT NOT NULL;
GO

-- Un número nunca se repite
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Factura_numero')
    CREATE UNIQUE INDEX UQ_Factura_numero ON Factura (numero);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Factura_numero')
    ALTER TABLE Factura ADD CONSTRAINT CK_Factura_numero CHECK (numero BETWEEN 1 AND 99999);
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Factura_tipo')
    ALTER TABLE Factura ADD CONSTRAINT CK_Factura_tipo CHECK (tipo IN ('Contado', 'Credito'));
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Factura_Cliente')
    ALTER TABLE Factura ADD CONSTRAINT FK_Factura_Cliente
        FOREIGN KEY (id_cliente) REFERENCES Cliente (id_cliente);
GO

-- Número con ceros a la izquierda (00001, 00002, ...) listo para los reportes
IF COL_LENGTH('Factura', 'numero_texto') IS NULL
    ALTER TABLE Factura ADD numero_texto AS RIGHT('00000' + CAST(numero AS VARCHAR(5)), 5);
GO

-- ------------------------------------------------------------
-- 4. DetalleFactura: ITBIS de cada línea (campo "iva" = monto)
-- ------------------------------------------------------------
IF COL_LENGTH('DetalleFactura', 'iva') IS NULL
    ALTER TABLE DetalleFactura ADD iva DECIMAL(12,2) NOT NULL CONSTRAINT DF_DetalleFactura_iva DEFAULT 0;
GO

-- Líneas anteriores: se reparte el iva de su factura según el valor de la línea
UPDATE d SET iva = ROUND(d.subtotal * f.iva / NULLIF(f.subtotal, 0), 2)
FROM DetalleFactura d
JOIN Factura f ON f.id_factura = d.id_factura
WHERE d.iva = 0;
GO

-- ------------------------------------------------------------
-- 5. Cuentas por cobrar (facturas a crédito)
-- ------------------------------------------------------------
IF OBJECT_ID('CuentaPorCobrar', 'U') IS NULL
    CREATE TABLE CuentaPorCobrar (
        id_cuenta  INT IDENTITY(1,1) PRIMARY KEY,
        id_factura INT NOT NULL UNIQUE REFERENCES Factura (id_factura),
        id_cliente INT NOT NULL REFERENCES Cliente (id_cliente),
        fecha      DATETIME NOT NULL,
        monto      DECIMAL(12,2) NOT NULL,
        saldo      DECIMAL(12,2) NOT NULL,
        estado     VARCHAR(10) NOT NULL DEFAULT 'Pendiente'
                   CHECK (estado IN ('Pendiente', 'Pagada'))
    );
GO

-- ============================================================
-- PROCEDIMIENTOS ALMACENADOS
-- ============================================================

-- Iniciar sesión: ahora devuelve también el turno
CREATE OR ALTER PROCEDURE sp_IniciarSesion
    @usuario VARCHAR(50),
    @clave VARCHAR(50)
AS
BEGIN
    SELECT id_usuario, nombres, apellidos, usuario, estado, turno
    FROM Usuario
    WHERE usuario = @usuario AND clave = @clave;
END;
GO

-- Crear cajero (estado 'Cajero' con su turno)
CREATE OR ALTER PROCEDURE sp_CrearCajero
    @cedula VARCHAR(20),
    @nombres VARCHAR(50),
    @apellidos VARCHAR(50),
    @telefono VARCHAR(20),
    @email VARCHAR(100),
    @usuario VARCHAR(50),
    @clave VARCHAR(50),
    @turno VARCHAR(10),
    @fecha_ingreso DATETIME
AS
BEGIN
    IF @turno NOT IN ('Mañana', 'Tarde', 'Noche')
    BEGIN
        RAISERROR('Turno inválido. Use Mañana, Tarde o Noche.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Usuario WHERE usuario = @usuario)
    BEGIN
        RAISERROR('Ya existe un usuario con ese nombre de usuario.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Usuario WHERE cedula = @cedula)
    BEGIN
        RAISERROR('Ya existe un usuario con esa cédula.', 16, 1);
        RETURN;
    END

    INSERT INTO Usuario (cedula, nombres, apellidos, telefono, email, usuario, clave, estado, fecha_ingreso, turno)
    VALUES (@cedula, @nombres, @apellidos, @telefono, @email, @usuario, @clave, 'Cajero', @fecha_ingreso, @turno);
END;
GO

CREATE OR ALTER PROCEDURE sp_ListarCajeros
AS
BEGIN
    SELECT id_usuario, cedula, nombres, apellidos, usuario, turno
    FROM Usuario
    WHERE estado = 'Cajero'
    ORDER BY nombres, apellidos;
END;
GO

-- Busca al cliente por cédula; si no existe lo crea. Devuelve su id
CREATE OR ALTER PROCEDURE sp_GuardarCliente
    @cedula VARCHAR(20),
    @nombre VARCHAR(100)
AS
BEGIN
    DECLARE @id INT;

    SELECT @id = id_cliente FROM Cliente WHERE cedula = @cedula;

    IF @id IS NULL
    BEGIN
        INSERT INTO Cliente (cedula, nombre) VALUES (@cedula, @nombre);
        SET @id = SCOPE_IDENTITY();
    END

    SELECT @id AS id_cliente;
END;
GO

-- Cabecera de factura. Asigna el número consecutivo (sin repetidos ni saltos)
-- y valida que los totales cuadren. Los parámetros nuevos son opcionales.
CREATE OR ALTER PROCEDURE sp_InsertarFactura
    @fecha DATETIME,
    @id_usuario INT,
    @cliente_nombre VARCHAR(100),
    @cliente_cedula VARCHAR(20),
    @subtotal DECIMAL(12,2),
    @iva DECIMAL(12,2),
    @total DECIMAL(12,2),
    @tipo VARCHAR(10) = 'Contado',
    @id_cliente INT = NULL,
    @monto_pagado DECIMAL(12,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @tipo NOT IN ('Contado', 'Credito')
    BEGIN
        RAISERROR('Tipo de factura inválido.', 16, 1);
        RETURN;
    END

    IF @subtotal + @iva <> @total
    BEGIN
        RAISERROR('Los totales no cuadran: total bruto + ITBIS debe ser igual al total a pagar.', 16, 1);
        RETURN;
    END

    DECLARE @devuelta DECIMAL(12,2);

    IF @tipo = 'Contado'
    BEGIN
        IF @monto_pagado IS NULL SET @monto_pagado = @total;

        IF @monto_pagado < @total
        BEGIN
            RAISERROR('El monto pagado no cubre el total de la factura.', 16, 1);
            RETURN;
        END

        SET @devuelta = @monto_pagado - @total;
    END
    ELSE
    BEGIN
        -- A crédito no se paga nada al facturar: queda en cuentas por cobrar
        SET @monto_pagado = 0;
        SET @devuelta = 0;
    END

    -- Siguiente número: el bloqueo evita que dos cajeros reciban el mismo
    DECLARE @numero INT;
    SELECT @numero = ISNULL(MAX(numero), 0) + 1 FROM Factura WITH (UPDLOCK, HOLDLOCK);

    IF @numero > 99999
    BEGIN
        RAISERROR('Se agotó la numeración de 5 dígitos (99999).', 16, 1);
        RETURN;
    END

    INSERT INTO Factura (fecha, id_usuario, cliente_nombre, cliente_cedula, subtotal, iva, total,
                         numero, tipo, id_cliente, monto_pagado, devuelta)
    VALUES (@fecha, @id_usuario, @cliente_nombre, @cliente_cedula, @subtotal, @iva, @total,
            @numero, @tipo, @id_cliente, @monto_pagado, @devuelta);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS id_factura, @numero AS numero;
END;
GO

-- Línea de factura: descuenta el stock y guarda el ITBIS de la línea (campo iva)
CREATE OR ALTER PROCEDURE sp_InsertarDetalleFactura
    @id_factura INT,
    @id_producto INT,
    @cantidad INT,
    @precio DECIMAL(10,2),
    @iva DECIMAL(12,2) = 0
AS
BEGIN
    IF @cantidad <= 0
    BEGIN
        RAISERROR('La cantidad debe ser mayor que cero (código %d).', 16, 1, @id_producto);
        RETURN;
    END

    UPDATE Productos
    SET stock = stock - @cantidad
    WHERE id_producto = @id_producto AND stock >= @cantidad;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Stock insuficiente o producto inexistente (código %d).', 16, 1, @id_producto);
        RETURN;
    END

    INSERT INTO DetalleFactura (id_factura, id_producto, nombre_producto, cantidad, precio_unitario, subtotal, iva)
    SELECT @id_factura, id_producto, nombre, @cantidad, @precio, @cantidad * @precio, @iva
    FROM Productos
    WHERE id_producto = @id_producto;
END;
GO

-- Factura a crédito -> cuentas por cobrar
CREATE OR ALTER PROCEDURE sp_InsertarCuentaPorCobrar
    @id_factura INT,
    @id_cliente INT,
    @fecha DATETIME,
    @monto DECIMAL(12,2)
AS
BEGIN
    INSERT INTO CuentaPorCobrar (id_factura, id_cliente, fecha, monto, saldo, estado)
    VALUES (@id_factura, @id_cliente, @fecha, @monto, @monto, 'Pendiente');
END;
GO

-- ============================================================
-- REPORTES
-- ============================================================

-- Cuadre del día (o del rango de fechas indicado)
CREATE OR ALTER PROCEDURE sp_ReporteCuadre
    @desde DATE,
    @hasta DATE
AS
BEGIN
    SELECT Tipo, Facturas, [Total bruto], ITBIS, [Total a pagar]
    FROM (
        SELECT 1 AS orden,
               CASE t.tipo WHEN 'Credito' THEN 'Crédito' ELSE t.tipo END AS Tipo,
               COUNT(f.id_factura) AS Facturas,
               ISNULL(SUM(f.subtotal), 0) AS [Total bruto],
               ISNULL(SUM(f.iva), 0) AS ITBIS,
               ISNULL(SUM(f.total), 0) AS [Total a pagar]
        FROM (VALUES ('Contado'), ('Credito')) AS t(tipo)
        LEFT JOIN Factura f ON f.tipo = t.tipo
                           AND f.fecha >= @desde AND f.fecha < DATEADD(DAY, 1, @hasta)
        GROUP BY t.tipo
        UNION ALL
        SELECT 2, 'TOTAL', COUNT(*), ISNULL(SUM(subtotal), 0), ISNULL(SUM(iva), 0), ISNULL(SUM(total), 0)
        FROM Factura
        WHERE fecha >= @desde AND fecha < DATEADD(DAY, 1, @hasta)
    ) r
    ORDER BY orden, Tipo;
END;
GO

CREATE OR ALTER PROCEDURE sp_ReporteVentasPorCajero
    @desde DATE,
    @hasta DATE
AS
BEGIN
    SELECT u.nombres + ' ' + u.apellidos AS Cajero,
           ISNULL(u.turno, '-') AS Turno,
           COUNT(f.id_factura) AS Facturas,
           SUM(f.subtotal) AS [Total bruto],
           SUM(f.iva) AS ITBIS,
           SUM(f.total) AS [Total a pagar]
    FROM Factura f
    JOIN Usuario u ON u.id_usuario = f.id_usuario
    WHERE f.fecha >= @desde AND f.fecha < DATEADD(DAY, 1, @hasta)
    GROUP BY u.id_usuario, u.nombres, u.apellidos, u.turno
    ORDER BY SUM(f.total) DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_ReporteInventario
AS
BEGIN
    SELECT id_producto AS Código, nombre AS Nombre, marca AS Marca,
           precio AS Precio, stock AS Stock, precio * stock AS [Valor en stock]
    FROM Productos
    ORDER BY nombre;
END;
GO

-- @tipo: NULL = todas, 'Contado' o 'Credito'
CREATE OR ALTER PROCEDURE sp_ReporteFacturas
    @desde DATE,
    @hasta DATE,
    @tipo VARCHAR(10) = NULL
AS
BEGIN
    SELECT f.numero_texto AS Número,
           CONVERT(VARCHAR(10), f.fecha, 103) AS Fecha,
           CONVERT(VARCHAR(5), f.fecha, 108) AS Hora,
           CASE f.tipo WHEN 'Credito' THEN 'Crédito' ELSE f.tipo END AS Tipo,
           u.nombres + ' ' + u.apellidos AS Cajero,
           f.cliente_nombre AS Cliente,
           f.subtotal AS [Total bruto],
           f.iva AS ITBIS,
           f.total AS [Total a pagar]
    FROM Factura f
    JOIN Usuario u ON u.id_usuario = f.id_usuario
    WHERE f.fecha >= @desde AND f.fecha < DATEADD(DAY, 1, @hasta)
      AND (@tipo IS NULL OR f.tipo = @tipo)
    ORDER BY f.numero;
END;
GO

CREATE OR ALTER PROCEDURE sp_ReporteTopClientes
    @desde DATE,
    @hasta DATE
AS
BEGIN
    SELECT TOP 5
           ROW_NUMBER() OVER (ORDER BY SUM(f.total) DESC, c.cedula) AS Puesto,
           c.nombre AS Cliente,
           c.cedula AS Cédula,
           COUNT(f.id_factura) AS Facturas,
           SUM(f.total) AS [Total comprado]
    FROM Factura f
    JOIN Cliente c ON c.id_cliente = f.id_cliente
    WHERE f.fecha >= @desde AND f.fecha < DATEADD(DAY, 1, @hasta)
    GROUP BY c.id_cliente, c.nombre, c.cedula
    ORDER BY SUM(f.total) DESC, c.cedula;
END;
GO

CREATE OR ALTER PROCEDURE sp_ReporteCuentasPorCobrar
AS
BEGIN
    SELECT f.numero_texto AS Factura,
           CONVERT(VARCHAR(10), cc.fecha, 103) AS Fecha,
           c.nombre AS Cliente,
           c.cedula AS Cédula,
           cc.monto AS Monto,
           cc.saldo AS Saldo,
           cc.estado AS Estado
    FROM CuentaPorCobrar cc
    JOIN Factura f ON f.id_factura = cc.id_factura
    JOIN Cliente c ON c.id_cliente = cc.id_cliente
    ORDER BY f.numero;
END;
GO
