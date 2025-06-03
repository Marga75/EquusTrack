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

-- Entrenamiento 4: Trabajo en pista de doma
INSERT INTO Entrenamientos (Titulo, Tipo, Descripcion, Imagen)
VALUES ('Trabajo en pista de doma', 'Montado', 'Ejercicios para mejorar la precisión y fluidez en la pista de doma.', 'https://www.robertogarrudo.com/blog/wp-content/uploads/2014/06/doma-clasica-ferrer-salat.jpg');

SET @EntrenamientoId4 = LAST_INSERT_ID();

INSERT INTO EjerciciosEntrenamiento (EntrenamientoId, Orden, Nombre, Descripcion, DuracionSegundos, TipoBloque, ImagenURL)
VALUES
(@EntrenamientoId4, 1, 'Calentamiento en paso', 'Camina el caballo con pasos largos y relajados.', 300, 'Calentamiento', 'https://complejoecuestrealfredoperez.com/wp-content/uploads/2023/05/Entrenamiento-de-Caballos-de-Salto-en-Moraleja-Mejora-su-Rendimiento-y-Habilidades.jpg'),
(@EntrenamientoId4, 2, 'Transiciones paso-trote', 'Practica cambios suaves entre paso y trote.', 600, 'Principal', 'https://galopedigital.com/wp-content/uploads/2021/02/Pasagge-principal.jpg'),
(@EntrenamientoId4, 3, 'Ejercicio de círculos', 'Realiza círculos de distintos diámetros para mejorar control.', 400, 'Principal', 'https://www.ecuestre.es/contenidos/imagenes/articulo/5e8dba768e5ff0175a263c69/1586356685027-gimnasia-del-caballo-de-saltos-ii-laboratorios-circulos-calles-y-recorridos.jpg'),
(@EntrenamientoId4, 4, 'Vuelta a la calma al paso', 'Relaja al caballo con paso tranquilo.', 300, 'VueltaCalma', 'https://www.laequitacion.com/attachments/avatar-trote-jpg.6132/');

-- Entrenamiento 5: Mejorar la velocidad en galopada
INSERT INTO Entrenamientos (Titulo, Tipo, Descripcion, Imagen)
VALUES ('Mejorar la velocidad en galopada', 'Montado', 'Entrenamiento para aumentar la velocidad y control en galopadas.', 'https://www.ecuestre.es/upload/images/article/147475/article-resolver-problemas-al-galope-5a43e3386495d.jpg');

SET @EntrenamientoId5 = LAST_INSERT_ID();

INSERT INTO EjerciciosEntrenamiento (EntrenamientoId, Orden, Nombre, Descripcion, DuracionSegundos, TipoBloque, ImagenURL)
VALUES
(@EntrenamientoId5, 1, 'Calentamiento al paso', 'Camina el caballo relajado para preparar.', 300, 'Calentamiento', 'https://www.segurosequitacion.com/wp-content/uploads/2022/03/Imagen1-770x540.jpg'),
(@EntrenamientoId5, 2, 'Aumentos progresivos de galopada', 'Incrementa la velocidad de galopada en intervalos.', 600, 'Principal', 'https://www.equisens.es/wp-content/uploads/2018/11/Secuencia-galope-con-caballo-e1542736846827.jpg'),
(@EntrenamientoId5, 3, 'Galopada controlada en círculo', 'Controla la velocidad en círculos amplios.', 400, 'Principal', 'https://www.ecuestre.es/upload/images/gallery/5bec20300ce6949f5d8b4708/5bec28160ce6946d608b46f8-resolver-problemas-al-galope.jpg'),
(@EntrenamientoId5, 4, 'Vuelta a la calma', 'Camina suavemente para recuperar.', 300, 'VueltaCalma', 'https://a.storyblok.com/f/183484/8334x4146/6f8f102c6d/home-nov2-femme.jpg');

-- Entrenamiento 6: Flexibilidad y movilidad básica
INSERT INTO Entrenamientos (Titulo, Tipo, Descripcion, Imagen)
VALUES ('Flexibilidad y movilidad básica', 'PieATierra', 'Ejercicios para mejorar la flexibilidad y movilidad del caballo.', 'https://www.a-alvarez.com/img/ybc_blog/post/47554_-_photo_1_1472806883_img.jpg');

