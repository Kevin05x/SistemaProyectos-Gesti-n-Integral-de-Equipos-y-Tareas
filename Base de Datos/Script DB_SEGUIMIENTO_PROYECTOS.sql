GO

USE master

GO
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'BD_SEGUIMIENTO_PROYECTOS')
BEGIN
    ALTER DATABASE BD_SEGUIMIENTO_PROYECTOS SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BD_SEGUIMIENTO_PROYECTOS;
END
GO

GO
CREATE DATABASE BD_SEGUIMIENTO_PROYECTOS

GO
USE BD_SEGUIMIENTO_PROYECTOS

GO
CREATE TABLE Equipo (
    EquipoID INT PRIMARY KEY IDENTITY(1, 1),
    NombreEquipo NVARCHAR(200) NOT NULL,
    Descripcion TEXT NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
)


GO
CREATE TABLE Usuario (
    UsuarioID INT PRIMARY KEY IDENTITY(1, 1),
    NombreUsuario NVARCHAR(200) NOT NULL,
    NombreCompleto NVARCHAR(250) NOT NULL,
    Correo NVARCHAR(150) NOT NULL,
    Contrasena NVARCHAR(255) NOT NULL,
    Rol NVARCHAR(100) NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
)

GO
CREATE TABLE DetalleEquipo (
    DetalleEquipoID INT PRIMARY KEY IDENTITY(1, 1),
    EquipoID INT NOT NULL
    CONSTRAINT FK_EQUIPODETALLEEQUIPO
    FOREIGN KEY (EquipoID) REFERENCES Equipo(EquipoID),
    UsuarioID INT NOT NULL
    CONSTRAINT FK_USUARIODETALLEEQUIPO
    FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID)
)

GO
CREATE TABLE Proyecto (
    ProyectoID INT PRIMARY KEY IDENTITY(1, 1),
    NombreProyecto NVARCHAR(200) NOT NULL,
    Descripcion TEXT NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    Estado VARCHAR(50) NOT NULL,
    EquipoID INT NOT NULL
    CONSTRAINT FK_EQUIPOPROYECTO
    FOREIGN KEY (EquipoID) REFERENCES Equipo(EquipoID),
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
)

GO
CREATE TABLE Tarea (
    TareaID INT PRIMARY KEY IDENTITY(1, 1),
    Titulo NVARCHAR(200) NOT NULL,
    Descripcion TEXT NOT NULL,
    Estado VARCHAR(50) NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    ProyectoID INT NOT NULL
    CONSTRAINT FK_PROYECTOTAREA
    FOREIGN KEY (ProyectoID) REFERENCES Proyecto(ProyectoID),
    UsuarioID INT NOT NULL --El usuaro debe pertenecer al equipo asignado en dicho proyecto
    CONSTRAINT FK_USUARIOTAREA
    FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID),
    Prioridad NVARCHAR(50) NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
    
)

GO
CREATE TABLE Comentario (
    ComentarioID INT PRIMARY KEY IDENTITY(1, 1),
    Contenido TEXT NOT NULL,
    UsuarioID INT NOT NULL
    CONSTRAINT FK_USUARIOCOMENTARIO
    FOREIGN KEY (UsuarioID) REFERENCES Usuario(UsuarioID),
    TareaID INT NOT NULL
    CONSTRAINT FK_TAREACOMENTARIO
    FOREIGN KEY (TareaID) REFERENCES Tarea(TareaID),
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
)

GO--EQUIPOS

INSERT INTO Equipo (NombreEquipo, Descripcion)
VALUES 
('Alfa', 'Equipo especializado en análisis de datos y generación de reportes'),
('Beta', 'Equipo de desarrollo de aplicaciones móviles y web'),
('Gamma', 'Equipo encargado de infraestructura de redes y servidores'),
('Delta', 'Equipo de soporte técnico y atención al usuario final'),
('Epsilon', 'Equipo de investigación y desarrollo de nuevas tecnologías'),
('Zeta', 'Equipo de pruebas y control de calidad de software'),
('Eta', 'Equipo de marketing digital y redes sociales'),
('Theta', 'Equipo de diseño gráfico y experiencia de usuario'),
('Iota', 'Equipo de inteligencia artificial y machine learning'),
('Kappa', 'Equipo de ciberseguridad y auditorías de sistemas'),
('Lambda', 'Equipo de gestión de proyectos corporativos'),
('Mu', 'Equipo de capacitación y formación interna'),
('Nu', 'Equipo de desarrollo de videojuegos'),
('Xi', 'Equipo de bases de datos y administración de información'),
('Omicron', 'Equipo de automatización y robótica industrial'),
('Pi', 'Equipo de gestión documental y archivística'),
('Rho', 'Equipo de innovación y mejora de procesos'),
('Sigma', 'Equipo de soporte remoto y asistencia técnica'),
('Tau', 'Equipo de logística y distribución tecnológica'),
('Upsilon', 'Equipo de administración y soporte de ERP empresarial');

