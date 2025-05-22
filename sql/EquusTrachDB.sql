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
    Titulo VARCHAR(100),
    Tipo ENUM('PieATierra', 'Montado', 'Jinete'),
    Descripcion TEXT,
    EsPredefinido BOOLEAN DEFAULT FALSE,
    CreadorId INT, -- NULL si es predefinido
    FOREIGN KEY (CreadorId) REFERENCES Usuarios(Id) ON DELETE SET NULL
);

-- Tabla registro_entrenamientos
CREATE TABLE HistorialEntrenamientos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdCaballo INT,
    IdEntrenamiento INT,
    Fecha DATE,
    Notas TEXT,
    Progreso INT, -- porcentaje o puntuación de seguimiento
    FOREIGN KEY (IdCaballo) REFERENCES Caballos(Id) ON DELETE CASCADE,
    FOREIGN KEY (IdEntrenamiento) REFERENCES Entrenamientos(Id) ON DELETE CASCADE
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

-- Tabla progreso
/* CREATE TABLE progreso (
    id INT AUTO_INCREMENT PRIMARY KEY,
    caballo_id INT,
    fecha DATE,
    rendimiento DECIMAL(5,2),
    notas TEXT,
    FOREIGN KEY (caballo_id) REFERENCES caballos(id)
); */

-- Tabla relación entrenador - alumno
CREATE TABLE RelEntrenadorAlumno (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdEntrenador INT,
    IdAlumno INT,
    FOREIGN KEY (IdEntrenador) REFERENCES Usuarios(Id),
    FOREIGN KEY (IdAlumno) REFERENCES Usuarios(Id),
    UNIQUE (IdEntrenador, IdAlumno)
);

-- Tabla recomendaciones de entrenamiento
CREATE TABLE Recomendaciones (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdEntrenador INT,
    IdAlumno INT,
    IdEntrenamiento INT,
    Fecha DATE,
    Comentario TEXT,
    FOREIGN KEY (IdEntrenador) REFERENCES Usuarios(Id),
    FOREIGN KEY (IdAlumno) REFERENCES Usuarios(Id),
    FOREIGN KEY (IdEntrenamiento) REFERENCES Entrenamientos(Id)
);

-- Añadir IdEntrenador en Caballos
ALTER TABLE Caballos 
ADD COLUMN IdEntrenador INT NULL;

ALTER TABLE Caballos 
ADD CONSTRAINT FK_Caballos_Entrenador FOREIGN KEY (IdEntrenador) REFERENCES Usuarios(Id) ON DELETE SET NULL;
