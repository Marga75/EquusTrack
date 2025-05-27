import { useState, useEffect } from "react";

export default function SeleccionarEntrenador({ usuario }) {
  const [entrenadores, setEntrenadores] = useState([]);
  const [entrenadorSeleccionado, setEntrenadorSeleccionado] = useState(null);
  const [estadoRelacion, setEstadoRelacion] = useState(null);
  const [mostrandoSelector, setMostrandoSelector] = useState(false);

  async function fetchEntrenadores() {
    try {
      const res = await fetch("http://localhost:5000/entrenadores");
      if (!res.ok) throw new Error("Error al cargar entrenadores");
      const data = await res.json();
      console.log("Respuesta de entrenadores:", data); // DEBUG
      setEntrenadores(Array.isArray(data) ? data : []);
    } catch (error) {
      console.error(error);
      setEntrenadores([]); // evitar errores si hay fallo en fetch
    }
  }

  async function fetchRelacionEntrenador() {
    try {
      const res = await fetch(
        `http://localhost:5000/entrenador/jinete/${usuario.id}`
      );
      if (!res.ok) throw new Error("Error al cargar relación entrenador");
      const data = await res.json();

      // Como backend devuelve solo el entrenador o null,
      // si hay entrenador el estado será "aceptado", sino null.
      if (data) {
        setEstadoRelacion("aceptado");
        setEntrenadorSeleccionado(data);
      } else {
        setEstadoRelacion(null);
        setEntrenadorSeleccionado(null);
      }
    } catch (error) {
      console.error(error);
    }
  }

  async function solicitarEntrenador(idEntrenador) {
    try {
      const res = await fetch("http://localhost:5000/relacion/jinete", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ idJinete: usuario.id, idEntrenador }),
      });
      if (!res.ok) throw new Error("Error enviando solicitud");
      setEstadoRelacion("pendiente");
      setMostrandoSelector(false);
    } catch (error) {
      console.error(error);
    }
  }

  useEffect(() => {
    if (usuario?.id && usuario?.rol?.toLowerCase() === "jinete") {
      fetchRelacionEntrenador();
    }
  }, [usuario]);

  if (usuario?.rol?.toLowerCase() !== "jinete") return null;

  return (
    <div className="mt-4 w-full text-center">
      {estadoRelacion === "aceptado" && entrenadorSeleccionado ? (
        <p className="font-semibold">
          Entrenador: {entrenadorSeleccionado.Nombre}{" "}
          {entrenadorSeleccionado.Apellido}
        </p>
      ) : estadoRelacion === "pendiente" ? (
        <p className="font-semibold text-yellow-600">
          Solicitud enviada. Esperando respuesta.
        </p>
      ) : (
        <>
          {!mostrandoSelector ? (
            <button
              onClick={() => {
                fetchEntrenadores();
                setMostrandoSelector(true);
              }}
              className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
            >
              Seleccionar entrenador
            </button>
          ) : (
            <select
              className="mt-2 w-full border rounded p-2"
              defaultValue=""
              onChange={(e) => {
                const idSeleccionado = e.target.value;
                if (idSeleccionado) {
                  solicitarEntrenador(parseInt(idSeleccionado));
                }
              }}
            >
              <option value="" disabled>
                Selecciona un entrenador
              </option>

              {console.log("Entrenadores para el selector:", entrenadores)}

              {Array.isArray(entrenadores) &&
                entrenadores.map((ent) => (
                  <option key={ent.Id} value={ent.Id}>
                    {ent.Nombre} {ent.Apellido}
                  </option>
                ))}
            </select>
          )}
        </>
      )}
    </div>
  );
}