GO--Usuarios

INSERT INTO Usuario (NombreUsuario, NombreCompleto, Correo, Contrasena, Rol)
VALUES
('jlopez', 'Juan López', 'jlopez@example.com', 'pass123', 'Empleado'),
('mgarcia', 'María García', 'mgarcia@example.com', 'pass123', 'Administrador'),
('cperez', 'Carlos Pérez', 'cperez@example.com', 'pass123', 'Empleado'),
('arodriguez', 'Ana Rodríguez', 'arodriguez@example.com', 'pass123', 'Administrador'),
('fmartinez', 'Francisco Martínez', 'fmartinez@example.com', 'pass123', 'Empleado'),
('lhernandez', 'Laura Hernández', 'lhernandez@example.com', 'pass123', 'Administrador'),
('dtorres', 'Diego Torres', 'dtorres@example.com', 'pass123', 'Empleado'),
('mmorales', 'Marta Morales', 'mmorales@example.com', 'pass123', 'Administrador'),
('rfernandez', 'Raúl Fernández', 'rfernandez@example.com', 'pass123', 'Empleado'),
('psanchez', 'Patricia Sánchez', 'psanchez@example.com', 'pass123', 'Administrador'),
('jramirez', 'Jorge Ramírez', 'jramirez@example.com', 'pass123', 'Empleado'),
('cortega', 'Carmen Ortega', 'cortega@example.com', 'pass123', 'Administrador'),
('gromero', 'Gabriel Romero', 'gromero@example.com', 'pass123', 'Empleado'),
('pnavarro', 'Pablo Navarro', 'pnavarro@example.com', 'pass123', 'Administrador'),
('nreyes', 'Nuria Reyes', 'nreyes@example.com', 'pass123', 'Empleado'),
('asantos', 'Andrés Santos', 'asantos@example.com', 'pass123', 'Administrador'),
('mruiz', 'Mónica Ruiz', 'mruiz@example.com', 'pass123', 'Empleado'),
('hcastro', 'Hugo Castro', 'hcastro@example.com', 'pass123', 'Administrador'),
('vibanez', 'Víctor Ibáñez', 'vibanez@example.com', 'pass123', 'Empleado'),
('egarcia', 'Elena García', 'egarcia@example.com', 'pass123', 'Administrador'),
('rramos', 'Ricardo Ramos', 'rramos@example.com', 'pass123', 'Empleado'),
('yfernandez', 'Yolanda Fernández', 'yfernandez@example.com', 'pass123', 'Administrador'),
('jdiaz', 'Javier Díaz', 'jdiaz@example.com', 'pass123', 'Empleado'),
('mprieto', 'Manuel Prieto', 'mprieto@example.com', 'pass123', 'Administrador'),
('acastro', 'Alicia Castro', 'acastro@example.com', 'pass123', 'Empleado'),
('frodriguez', 'Felipe Rodríguez', 'frodriguez@example.com', 'pass123', 'Administrador'),
('cbenitez', 'Cristina Benítez', 'cbenitez@example.com', 'pass123', 'Empleado'),
('rlopez', 'Roberto López', 'rlopez@example.com', 'pass123', 'Administrador'),
('jmartin', 'Javier Martín', 'jmartin@example.com', 'pass123', 'Empleado'),
('eluna', 'Estela Luna', 'eluna@example.com', 'pass123', 'Administrador'),
('pmendoza', 'Pedro Mendoza', 'pmendoza@example.com', 'pass123', 'Empleado'),
('mcampos', 'Marta Campos', 'mcampos@example.com', 'pass123', 'Administrador'),
('aaguilar', 'Alberto Aguilar', 'aaguilar@example.com', 'pass123', 'Empleado'),
('dfernandez', 'Daniela Fernández', 'dfernandez@example.com', 'pass123', 'Administrador'),
('eortiz', 'Eduardo Ortiz', 'eortiz@example.com', 'pass123', 'Empleado'),
('nmartinez', 'Natalia Martínez', 'nmartinez@example.com', 'pass123', 'Administrador'),
('agarcia', 'Adrián García', 'agarcia@example.com', 'pass123', 'Empleado'),
('gsantos', 'Gloria Santos', 'gsantos@example.com', 'pass123', 'Administrador'),
('jtorres', 'Julián Torres', 'jtorres@example.com', 'pass123', 'Empleado'),
('mdominguez', 'Marcos Domínguez', 'mdominguez@example.com', 'pass123', 'Administrador'),
('lperez', 'Lorena Pérez', 'lperez@example.com', 'pass123', 'Empleado'),
('emartin', 'Enrique Martín', 'emartin@example.com', 'pass123', 'Administrador'),
('scarvajal', 'Sara Carvajal', 'scarvajal@example.com', 'pass123', 'Empleado'),
('jfuentes', 'José Fuentes', 'jfuentes@example.com', 'pass123', 'Administrador'),
('rsalazar', 'Rosa Salazar', 'rsalazar@example.com', 'pass123', 'Empleado'),
('mvaldez', 'Mario Valdez', 'mvaldez@example.com', 'pass123', 'Administrador'),
('iflores', 'Isabel Flores', 'iflores@example.com', 'pass123', 'Empleado'),
('lrojas', 'Luis Rojas', 'lrojas@example.com', 'pass123', 'Administrador'),
('yparedes', 'Yuri Paredes', 'yparedes@example.com', 'pass123', 'Empleado'),
('mvelasquez', 'Miguel Velásquez', 'mvelasquez@example.com', 'pass123', 'Administrador'),
('pgomez', 'Paula Gómez', 'pgomez@example.com', 'pass123', 'Empleado'),
('dmejia', 'David Mejía', 'dmejia@example.com', 'pass123', 'Administrador'),
('eserrano', 'Eva Serrano', 'eserrano@example.com', 'pass123', 'Empleado'),
('fguzman', 'Fernando Guzmán', 'fguzman@example.com', 'pass123', 'Administrador'),
('rmendez', 'Ruth Méndez', 'rmendez@example.com', 'pass123', 'Empleado'),
('cjimenez', 'César Jiménez', 'cjimenez@example.com', 'pass123', 'Administrador'),
('jrodriguez', 'Julio Rodríguez', 'jrodriguez@example.com', 'pass123', 'Empleado'),
('eblanco', 'Esteban Blanco', 'eblanco@example.com', 'pass123', 'Administrador'),
('vcastillo', 'Verónica Castillo', 'vcastillo@example.com', 'pass123', 'Administrador'),
('mgomez', 'Manuel Gómez', 'mgomez@example.com', 'pass123', 'Administrador'),
('aparedes', 'Andrea Paredes', 'aparedes@example.com', 'pass123', 'Empleado'),
('odominguez', 'Oscar Domínguez', 'odominguez@example.com', 'pass123', 'Administrador'),
('acarvajal', 'Alba Carvajal', 'acarvajal@example.com', 'pass123', 'Empleado'),
('jluna', 'Jesús Luna', 'jluna@example.com', 'pass123', 'Administrador'),
('mreyes', 'Martín Reyes', 'mreyes@example.com', 'pass123', 'Empleado'),
('bhernandez', 'Beatriz Hernández', 'bhernandez@example.com', 'pass123', 'Administrador'),
('fvaldez', 'Federico Valdez', 'fvaldez@example.com', 'pass123', 'Empleado'),
('mibarra', 'María Ibarra', 'mibarra@example.com', 'pass123', 'Administrador'),
('trojas', 'Tomás Rojas', 'trojas@example.com', 'pass123', 'Empleado'),
('operez', 'Olga Pérez', 'operez@example.com', 'pass123', 'Administrador'),
('mcastro', 'Mauricio Castro', 'mcastro@example.com', 'pass123', 'Empleado'),
('iselva', 'Inés Selva', 'iselva@example.com', 'pass123', 'Administrador'),
('rbenitez', 'Ramiro Benítez', 'rbenitez@example.com', 'pass123', 'Empleado'),
('nvalencia', 'Natalia Valencia', 'nvalencia@example.com', 'pass123', 'Administrador'),
('aguzman', 'Ángel Guzmán', 'aguzman@example.com', 'pass123', 'Empleado'),
('hlopez', 'Héctor López', 'hlopez@example.com', 'pass123', 'Administrador'),
('lrodriguez', 'Lucía Rodríguez', 'lrodriguez@example.com', 'pass123', 'Empleado'),
('cgutierrez', 'Camilo Gutiérrez', 'cgutierrez@example.com', 'pass123', 'Administrador'),
('fgarcia', 'Fabián García', 'fgarcia@example.com', 'pass123', 'Empleado'),
('amorales', 'Ariana Morales', 'amorales@example.com', 'pass123', 'Administrador'),
('aarroyo', 'Adela Arroyo', 'aarroyo@example.com', 'pass123', 'Empleado'),
('vvaldez', 'Valeria Valdez', 'vvaldez@example.com', 'pass123', 'Administrador'),
('tcastro', 'Tatiana Castro', 'tcastro@example.com', 'pass123', 'Empleado'),
('rrios', 'Ricardo Ríos', 'rrios@example.com', 'pass123', 'Administrador'),
('nrojas', 'Norma Rojas', 'nrojas@example.com', 'pass123', 'Empleado'),
('agonzalez', 'Alonso González', 'agonzalez@example.com', 'pass123', 'Administrador'),
('lperezg', 'Laura Pérez', 'lperezg@example.com', 'pass123', 'Empleado'),
('dnavarro', 'Daniel Navarro', 'dnavarro@example.com', 'pass123', 'Administrador'),
('agomez', 'Andrea Gómez', 'agomez@example.com', 'pass123', 'Empleado'),
('jjimenez', 'Joaquín Jiménez', 'jjimenez@example.com', 'pass123', 'Administrador');



