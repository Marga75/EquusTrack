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
  const [alumnoSeleccionado, setAlumnoSeleccionado] = useState(""); // filtro alumno

  // Memoizamos usuario para que no cambie en cada render
  const usuario = useMemo(() => {
    const usuarioGuardado = localStorage.getItem("usuario");
    return usuarioGuardado ? JSON.parse(usuarioGuardado) : null;
  }, []);

  // Guardamos la lista de alumnos solo si es entrenador
  const [alumnos, setAlumnos] = useState([]);

  useEffect(() => {
    async function fetchHistorialDeJinete(idJinete) {
      const res = await fetch(
        `http://localhost:5000/api/historial/jinete/${idJinete}`
      );
      if (!res.ok) throw new Error("Error al cargar el historial");
      const data = await res.json();
      if (!data.exito || !Array.isArray(data.historial)) {
        throw new Error("El backend no devolvió un array válido");
      }
      return data.historial;
    }

    async function fetchAlumnosDelEntrenador() {
      const res = await fetch(
        `http://localhost:5000/relacion/alumnos/entrenador/${usuario.id}`
      );
      if (!res.ok) throw new Error("Error al cargar alumnos");
      const data = await res.json();
      return data;
    }

    async function cargarHistorial() {
      try {
        if (!usuario) return;

        if (usuario.rol === "Entrenador") {
          const alumnosData = await fetchAlumnosDelEntrenador();

          // Añadimos el propio entrenador para filtrar sus entrenamientos
          const alumnosConEntrenador = [
            { id: usuario.id, Nombre: "Entrenamientos propios" },
            ...alumnosData.map((al) => ({
              id: al.Id || al.id,
              Nombre: al.Nombre || al.nombre,
            })),
          ];

          setAlumnos(alumnosConEntrenador);

          // Obtener historial de cada alumno + entrenador
          const todosHistoriales = await Promise.all(
            alumnosConEntrenador.map(async (alumno) => {
              try {
                return await fetchHistorialDeJinete(alumno.id);
              } catch {
                return [];
              }
            })
          );

          // Unir todos los historiales en uno solo y guardarlo
          const historialUnificado = todosHistoriales.flat();
          setHistorial(historialUnificado);
        } else if (usuario.rol === "Jinete") {
          // Solo historial propio
          const historialPropio = await fetchHistorialDeJinete(usuario.id);
          setHistorial(historialPropio);
        }
      } catch (err) {
        console.error(err);
        setError("No se pudo cargar el historial");
      }
    }

    cargarHistorial();
  }, [usuario]);

  // Lista única de caballos
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

  // Filtrar historial por caballo, filtro general y alumno (si es entrenador)
  const historialFiltrado = useMemo(() => {
    let resultado = historial;

    if (filtro === "sinCaballo") {
      resultado = resultado.filter((h) => h.IdCaballo === null);
    } else if (filtro === "conCaballo") {
      if (caballoSeleccionado) {
        resultado = resultado.filter(
          (h) => h.IdCaballo === parseInt(caballoSeleccionado)
        );
      } else {
        resultado = resultado.filter((h) => h.IdCaballo !== null);
      }
    }

    if (usuario?.rol === "Entrenador" && alumnoSeleccionado) {
      resultado = resultado.filter(
        (h) => h.RegistradoPorId === parseInt(alumnoSeleccionado)
      );
    }

    return resultado;
  }, [filtro, caballoSeleccionado, alumnoSeleccionado, historial, usuario]);

  return (
    <div>
      <LayoutConHeader links={links} />
      <div className="max-w-4xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-6 text-center">
          Historial de Entrenamientos
        </h1>

        {/* Filtros */}
        <div className="mb-6 flex flex-col md:flex-row md:items-center md:gap-4">
          {/* Filtro general */}
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

          {/* Filtro caballo */}
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

          {/* Filtro alumno (solo si es entrenador y tiene alumnos) */}
          {usuario?.rol === "Entrenador" && alumnos.length > 0 && (
            <select
              className="border rounded px-3 py-2 mt-2 md:mt-0"
              value={alumnoSeleccionado}
              onChange={(e) => setAlumnoSeleccionado(e.target.value)}
            >
              <option value="">Todos los usuarios</option>
              {alumnos.map((al) => (
                <option key={al.id} value={al.id}>
                  {al.Nombre}
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
                  {usuario?.rol === "Entrenador" && (
                    <p className="text-sm text-gray-500">
                      Jinete: {item.NombreUsuario}
                    </p>
                  )}
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
