import LayoutConHeader from "../components/Header";
import { useNavigate } from "react-router-dom";
import React, { useEffect, useState, useMemo } from "react";

export default function HistorialEntrenamientos() {
  const navigate = useNavigate();

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Entrenamientos", href: "/entrenamientos" },
  ];

  const [historial, setHistorial] = useState([]);
  const [error, setError] = useState(null);

  const [filtro, setFiltro] = useState("todos");
  const [caballoSeleccionado, setCaballoSeleccionado] = useState("");

  const usuarioGuardado = localStorage.getItem("usuario");
  const usuario = usuarioGuardado ? JSON.parse(usuarioGuardado) : null;
  const idJinete = usuario ? usuario.id : null;

  useEffect(() => {
    async function fetchHistorial() {
      try {
        const res = await fetch(
          `http://localhost:5000/api/historial/jinete/${idJinete}`
        );
        if (!res.ok) throw new Error("Error al cargar el historial");
        const data = await res.json();

        console.log("Respuesta del backend:", data);

        if (!data.exito || !Array.isArray(data.historial)) {
          throw new Error("El backend no devolvió un array válido");
        }

        setHistorial(data.historial);
      } catch (error) {
        console.error(error);
        setError("No se pudo cargar el historial");
      }
    }

    if (idJinete) {
      fetchHistorial();
    }
  }, [idJinete]);

  // Obtener lista única de caballos
  const caballosUnicos = useMemo(() => {
    const caballos = historial
      .filter((h) => h.IdCaballo !== null)
      .map((h) => ({ id: h.IdCaballo, nombre: h.NombreCaballo }));

    const unicos = [];
    const ids = new Set();
    for (const c of caballos) {
      if (!ids.has(c.id)) {
        ids.add(c.id);
        unicos.push(c);
      }
    }
    return unicos;
  }, [historial]);

  // Filtrar historial según filtro y caballo seleccionado
  const historialFiltrado = useMemo(() => {
    if (filtro === "sinCaballo") {
      return historial.filter((h) => h.IdCaballo === null);
    } else if (filtro === "conCaballo") {
      if (!caballoSeleccionado)
        return historial.filter((h) => h.IdCaballo !== null);
      return historial.filter(
        (h) => h.IdCaballo === parseInt(caballoSeleccionado)
      );
    } else {
      return historial;
    }
  }, [filtro, caballoSeleccionado, historial]);

  return (
    <div>
      <LayoutConHeader links={links} />
      <div className="max-w-4xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-6 text-center">
          Historial de Entrenamientos
        </h1>

        {/* Filtros */}
        <div className="mb-6 flex flex-col md:flex-row md:items-center md:gap-4">
          <select
            className="border rounded px-3 py-2"
            value={filtro}
            onChange={(e) => {
              setFiltro(e.target.value);
              setCaballoSeleccionado("");
            }}
          >
            <option value="todos">Todos los entrenamientos</option>
            <option value="conCaballo">Con caballo</option>
            <option value="sinCaballo">Sin caballo</option>
          </select>

          {filtro === "conCaballo" && (
            <select
              className="border rounded px-3 py-2 mt-2 md:mt-0"
              value={caballoSeleccionado}
              onChange={(e) => setCaballoSeleccionado(e.target.value)}
            >
              <option value="">Todos los caballos</option>
              {caballosUnicos.map((cab) => (
                <option key={cab.id} value={cab.id}>
                  {cab.nombre}
                </option>
              ))}
            </select>
          )}
        </div>

        {/* Error */}
        {error && <p className="text-center text-red-600 mb-4">{error}</p>}

        {/* Listado */}
        {historialFiltrado.length === 0 ? (
          <p className="text-center text-gray-600">
            No hay entrenamientos realizados aún.
          </p>
        ) : (
          <div className="space-y-6">
            {historialFiltrado.map((item) => (
              <div
                key={item.Id}
                className="bg-white rounded-lg shadow p-4 flex flex-col md:flex-row md:items-center md:justify-between"
              >
                <div>
                  <h2 className="text-xl font-semibold">
                    {item.NombreEntrenamiento}
                  </h2>
                  <p className="text-sm text-gray-700">
                    {item.IdCaballo
                      ? `Caballo: ${item.NombreCaballo}`
                      : "Entrenamiento sin caballo"}
                  </p>
                  <p className="text-gray-600 text-sm">
                    {new Date(item.Fecha).toLocaleDateString()}
                  </p>
                </div>
                <div className="mt-2 md:mt-0 flex items-center gap-6">
                  <div>
                    <p className="font-semibold text-center">Progreso</p>
                    <p className="text-green-600 font-bold text-lg text-center">
                      {item.Progreso}%
                    </p>
                  </div>
                  <div className="max-w-xs">
                    <p className="font-semibold">Notas</p>
                    <p className="text-gray-700 text-sm">{item.Notas}</p>
                  </div>
                  <button
                    onClick={() => navigate(`/historial/${item.Id}`)}
                    className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
                  >
                    Ver detalles
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