GO--Detalle Equipos

INSERT INTO DetalleEquipo (EquipoID, UsuarioID) VALUES
(1, 1),  -- jlopez
(2, 2),  -- mgarcia
(3, 3),  -- cperez
(4, 4),  -- arodriquez
(5, 5),  -- fmartinez
(6, 6),  -- lhernandez
(7, 7),  -- dtorres
(8, 8),  -- mmorales
(9, 9),  -- rfernandez
(10, 10), -- psanchez
(11, 11), -- jramirez
(12, 12), -- cortega
(13, 13), -- gromero
(14, 14), -- pnavarro
(15, 15), -- nreyes
(16, 16), -- asantos
(17, 17), -- mruiz
(18, 18), -- hcastro
(19, 19), -- vibanez
(20, 20), -- egarcia
(1, 21),  -- rramos
(2, 22),  -- yfernandez
(3, 23),  -- jdiaz
(4, 24),  -- mprieto
(5, 25),  -- acastro
(6, 26),  -- frodriguez
(7, 27),  -- cbenitez
(8, 28),  -- rlopez
(9, 29),  -- jmartin
(10, 30), -- eluna
(11, 31), -- pmendoza
(12, 32), -- mcampos
(13, 33), -- aaguilar
(14, 34), -- dfernandez
(15, 35), -- eortiz
(16, 36), -- nmartinez
(17, 37), -- agarcia
(18, 38), -- gsantos
(19, 39), -- jtorres
(20, 40), -- mdominguez
(1, 41),  -- lperez
(2, 42),  -- emartin
(3, 43),  -- scarvajal
(4, 44),  -- jfuentes
(5, 45),  -- rsalazar
(6, 46),  -- mvaldez
(7, 47),  -- iflores
(8, 48),  -- lrojas
(9, 49),  -- yparedes
(10, 50), -- mvelasquez
(11, 51), -- pgomez
(12, 52), -- dmejia
(13, 53), -- eserrano
(14, 54), -- fguzman
(15, 55), -- rmendez
(16, 56), -- cjimenez
(17, 57), -- jrodriguez
(18, 58), -- eblanco
(19, 59), -- vcastillo
(20, 60), -- mgomez
(1, 61),  -- aparedes
(2, 62),  -- odominguez
(3, 63),  -- acarvajal
(4, 64),  -- jluna
(5, 65),  -- mreyes
(6, 66),  -- bhernandez
(7, 67),  -- fvaldez
(8, 68),  -- mibarra
(9, 69),  -- trojas
(10, 70), -- operez
(11, 71), -- mcastro
(12, 72), -- iselva
(13, 73), -- rbenitez
(14, 74), -- nvalencia
(15, 75), -- aguzman
(16, 76), -- hlopez
(17, 77), -- lrodriguez
(18, 78), -- cgutierrez
(19, 79), -- fgarcia
(20, 80), -- amorales
(1, 81),  -- aarroyo
(2, 82),  -- vvaldez
(3, 83),  -- tcastro
(4, 84),  -- rrios
(5, 85),  -- nrojas
(6, 86),  -- agonzalez
(7, 87),  -- lperezg
(8, 88),  -- dnavarro
(9, 89),  -- agomez
(10, 90); -- jjimenez

