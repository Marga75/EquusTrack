import { useEffect, useState } from "react";

export default function HistorialServicios({ idCaballo, tipo, recarga }) {
  const [servicios, setServicios] = useState([]);
  const [mostrarTodos, setMostrarTodos] = useState(false);

  useEffect(() => {
    async function cargarDatos() {
      try {
        const url = `http://localhost:5000/api/caballos/${idCaballo}/${tipo}`;
        const res = await fetch(url);
        const data = await res.json();
        console.log("Datos recibidos:", data);

        if (data.exito) {
          // Mapear el nombre del array esperado según el tipo
          let arrayNombre = "";
          if (tipo === "herrador") arrayNombre = "herradas";
          else if (tipo === "veterinario") arrayNombre = "visitas";
          else if (tipo === "fisio") arrayNombre = "sesiones";

          setServicios(data[arrayNombre] || []);
        }
      } catch (error) {
        console.error("Error cargando servicios", error);
      }
    }
    cargarDatos();
  }, [idCaballo, tipo, recarga, mostrarTodos]);

  return (
    <div>
      <table className="w-full border-collapse border">
        <thead>
          <tr className="bg-gray-200">
            <th className="border p-2 text-left">Fecha</th>
            <th className="border p-2 text-left">Nombre</th>
            {tipo !== "herrador" && (
              <th className="border p-2 text-left">Descripción</th>
            )}
            <th className="border p-2 text-right">Costo</th>
            {tipo === "herrador" && (
              <th className="border p-2 text-left">Próxima herrada</th>
            )}
          </tr>
        </thead>
        <tbody>
          {servicios.length === 0 ? (
            <tr>
              <td
                colSpan={tipo !== "herrador" ? 4 : 3}
                className="text-center p-4"
              >
                No hay registros
              </td>
            </tr>
          ) : (
            servicios.map((servicio) => (
              <tr key={servicio.Id || servicio.id}>
                <td className="border p-2">
                  {new Date(
                    servicio.FechaHerrada ||
                      servicio.FechaVisita ||
                      servicio.FechaSesion
                  ).toLocaleDateString()}
                </td>
                <td className="border p-2">
                  {servicio.NombreHerrador ||
                    servicio.VeterinarioNombre ||
                    servicio.Profesional ||
                    "—"}
                </td>
                {tipo !== "herrador" && (
                  <td className="border p-2">{servicio.Descripcion || "—"}</td>
                )}
                <td className="border p-2 text-right">
                  {(typeof servicio.Costo === "number"
                    ? servicio.Costo.toFixed(2)
                    : servicio.Costo) || "0.00"}{" "}
                  €
                </td>
                {tipo === "herrador" && (
                  <td className="border p-2">
                    {servicio.ProximaFechaHerrada
                      ? new Date(
                          servicio.ProximaFechaHerrada
                        ).toLocaleDateString()
                      : "—"}
                  </td>
                )}
              </tr>
            ))
          )}
        </tbody>
      </table>

      {servicios.length > 5 && (
        <button
          className="mt-2 text-blue-600 hover:underline"
          onClick={() => setMostrarTodos(!mostrarTodos)}
        >
          {mostrarTodos ? "Ver menos" : "Ver todos"}
        </button>
      )}
    </div>
  );
}
