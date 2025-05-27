import { useState, useEffect } from "react";

export default function EntrenadorPerfil({ usuario }) {
  const [solicitudes, setSolicitudes] = useState([]);
  const [alumnos, setAlumnos] = useState([]);

  // Carga solicitudes pendientes y alumnos aceptados
  useEffect(() => {
    async function fetchSolicitudesYAlumnos() {
      try {
        // Suponiendo que tu backend tiene estas APIs:
        const resSolicitudes = await fetch(
          `http://localhost:5000/relacion/solicitudes/entrenador/${usuario.id}`
        );
        const solicitudesData = await resSolicitudes.json();
        console.log("Solicitudes recibidas:", solicitudesData);

        const resAlumnos = await fetch(
          `http://localhost:5000/relacion/alumnos/entrenador/${usuario.id}`
        );
        const alumnosData = await resAlumnos.json();

        setSolicitudes(solicitudesData);
        setAlumnos(alumnosData);
      } catch (error) {
        console.error("Error cargando solicitudes o alumnos", error);
      }
    }

    fetchSolicitudesYAlumnos();
  }, [usuario.id]);

  return (
    <div className="mt-6 w-full">
      <h3 className="text-xl font-semibold mb-2">Solicitudes de Alumnos</h3>
      {solicitudes.length === 0 && <p>No hay solicitudes pendientes</p>}
      <ul>
        {solicitudes.map((sol) => (
          <li key={sol.Id} className="border p-2 rounded mb-1">
            <p>
              <strong>
                {sol.Nombre} {sol.Apellido}
              </strong>{" "}
              ha solicitado relacionarse
            </p>
          </li>
        ))}
      </ul>

      <h3 className="text-xl font-semibold mt-8 mb-2">Alumnos</h3>
      {alumnos.length === 0 && <p>No tienes alumnos aceptados</p>}
      <ul>
        {alumnos.map((alumno) => (
          <li key={alumno.id} className="p-2 border rounded mb-2">
            <p>
              {alumno.nombre} {alumno.apellido}
            </p>
          </li>
        ))}
      </ul>
    </div>
  );
}