GO--Proyectos

INSERT INTO Proyecto (NombreProyecto, Descripcion, FechaInicio, FechaFin, Estado, EquipoID)
VALUES
('Sistema de Inventarios', 'Desarrollo de un sistema para gestionar el inventario de la empresa', '2025-01-15', '2025-04-30', 'En Progreso', 1),
('Aplicación de Ventas', 'App móvil para registrar y procesar ventas en tiempo real', '2025-02-01', '2025-06-15', 'En Progreso', 2),
('Rediseño Web Corporativa', 'Actualización del diseño y contenido del sitio web principal', '2025-03-10', '2025-05-25', 'Pendiente', 3),
('Sistema de Soporte Online', 'Implementación de chat y tickets para soporte técnico', '2025-01-20', '2025-03-30', 'Completado', 4),
('Plataforma de E-Learning', 'Desarrollo de una plataforma interna de capacitación', '2025-02-15', '2025-07-01', 'En Progreso', 5),
('Automatización de Procesos', 'Implementación de RPA para tareas repetitivas', '2025-03-01', '2025-06-20', 'Pendiente', 6),
('Seguridad en Red', 'Fortalecimiento de la seguridad en la infraestructura de red', '2025-01-10', '2025-03-15', 'Completado', 7),
('Control de Producción', 'Sistema para monitorear la producción en planta', '2025-02-25', '2025-05-10', 'En Progreso', 8),
('Aplicación de Mensajería Interna', 'Herramienta para comunicación segura entre empleados', '2025-03-05', '2025-06-30', 'Pendiente', 9),
('Integración con ERP', 'Conexión de sistemas internos con el ERP empresarial', '2025-01-18', '2025-04-15', 'En Progreso', 10);