SET @EntrenamientoId6 = LAST_INSERT_ID();

INSERT INTO EjerciciosEntrenamiento (EntrenamientoId, Orden, Nombre, Descripcion, DuracionSegundos, TipoBloque, ImagenURL)
VALUES
(@EntrenamientoId6, 1, 'Estiramientos de cuello', 'Estira suavemente el cuello del caballo a ambos lados.', 300, 'Calentamiento', 'https://www.ecuestre.es/contenidos/imagenes/articulo/60b675a78e5ff059ed326b69/1622571014658-fisioterapia-practica-estiramientos-de-cuello.jpg'),
(@EntrenamientoId6, 2, 'Movilidad de articulaciones', 'Realiza movimientos circulares con las patas.', 400, 'Principal', 'https://myhorsebackview.com/wp-content/uploads/2018/08/IMG-20180724-WA0037-1024x681.jpg'),
(@EntrenamientoId6, 3, 'Marcha lateral pie a tierra', 'Camina el caballo lateralmente con ayuda.', 600, 'Principal', 'https://www.equisens.es/wp-content/uploads/2018/06/Schouderbinnen-1024x576.jpeg'),
(@EntrenamientoId6, 4, 'Relajación', 'Deja al caballo caminar libremente.', 300, 'VueltaCalma', 'https://img.freepik.com/foto-gratis/foto-espalda-mujer-joven-uniforme-especial-casco-su-caballo-montar_496169-175.jpg?semt=ais_hybrid&w=740');

-- Entrenamiento 7: Control y respuesta pie a tierra
INSERT INTO Entrenamientos (Titulo, Tipo, Descripcion, Imagen)
VALUES ('Control y respuesta pie a tierra', 'PieATierra', 'Entrenamiento para mejorar la comunicación y control del caballo sin montar.', 'https://www.equisens.es/wp-content/uploads/2019/11/portada.jpeg');

SET @EntrenamientoId7 = LAST_INSERT_ID();

INSERT INTO EjerciciosEntrenamiento (EntrenamientoId, Orden, Nombre, Descripcion, DuracionSegundos, TipoBloque, ImagenURL)
VALUES
(@EntrenamientoId7, 1, 'Ejercicios de parada y avance', 'Practica detener y avanzar al caballo con comandos.', 400, 'Calentamiento', 'https://www.equisens.es/wp-content/uploads/2019/11/1a.jpg'),
(@EntrenamientoId7, 2, 'Caminar en línea recta', 'Conduce al caballo en línea recta, mejorando atención.', 600, 'Principal', 'https://blog.sentimientoycompas.es/wp-content/uploads/2021/11/Dar-cuerda-a-un-caballo.jpg'),
(@EntrenamientoId7, 3, 'Virajes y cambios de dirección', 'Ejercita cambios de dirección rápidos.', 400, 'Principal', 'https://www.ecuestre.es/contenidos/imagenes/articulo/63b030d88e5ff076810f769f/1672491492398-trabajo-a-la-cuerda-diferente-y-complementario-al-que-se-hace-montado.jpg'),
(@EntrenamientoId7, 4, 'Caminar libre', 'Permite al caballo caminar libremente para relajarse.', 300, 'VueltaCalma', 'https://images.unsplash.com/photo-1674504787092-ff26949e3aa8?fm=jpg&q=60&w=3000&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D');

-- Entrenamiento 8: Fortalecimiento y postura para jinetes
INSERT INTO Entrenamientos (Titulo, Tipo, Descripcion, Imagen)
VALUES ('Fortalecimiento y postura para jinetes', 'Jinete', 'Mejora la postura y fuerza del jinete para un mejor equilibrio.', 'https://unionargentinadeyoga.com.ar/wp-content/uploads/is-yoga-good-for-equestrians.webp');

SET @EntrenamientoId8 = LAST_INSERT_ID();

