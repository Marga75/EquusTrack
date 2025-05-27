import { useState, useEffect } from "react";

export default function EntrenadorPerfil({ usuario }) {
  const [solicitudes, setSolicitudes] = useState([]);
  const [alumnos, setAlumnos] = useState([]);

  async function fetchSolicitudesYAlumnos() {
    try {
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

  useEffect(() => {
    fetchSolicitudesYAlumnos();
  }, [usuario.id]);

  async function manejarRespuesta(idJinete, estado) {
    try {
      const res = await fetch("http://localhost:5000/relacion/estado", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          idJinete: idJinete,
          idEntrenador: usuario.id,
          estado: estado,
        }),
      });

      if (!res.ok) {
        const textoError = await res.text();
        console.error("Error respuesta backend:", textoError);
        alert("Error al actualizar la solicitud");
        return;
      }

      const data = await res.json();

      if (data.exito) {
        await fetchSolicitudesYAlumnos(); // Recarga datos
      } else {
        alert("Error al actualizar la solicitud");
      }
    } catch (error) {
      console.error("Error en manejarRespuesta", error);
    }
  }

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
            <div>
              <button
                className="bg-green-500 text-white px-3 py-1 rounded mr-2"
                onClick={() => manejarRespuesta(sol.Id, "aceptado")}
              >
                Aceptar
              </button>
              <button
                className="bg-red-500 text-white px-3 py-1 rounded"
                onClick={() => manejarRespuesta(sol.Id, "rechazado")}
              >
                Rechazar
              </button>
            </div>
          </li>
        ))}
      </ul>

      <h3 className="text-xl font-semibold mt-8 mb-2">Alumnos</h3>
      {alumnos.length === 0 && <p>No tienes alumnos aceptados</p>}
      <ul>
        {alumnos.map((alumno) => (
          <li key={alumno.Id} className="p-2 border rounded mb-2">
            <p>
              {alumno.Nombre} {alumno.Apellido}
            </p>
          </li>
        ))}
      </ul>
    </div>
  );
}