GO--Tareas

INSERT INTO Tarea (Titulo, Descripcion, Estado, FechaInicio, FechaFin, ProyectoID, UsuarioID, Prioridad)
VALUES
-- Proyecto 1 (Usuarios del 1 al 10)
('Diseñar base de datos', 'Crear el modelo entidad-relación para el sistema de inventarios', 'En Progreso', '2025-01-15', '2025-01-20', 1, 3, 'Alta'),
('Configurar servidor', 'Instalar y configurar SQL Server para el sistema', 'Pendiente', '2025-01-18', '2025-01-25', 1, 9, 'Media'),
('Crear interfaz de login', 'Desarrollar formulario de inicio de sesión', 'En Progreso', '2025-01-20', '2025-01-25', 1, 6, 'Alta'),
('Pruebas de carga', 'Realizar pruebas de rendimiento en el sistema', 'Pendiente', '2025-02-01', '2025-02-05', 1, 10, 'Baja'),
('Documentar API', 'Escribir la documentación de los servicios del sistema', 'Finalizado', '2025-02-05', '2025-02-10', 1, 1, 'Media'),
('Diseño de reportes', 'Crear reportes de stock y movimientos', 'En Progreso', '2025-02-12', '2025-02-20', 1, 8, 'Alta'),
('Control de accesos', 'Configurar roles y permisos', 'Pendiente', '2025-02-15', '2025-02-18', 1, 5, 'Alta'),
('Migración de datos', 'Importar datos del inventario anterior', 'Finalizado', '2025-02-18', '2025-02-25', 1, 2, 'Alta'),
('Diseño de menú principal', 'Crear el menú de navegación de la aplicación', 'En Progreso', '2025-02-22', '2025-02-28', 1, 7, 'Media'),
('Validación de datos', 'Verificar integridad de los datos importados', 'Pendiente', '2025-03-01', '2025-03-03', 1, 4, 'Alta'),

-- Proyecto 2 (Usuarios del 11 al 20)
('Análisis de requerimientos', 'Reunir información sobre las necesidades de la app de ventas', 'En Progreso', '2025-02-01', '2025-02-05', 2, 17, 'Alta'),
('Diseñar mockups', 'Crear bocetos de la interfaz de la app', 'Pendiente', '2025-02-03', '2025-02-06', 2, 13, 'Media'),
('Integrar pasarela de pagos', 'Conectar app con sistema de pagos', 'En Progreso', '2025-02-06', '2025-02-12', 2, 11, 'Alta'),
('Pruebas de usabilidad', 'Testear experiencia de usuario', 'Pendiente', '2025-02-12', '2025-02-15', 2, 20, 'Baja'),
('Optimizar consultas SQL', 'Mejorar rendimiento de las consultas a la base de datos', 'Finalizado', '2025-02-15', '2025-02-17', 2, 15, 'Alta'),
('Implementar notificaciones', 'Configurar alertas en la app', 'En Progreso', '2025-02-18', '2025-02-22', 2, 18, 'Media'),
('Diseño del dashboard', 'Pantalla principal con métricas', 'Pendiente', '2025-02-20', '2025-02-25', 2, 12, 'Alta'),
('Desarrollo del carrito', 'Funcionalidad para agregar y quitar productos', 'En Progreso', '2025-02-22', '2025-02-28', 2, 16, 'Alta'),
('Implementar login con redes sociales', 'Permitir autenticación con Facebook y Google', 'Pendiente', '2025-02-25', '2025-03-01', 2, 14, 'Media'),
('Test de seguridad', 'Probar la seguridad de la aplicación', 'Pendiente', '2025-03-01', '2025-03-05', 2, 19, 'Alta'),