INSERT INTO EjerciciosEntrenamiento (EntrenamientoId, Orden, Nombre, Descripcion, DuracionSegundos, TipoBloque, ImagenURL)
VALUES
(@EntrenamientoId8, 1, 'Rotaciones de tronco', 'Gira suavemente el torso para mejorar flexibilidad.', 300, 'Calentamiento', 'https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEipOPfR1KpjNoMN_uFZs1lozqu-3uN-kbr5EvYod1CFmh1_DKwcIHJ4CYnc7nb4kuoWe2X85MySZVCTDDz6-TlgXCQ2Q0tkvRGTtxRyxvnbo2fa7erpALSB58lTPUIozOT2-kkg3vdr/s1600/Tronco.jpg'),
(@EntrenamientoId8, 2, 'Plancha lateral', 'Fortalece los músculos laterales del core.', 90, 'Principal', 'https://media.vogue.es/photos/5eb13d80a2686abb984c2bc9/1:1/pass/tabla-de-ejercicios2-1.jpg'),
(@EntrenamientoId8, 3, 'Equilibrio en bosu', 'Mejora estabilidad parándote en bosu o superficie inestable.', 120, 'Principal', 'https://thumbs.dreamstime.com/b/hombre-haciendo-un-vector-plano-de-equilibrio-est%C3%A1tico-bola-balance-bosu-ilustraci%C3%B3n-vectorial-plana-aislada-en-fondo-blanco-236633737.jpg'),
(@EntrenamientoId8, 4, 'Respiración y relajación', 'Técnicas de respiración para controlar tensión.', 180, 'VueltaCalma', 'https://metabolicas.sjdhospitalbarcelona.org/sites/default/files/imatge_respiracio_cast.jpg');

-- Entrenamiento 9: Coordinación y resistencia para jinetes
INSERT INTO Entrenamientos (Titulo, Tipo, Descripcion, Imagen)
VALUES ('Coordinación y resistencia para jinetes', 'Jinete', 'Sesión enfocada en mejorar coordinación y resistencia física del jinete.', 'https://esp.horselife.org/wp-content/uploads/2024/09/workouts-for-equestrians-30603-adt1_0.jpg');

SET @EntrenamientoId9 = LAST_INSERT_ID();

INSERT INTO EjerciciosEntrenamiento (EntrenamientoId, Orden, Nombre, Descripcion, DuracionSegundos, TipoBloque, ImagenURL)
VALUES
(@EntrenamientoId9, 1, 'Calentamiento articular', 'Moviliza muñecas, codos, hombros y cuello.', 300, 'Calentamiento', 'https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEjvpX3OSQLx8p7HgkOdX2KuXMwqpZobjBg1QnOakI53QkFT-2kzJaH_vMVRQUbE8cwLJXqKfQDFfSKZOv1aosDJtGl36zIF4OtZURaGEg5sovNvqFNd4mPGuO2fKZIxTcHt3LR0EBkoJHqZ/s1600/Image+11.png'),
(@EntrenamientoId9, 2, 'Sentadillas con peso corporal', 'Mejora resistencia y fuerza de piernas.', 300, 'Principal', 'https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEhKTQN3i8IvozFKr9TlaOXILeecyJ6NCVgYaK0AJtH8F_OUKgQut5a2FMzofqqCVzNASwoiUKiNUtUcZ72knbsMURIDEyIsLBIDfNPQFZlVxKzkr9Q3l-LcBPlUOZPB-ta_-7r66dqlDXw/s1600/sentadillas-peso-corporal.jpg'),
(@EntrenamientoId9, 3, 'Ejercicio de coordinación manos-pies', 'Realiza movimientos alternados para mejorar coordinación.', 240, 'Principal', 'https://png.pngtree.com/png-clipart/20190618/original/pngtree-yoga-yoga-action-physical-education-sports-png-image_3922523.jpg'),
(@EntrenamientoId9, 4, 'Estiramientos finales', 'Relaja músculos y mejora flexibilidad.', 180, 'VueltaCalma', 'https://fisioterapiacranium.com/wp-content/uploads/2017/03/Estiramientos.gif');
