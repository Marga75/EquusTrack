-- Crear base de datos
DROP DATABASE IF EXISTS EquusTrackDB;
CREATE DATABASE EquusTrackDB;
USE EquusTrackDB;

-- Tabla usuarios
CREATE TABLE Usuarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Rol ENUM('Jinete', 'Entrenador', 'Administrador') NOT NULL,
    FechaNacimiento DATE,
    Genero ENUM('Masculino', 'Femenino', 'Otro'),
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP
);


-- Tabla caballos
CREATE TABLE Caballos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100),
    Edad INT,
    Raza VARCHAR(100),
    Color VARCHAR(100),
    FotoUrl TEXT,
    IdUsuario INT,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id) ON DELETE CASCADE
);

-- Tabla entrenamientos
CREATE TABLE Entrenamientos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Titulo VARCHAR(100) NOT NULL,
    Tipo ENUM('PieATierra', 'Montado', 'Jinete') NOT NULL,
    Descripcion TEXT
);

-- Tabla ejercicios_entrenamiento
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

-- Tabla registro_entrenamientos
CREATE TABLE RegistroEntrenamientos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCaballo INT NOT NULL,
    IdEntrenamiento INT NOT NULL,
    Fecha DATE NOT NULL,
    Notas TEXT,
    Progreso INT,
    RegistradoPorId INT,
    Estado ENUM('Completado', 'Incompleto', 'Cancelado') DEFAULT 'Completado',
    FOREIGN KEY (IdCaballo) REFERENCES Caballos(Id) ON DELETE CASCADE,
    FOREIGN KEY (IdEntrenamiento) REFERENCES Entrenamientos(Id) ON DELETE CASCADE,
    FOREIGN KEY (RegistradoPorId) REFERENCES Usuarios(Id) ON DELETE SET NULL
);

-- Tabla veterinario
CREATE TABLE Veterinario (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCaballo INT,
    Fecha DATE,
    Descripcion TEXT,
    VeterinarioNombre VARCHAR(100),
    Costo DECIMAL(10,2),
    FOREIGN KEY (IdCaballo) REFERENCES Caballos(Id) ON DELETE CASCADE
);

-- Tabla herrador
CREATE TABLE Herrador (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCaballo INT,
    Fecha DATE,
    ProximaHerrada DATE,
    HerradorNombre VARCHAR(100),
    Costo DECIMAL(10,2),
    FOREIGN KEY (IdCaballo) REFERENCES Caballos(Id) ON DELETE CASCADE
);

-- Tabla fisio
CREATE TABLE Fisioterapia (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCaballo INT,
    Fecha DATE,
    Descripcion TEXT,
    Profesional VARCHAR(100),
    Costo DECIMAL(10,2),
    FOREIGN KEY (IdCaballo) REFERENCES Caballos(Id) ON DELETE CASCADE
);

-- Tabla relación entrenador - alumno
CREATE TABLE RelEntrenadorAlumno (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdEntrenador INT,
    IdAlumno INT,
    FOREIGN KEY (IdEntrenador) REFERENCES Usuarios(Id),
    FOREIGN KEY (IdAlumno) REFERENCES Usuarios(Id),
    UNIQUE (IdEntrenador, IdAlumno)
);

-- Añadir IdEntrenador en Caballos
ALTER TABLE Caballos 
ADD COLUMN IdEntrenador INT NULL;

ALTER TABLE Caballos 
ADD CONSTRAINT FK_Caballos_Entrenador FOREIGN KEY (IdEntrenador) REFERENCES Usuarios(Id) ON DELETE SET NULL;

-- Quitar columna Edad
ALTER TABLE Caballos
DROP COLUMN Edad;

-- Añadir FechaNacimiento y FechaAdopcion
ALTER TABLE Caballos
ADD COLUMN FechaNacimiento DATE,
ADD COLUMN FechaAdopcion DATE;

-- Añadir Estado y FechaSolicitud
ALTER TABLE RelEntrenadorAlumno
ADD COLUMN Estado ENUM('pendiente', 'aceptado', 'rechazado') NOT NULL DEFAULT 'pendiente',
ADD COLUMN FechaSolicitud DATETIME DEFAULT CURRENT_TIMESTAMP;

-- Añadir imagen en usuario
ALTER TABLE usuarios
ADD COLUMN FotoUrl TEXT;