-- Proyecto 3 (Usuarios del 21 al 30)
('Revisión de contenido', 'Actualizar textos de la web corporativa', 'En Progreso', '2025-03-10', '2025-03-15', 3, 23, 'Media'),
('Optimización SEO', 'Mejorar posicionamiento en buscadores', 'Pendiente', '2025-03-12', '2025-03-20', 3, 28, 'Alta'),
('Diseño responsivo', 'Adaptar web para móviles y tabletas', 'En Progreso', '2025-03-15', '2025-03-22', 3, 21, 'Alta'),
('Implementar CMS', 'Instalar y configurar gestor de contenidos', 'Pendiente', '2025-03-18', '2025-03-25', 3, 26, 'Media'),
('Optimización de imágenes', 'Reducir tamaño de imágenes sin perder calidad', 'Finalizado', '2025-03-20', '2025-03-22', 3, 29, 'Baja'),
('Pruebas de compatibilidad', 'Comprobar visualización en diferentes navegadores', 'En Progreso', '2025-03-23', '2025-03-28', 3, 25, 'Media'),
('Integrar chat en línea', 'Agregar soporte por chat en la web', 'Pendiente', '2025-03-25', '2025-03-30', 3, 22, 'Alta'),
('Ajuste de colores corporativos', 'Aplicar nueva paleta de colores', 'En Progreso', '2025-03-28', '2025-04-02', 3, 30, 'Media'),
('Revisión de enlaces rotos', 'Comprobar y corregir enlaces que no funcionan', 'Pendiente', '2025-04-01', '2025-04-05', 3, 24, 'Alta'),
('Actualización de certificados SSL', 'Instalar nuevo certificado de seguridad', 'Pendiente', '2025-04-03', '2025-04-07', 3, 27, 'Alta'),

-- Proyecto 4 (Usuarios del 31 al 40)
('Diseño de dashboard', 'Interfaz principal con métricas clave', 'En Progreso', '2025-01-18', '2025-01-28', 4, 38, 'Alta'),
('Automatización de tareas', 'Usar scripts para reducir tareas manuales', 'Finalizado', '2025-01-09', '2025-01-19', 4, 32, 'Media'),
('Implementación de backup', 'Configurar copias de seguridad automáticas', 'Pendiente', '2025-02-03', '2025-02-13', 4, 35, 'Alta'),
('Pruebas funcionales', 'Validar que todas las funciones trabajen como se espera', 'En Progreso', '2025-02-07', '2025-02-17', 4, 31, 'Baja'),
('Optimización de rendimiento', 'Ajustar configuraciones para mayor velocidad', 'Pendiente', '2025-02-09', '2025-02-19', 4, 39, 'Media'),

-- Proyecto 5 (Usuarios del 41 al 50)
('Diseño de módulo de notificaciones', 'Crear sistema de alertas y recordatorios', 'En Progreso', '2025-01-20', '2025-01-30', 5, 43, 'Alta'),
('Integración con correo electrónico', 'Permitir notificaciones vía email', 'Finalizado', '2025-01-11', '2025-01-21', 5, 48, 'Media'),
('Pruebas de integración', 'Verificar interacción entre módulos', 'Pendiente', '2025-02-05', '2025-02-15', 5, 41, 'Alta'),
('Optimización del frontend', 'Mejorar velocidad de carga de la interfaz', 'En Progreso', '2025-02-08', '2025-02-18', 5, 50, 'Baja'),
('Creación de plantillas', 'Diseñar plantillas predeterminadas para reportes', 'Pendiente', '2025-02-10', '2025-02-20', 5, 45, 'Media'),

-- Proyecto 6 (Usuarios del 51 al 58)
('Diseño de módulo de permisos', 'Configurar niveles de acceso para usuarios', 'En Progreso', '2025-01-22', '2025-02-01', 6, 54, 'Alta'),
('Auditoría de seguridad', 'Registrar y revisar eventos importantes del sistema', 'Finalizado', '2025-01-13', '2025-01-23', 6, 57, 'Media'),
('Implementación de logs', 'Guardar historial de acciones de usuarios', 'Pendiente', '2025-02-06', '2025-02-16', 6, 51, 'Alta'),
('Optimización de base de datos', 'Ajustar índices y consultas para mayor rendimiento', 'En Progreso', '2025-02-09', '2025-02-19', 6, 58, 'Baja'),
('Creación de API interna', 'Desarrollar API para uso interno de la empresa', 'Pendiente', '2025-02-11', '2025-02-21', 6, 55, 'Media'),

-- Proyecto 7 (Usuarios del 59 al 66)
('Diseño del módulo de facturación', 'Crear sistema para generar y registrar facturas', 'En Progreso', '2025-01-24', '2025-02-03', 7, 62, 'Alta'),
('Integración con pasarela de pagos', 'Permitir pagos en línea con distintos métodos', 'Finalizado', '2025-01-14', '2025-01-24', 7, 65, 'Media'),
('Pruebas unitarias del sistema de cobros', 'Verificar funciones clave del módulo de pagos', 'Pendiente', '2025-02-07', '2025-02-17', 7, 59, 'Alta'),
('Optimización de reportes financieros', 'Mejorar tiempos de generación de reportes', 'En Progreso', '2025-02-10', '2025-02-20', 7, 66, 'Baja'),
('Creación de panel de control de ventas', 'Dashboard para métricas de ingresos y ventas', 'Pendiente', '2025-02-12', '2025-02-22', 7, 61, 'Media'),

