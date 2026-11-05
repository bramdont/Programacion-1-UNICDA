-- Cajeros de prueba (contraseña 12345). Se puede ejecutar varias veces: no repite los que ya existen.
-- Requiere haber ejecutado antes Facturacion.sql

DECLARE @cajeros TABLE (cedula VARCHAR(20), nombres VARCHAR(50), apellidos VARCHAR(50), usuario VARCHAR(50), turno VARCHAR(10));

INSERT INTO @cajeros VALUES
    ('001-0000001-1', 'Maria',  'Perez',     'cajero1', 'Mañana'),
    ('001-0000002-2', 'Juan',   'Rodriguez', 'cajero2', 'Mañana'),
    ('001-0000003-3', 'Ana',    'Martinez',  'cajero3', 'Tarde'),
    ('001-0000004-4', 'Luis',   'Gomez',     'cajero4', 'Tarde'),
    ('001-0000005-5', 'Carla',  'Santos',    'cajero5', 'Noche');

DECLARE @cedula VARCHAR(20), @nombres VARCHAR(50), @apellidos VARCHAR(50), @usuario VARCHAR(50), @turno VARCHAR(10);
DECLARE @ahora DATETIME = GETDATE();

DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT cedula, nombres, apellidos, usuario, turno FROM @cajeros;
OPEN c;
FETCH NEXT FROM c INTO @cedula, @nombres, @apellidos, @usuario, @turno;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Usuario WHERE usuario = @usuario OR cedula = @cedula)
        EXEC sp_CrearCajero @cedula, @nombres, @apellidos, '', '', @usuario, '12345', @turno, @ahora;
    FETCH NEXT FROM c INTO @cedula, @nombres, @apellidos, @usuario, @turno;
END
CLOSE c;
DEALLOCATE c;
GO
