-- Insertar 1 entrenammiento
insert into entrenamientos (Titulo, Tipo, Descripcion)
values ('Resistencia Básica', 'PieATierra', 'Entrenamiento ideal para mejorar la resistencia general del caballo.');

set @entrenamientoId1 = last_insert_id();

-- Insertar ejercicios para Resistencia Básica
INSERT INTO EjerciciosEntrenamiento (EntrenamientoId, Orden, Nombre, Descripcion, DuracionSegundos, TipoBloque, ImagenURL)
VALUES 
(@EntrenamientoId1, 1, 'Paso prolongado', 'Camina a paso lento pero constante.', 300, 'Calentamiento', 'https://blog.equippos.com/wp-content/uploads/pessoa-sistem-1.jpg'),
(@EntrenamientoId1, 2, 'Trote en círculo', 'Mantén el trote en círculos amplios.', 600, 'Principal', 'https://blog.equippos.com/wp-content/uploads/How-to-lunge-your-horse-how-to-dressage.jpg'),
(@EntrenamientoId1, 3, 'Paso relajado', 'Relaja y camina tranquilamente para bajar ritmo.', 300, 'VueltaCalma', 'https://blog.equippos.com/wp-content/uploads/Cuerda-suave-para-trabajo-a-la-cuerda-1.jpg');

-- Insertar Entrenamiento 2: Técnica de Salto
INSERT INTO Entrenamientos (Titulo, Tipo, Descripcion)
VALUES ('Técnica de Salto', 'Montado', 'Técnica enfocada en mejorar los saltos con obstáculos.');

SET @EntrenamientoId2 = LAST_INSERT_ID();

-- Insertar ejercicios para Técnica de Salto
INSERT INTO EjerciciosEntrenamiento (EntrenamientoId, Orden, Nombre, Descripcion, DuracionSegundos, TipoBloque, ImagenURL)
VALUES 
(@EntrenamientoId2, 1, 'Calentamiento paso y trote', 'Calienta con paso y trote ligero.', 300, 'Calentamiento', 'https://ophena.com/cdn/shop/articles/IMG_4641_1_d2179386-f6b2-4cda-85b7-d92f2a8cfbcc.jpg'),
(@EntrenamientoId2, 2, 'Saltos en línea', 'Practica saltos consecutivos.', 600, 'Principal', 'https://www.ecuestre.es/contenidos/imagenes/articulo/5e8dba768e5ff0175a263c69/1586356658069-gimnasia-del-caballo-de-saltos-ii-laboratorios-circulos-calles-y-recorridos.jpg'),
(@EntrenamientoId2, 3, 'Vuelta a la calma', 'Relaja el caballo con paso suave.', 300, 'VueltaCalma', 'https://www.equisens.es/wp-content/uploads/2018/08/equisens-doma-1024x683.jpg');

-- Insertar Entrenamiento 3: Core y equilibrio del jinete
INSERT INTO Entrenamientos (Titulo, Tipo, Descripcion)
VALUES ('Core y equilibrio del jinete', 'Jinete', 'Sesión centrada en fortalecer el core y mejorar el equilibrio del jinete sin usar al caballo.');

SET @EntrenamientoId3 = LAST_INSERT_ID();

-- Insertar ejercicios para Core y equilibrio del jinete
INSERT INTO EjerciciosEntrenamiento (EntrenamientoId, Orden, Nombre, Descripcion, DuracionSegundos, TipoBloque, ImagenURL)
VALUES 
(@EntrenamientoId3, 1, 'Estiramientos dinámicos', 'Moviliza las articulaciones con movimientos suaves para preparar el cuerpo.', 300, 'Calentamiento', 'https://fisiolution.com/wp-content/uploads/2021/07/Foto-2-J.L-OK.png'),
(@EntrenamientoId3, 2, 'Plancha frontal', 'Fortalece el core manteniendo la posición de plancha.', 60, 'Principal', 'https://www.boteprote.com/blog/wp-content/uploads/2019/08/core-para-atletas-plancha-frontal.jpg'),
(@EntrenamientoId3, 3, 'Sentadillas con salto', 'Mejora fuerza y coordinación con sentadillas explosivas.', 120, 'Principal', 'https://static.vecteezy.com/system/resources/previews/008/635/521/non_2x/woman-doing-jump-squats-exercise-flat-illustration-isolated-on-white-background-vector.jpg'),
(@EntrenamientoId3, 4, 'Equilibrio sobre una pierna', 'Sostén el equilibrio alternando piernas, mejorando estabilidad.', 90, 'Principal', 'https://www.fisioserv.com/wp-content/uploads/2023/06/equilibrio-sobre-bosu.png'),
(@EntrenamientoId3, 5, 'Respiración profunda', 'Recupera y relaja el cuerpo con respiraciones controladas.', 150, 'VueltaCalma', 'https://surferrule-media-495167.s3-accelerate.amazonaws.com/2019/12/v4-728px-Hold-Your-Breath-for-Long-Periods-of-Time-Step-1-Version-3.jpg');


UPDATE Entrenamientos
SET Imagen = 'https://www.equipassio.com/66637-large_default/cuerda-para-dar-cuerda-de-qhp.jpg'
WHERE Id = 1;

UPDATE Entrenamientos
SET Imagen = 'https://www.ecuestre.es/contenidos/imagenes/articulo/60758cc88e5ff059ed4e6771/1618316897202-pista-de-ensayo-2-lo-que-debemos-buscar-en-el-precalentamiento.jpg'
WHERE Id = 2;

UPDATE Entrenamientos
SET Imagen = 'https://www.ecuestre.es/upload/images/article/146748/article-ejercicio-n10-equilibrio-sobre-fitball-59bbff74a2ddd.jpg'
WHERE Id = 3;