-- Proyecto 8 (Usuarios del 67 al 74)
('Diseño del módulo de inventario', 'Control y gestión de stock en tiempo real', 'En Progreso', '2025-01-25', '2025-02-04', 8, 70, 'Alta'),
('Implementación de lector de código de barras', 'Integrar dispositivos de escaneo para productos', 'Finalizado', '2025-01-15', '2025-01-25', 8, 73, 'Media'),
('Pruebas de control de inventario', 'Verificar exactitud en las cantidades de stock', 'Pendiente', '2025-02-08', '2025-02-18', 8, 67, 'Alta'),
('Optimización de almacenamiento', 'Mejorar estructura de datos para consultas rápidas', 'En Progreso', '2025-02-11', '2025-02-21', 8, 74, 'Baja'),
('Creación de alertas de stock bajo', 'Notificaciones automáticas cuando falte inventario', 'Pendiente', '2025-02-13', '2025-02-23', 8, 69, 'Media'),

-- Proyecto 9 (Usuarios del 75 al 82)
('Diseño de módulo de soporte técnico', 'Plataforma para atención de incidencias', 'En Progreso', '2025-01-26', '2025-02-05', 9, 78, 'Alta'),
('Integración con chat en línea', 'Canal de comunicación directa con clientes', 'Finalizado', '2025-01-16', '2025-01-26', 9, 81, 'Media'),
('Pruebas de flujo de tickets', 'Asegurar que las solicitudes se gestionen correctamente', 'Pendiente', '2025-02-09', '2025-02-19', 9, 75, 'Alta'),
('Optimización del panel de soporte', 'Reducir tiempos de carga de tickets', 'En Progreso', '2025-02-12', '2025-02-22', 9, 82, 'Baja'),
('Creación de base de conocimientos', 'Repositorio de soluciones a problemas comunes', 'Pendiente', '2025-02-14', '2025-02-24', 9, 77, 'Media'),

-- Proyecto 10 (Usuarios del 83 al 90)
('Diseño de interfaz de usuario', 'Crear un diseño atractivo y funcional para la aplicación', 'En Progreso', '2025-01-27', '2025-02-06', 10, 85, 'Alta'),
('Implementación de autenticación', 'Desarrollar sistema de inicio de sesión seguro', 'Finalizado', '2025-01-17', '2025-01-27', 10, 89, 'Media'),
('Integración con API externa', 'Conectar el sistema con servicio de terceros', 'Pendiente', '2025-02-08', '2025-02-18', 10, 83, 'Alta'),
('Pruebas de rendimiento', 'Medir tiempos de respuesta y optimizar código', 'En Progreso', '2025-02-11', '2025-02-21', 10, 90, 'Baja'),
('Optimización de consultas SQL', 'Reducir tiempos de ejecución en base de datos', 'Pendiente', '2025-02-13', '2025-02-23', 10, 87, 'Media'),
('Diseño de módulo de reportes', 'Generar reportes personalizados para usuarios', 'En Progreso', '2025-01-28', '2025-02-07', 10, 84, 'Alta'),
('Implementación de notificaciones', 'Enviar alertas por correo y dentro de la app', 'Finalizado', '2025-01-18', '2025-01-28', 10, 88, 'Media'),
('Pruebas de integración', 'Asegurar que todos los módulos trabajen en conjunto', 'Pendiente', '2025-02-09', '2025-02-19', 10, 86, 'Alta'),
('Documentación del sistema', 'Crear manuales y guías de uso para usuarios y técnicos', 'En Progreso', '2025-02-12', '2025-02-22', 10, 90, 'Baja'),
('Despliegue en servidor de producción', 'Lanzar la aplicación en el entorno final', 'Pendiente', '2025-02-14', '2025-02-24', 10, 83, 'Media');


GO --Comentarios

INSERT INTO Comentario (Contenido, UsuarioID, TareaID)
VALUES
-- Comentarios para el Proyecto 1 (Usuarios 1-10)
('Revisé el diseño inicial y tiene buena estructura.', 4, 1),
('Agregué algunos cambios en los colores y tipografía.', 9, 1),
('El módulo de autenticación ya está implementado.', 7, 2),
('Se corrigieron errores menores en la validación.', 1, 2),
('Se inició la conexión con la API externa.', 5, 3),
('API responde correctamente, falta manejo de errores.', 10, 3),
('Pruebas de carga realizadas, se detectaron cuellos de botella.', 2, 4),
('Optimizaciones aplicadas, rendimiento mejoró un 20%.', 8, 4),
('Consultas SQL optimizadas en el módulo de reportes.', 6, 5),
('Se redujo el tiempo de ejecución de 5s a 2s.', 3, 5),
('Diseño del módulo de reportes finalizado.', 1, 6),
('Se añadieron gráficos y filtros en el reporte.', 5, 6),
('Sistema de notificaciones ya envía alertas por correo.', 9, 7),
('Notificaciones internas también implementadas.', 2, 7),
('Pruebas de integración iniciadas con todos los módulos.', 8, 8),
('Se detectaron fallos menores en la integración.', 4, 8),
('Documentación técnica en progreso.', 6, 9),
('Manual de usuario finalizado y revisado.', 10, 9),
('Despliegue en servidor de pruebas realizado.', 3, 10),
('Ajustes en configuración antes del despliegue final.', 7, 10),

