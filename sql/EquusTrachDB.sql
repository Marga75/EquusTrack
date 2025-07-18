-- Crear base de datos
DROP DATABASE IF EXISTS EquusTrackDB;
CREATE DATABASE EquusTrackDB;
USE EquusTrackDB;

-- Tabla Usuarios
CREATE TABLE Usuarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Rol ENUM('Jinete', 'Entrenador', 'Administrador') NOT NULL,
    FechaNacimiento DATE,
    Genero ENUM('Masculino', 'Femenino', 'Otro'),
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FotoUrl TEXT
);

-- Tabla Caballos
CREATE TABLE Caballos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100),
    Raza VARCHAR(100),
    Color VARCHAR(100),
    FotoUrl TEXT,
    IdUsuario INT,
    IdEntrenador INT NULL,
    FechaNacimiento DATE,
    FechaAdopcion DATE,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id) ON DELETE CASCADE,
    FOREIGN KEY (IdEntrenador) REFERENCES Usuarios(Id) ON DELETE SET NULL
);

-- Tabla Entrenamientos
CREATE TABLE Entrenamientos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Titulo VARCHAR(100) NOT NULL,
    Tipo ENUM('PieATierra', 'Montado', 'Jinete') NOT NULL,
    Descripcion TEXT,
    Imagen VARCHAR(255)
);

-- Tabla EjerciciosEntrenamiento
CREATE TABLE EjerciciosEntrenamiento (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    EntrenamientoId INT NOT NULL,
    Orden INT NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion TEXT,
    DuracionSegundos INT,
    Repeticiones INT,
    TipoBloque ENUM('Calentamiento', 'Principal', 'VueltaCalma') DEFAULT 'Principal',
    ImagenURL VARCHAR(255),
    FOREIGN KEY (EntrenamientoId) REFERENCES Entrenamientos(Id) ON DELETE CASCADE
);

-- Tabla RegistroEntrenamientos
CREATE TABLE RegistroEntrenamientos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCaballo INT,
    IdEntrenamiento INT NOT NULL,
    Fecha DATE NOT NULL,
    Notas TEXT,
    Progreso INT,
    RegistradoPorId INT,
    Estado ENUM('Completado', 'Incompleto', 'Cancelado') DEFAULT 'Completado',
    FOREIGN KEY (IdCaballo) REFERENCES Caballos(Id) ON DELETE SET NULL,
    FOREIGN KEY (IdEntrenamiento) REFERENCES Entrenamientos(Id) ON DELETE CASCADE,
    FOREIGN KEY (RegistradoPorId) REFERENCES Usuarios(Id) ON DELETE SET NULL
);

-- Tabla Veterinario
CREATE TABLE Veterinario (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCaballo INT,
    Fecha DATE,
    Descripcion TEXT,
    VeterinarioNombre VARCHAR(100),
    Costo DECIMAL(10,2),
    FOREIGN KEY (IdCaballo) REFERENCES Caballos(Id) ON DELETE CASCADE
);

-- Tabla Herrador
CREATE TABLE Herrador (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCaballo INT,
    Fecha DATE,
    ProximaHerrada DATE,
    HerradorNombre VARCHAR(100),
    Costo DECIMAL(10,2),
    FOREIGN KEY (IdCaballo) REFERENCES Caballos(Id) ON DELETE CASCADE
);

-- Tabla Fisioterapia
CREATE TABLE Fisioterapia (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCaballo INT,
    Fecha DATE,
    Descripcion TEXT,
    Profesional VARCHAR(100),
    Costo DECIMAL(10,2),
    FOREIGN KEY (IdCaballo) REFERENCES Caballos(Id) ON DELETE CASCADE
);

-- Tabla RelEntrenadorAlumno
CREATE TABLE RelEntrenadorAlumno (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdEntrenador INT,
    IdAlumno INT,
    Estado ENUM('pendiente', 'aceptado', 'rechazado') NOT NULL DEFAULT 'pendiente',
    FechaSolicitud DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (IdEntrenador, IdAlumno),
    FOREIGN KEY (IdEntrenador) REFERENCES Usuarios(Id),
    FOREIGN KEY (IdAlumno) REFERENCES Usuarios(Id)
);