-- Comentarios para el Proyecto 2 (Usuarios 11-20)
('El código fuente fue revisado y aprobado.', 14, 11),
('Se añadieron funciones para exportar datos.', 11, 12),
('Revisión de interfaz móvil completada.', 18, 13),
('Integración con servicio de mapas lista.', 12, 14),
('Pruebas unitarias ejecutadas con éxito.', 15, 15),
('Se mejoró la seguridad en el acceso de datos.', 19, 16),
('Optimización de imágenes para carga rápida.', 13, 17),
('Actualización de dependencias realizada.', 16, 18),
('Feedback recibido del cliente, se aplicarán cambios.', 17, 19),
('El despliegue final fue exitoso.', 20, 20),

-- Comentarios para el Proyecto 3 (Usuarios 21-30)
('El informe financiero fue cargado correctamente.', 25, 21),
('Se cambió el formato de exportación a PDF.', 28, 22),
('Faltan pruebas en navegadores antiguos.', 21, 23),
('Se solucionó el error de compatibilidad con Safari.', 26, 24),
('Base de datos migrada sin pérdida de datos.', 29, 25),
('Se agregaron índices para mejorar rendimiento.', 22, 26),
('Pruebas de seguridad superadas con éxito.', 30, 27),
('El sistema ahora soporta múltiples idiomas.', 24, 28),
('Se añadió autenticación de dos factores.', 23, 29),
('Se actualizó el logo en todas las páginas.', 27, 30),

-- Comentarios para el Proyecto 4 (Usuarios 31-40)
('Error en la carga de imágenes corregido.', 35, 31),
('La API ahora soporta peticiones asincrónicas.', 32, 32),
('Se incorporó paginación en el listado de clientes.', 39, 33),
('Funcionalidad de búsqueda avanzada implementada.', 31, 34),
('Se mejoró el sistema de caché para datos estáticos.', 38, 35),

-- Comentarios para el Proyecto 5 (Usuarios 41-50)
('La interfaz de usuario fue rediseñada.', 43, 36),
('Actualización automática implementada.', 48, 37),
('Reportes de errores enviados a soporte.', 41, 38),
('Se reemplazó el servidor por uno más potente.', 50, 39),
('Pruebas con usuarios reales completadas.', 45, 40),
('Se creó un nuevo endpoint para la integración móvil.', 42, 41),
('Las plantillas de reportes ya son dinámicas.', 47, 42),
('Se optimizó una consulta que tardaba 10 segundos.', 46, 43),
('La documentación de la beta ya está en línea.', 49, 44),
('Se ha duplicado el espacio de almacenamiento en la nube.', 44, 45),

-- Comentarios para el Proyecto 6 (Usuarios 51-58)
('Correcciones menores en interfaz de administración.', 54, 46),
('El módulo de pagos ya acepta nuevas tarjetas.', 57, 47),
('Se agregaron logs detallados de actividad.', 51, 48),
('Implementación de chatbot para soporte básico.', 58, 49),
('Se mejoró el tiempo de respuesta del servidor.', 55, 50),
('El módulo de permisos ya permite roles personalizados.', 52, 51),
('Se ha completado la auditoría interna de seguridad.', 56, 52),
('Los logs ahora se guardan en un bucket de almacenamiento externo.', 53, 53),
('Se han agregado índices a las tablas más grandes.', 51, 54),
('La API interna ha sido desplegada en producción.', 57, 55),
('Se ajustó el algoritmo de encriptación para mayor seguridad.', 54, 56),
('Pruebas de estrés en la API interna completadas.', 58, 57),
('Se documentó la nueva API interna para el equipo de desarrollo.', 53, 58),
('Funcionalidad de registro de errores implementada.', 51, 59),
('Se ha programado una tarea de limpieza de logs semanal.', 56, 60),

-- Comentarios para el Proyecto 7 (Usuarios 59-66)
('El diseño del módulo de facturación está listo para desarrollo.', 62, 61),
('Se integró la pasarela de pagos de PayPal.', 65, 62),
('Pruebas unitarias para el cálculo de impuestos completadas.', 59, 63),
('El reporte financiero ahora se genera en formato Excel y PDF.', 66, 64),
('El dashboard de ventas muestra métricas en tiempo real.', 61, 65),
('Se han agregado validaciones en el formulario de facturación.', 64, 66),
('El webhook de la pasarela de pagos ya está configurado.', 60, 67),
('Pruebas de integración con el módulo de inventario iniciadas.', 63, 68),
('Se mejoró la legibilidad de los reportes financieros.', 66, 69),
('Se añadió un filtro por fecha al dashboard de ventas.', 61, 70);
