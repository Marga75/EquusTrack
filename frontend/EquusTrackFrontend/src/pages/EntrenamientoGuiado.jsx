import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState, useRef } from "react";
import LayoutConHeader from "../components/Header";
import { useAuth } from "../components/AuthContext";

function formatTime(segundos) {
  const min = Math.floor(segundos / 60);
  const seg = segundos % 60;
  return `${min}:${seg.toString().padStart(2, "0")}`;
}

export default function EntrenamientoGuiado() {
  const { logout } = useAuth();
  const { id } = useParams();
  const navigate = useNavigate();
  const [entrenamiento, setEntrenamiento] = useState(null);
  const [indiceEjercicio, setIndiceEjercicio] = useState(0);
  const [tiempoRestante, setTiempoRestante] = useState(0);
  const [activo, setActivo] = useState(false);
  const [finalizado, setFinalizado] = useState(false);
  const [notas, setNotas] = useState("");
  const [exito, setExito] = useState(false);
  const [error, setError] = useState(null);
  const [caballos, setCaballos] = useState([]);
  const [caballoSeleccionado, setCaballoSeleccionado] = useState(null);
  const timerRef = useRef(null);

  const usuarioGuardado = localStorage.getItem("usuario");
  const usuario = usuarioGuardado ? JSON.parse(usuarioGuardado) : null;
  const idUsuario = usuario ? usuario.id : null;

  const handleLogout = () => {
    logout();
    navigate("/", { replace: true });
  };

  useEffect(() => {
    async function fetchEntrenamiento() {
      try {
        const res = await fetch(
          `http://localhost:5000/api/entrenamientos/${id}`
        );
        if (!res.ok) throw new Error("Error al cargar el entrenamiento guiado");
        const data = await res.json();
        const entren = data.entrenamiento || data;
        if (!entren) throw new Error("Entrenamiento no encontrado");
        setEntrenamiento(entren);
        setIndiceEjercicio(0);
      } catch (err) {
        console.error(err);
        setError("No se pudo cargar el entrenamiento guiado");
      }
    }

    async function fetchCaballos() {
      if (!idUsuario) {
        console.warn("No hay usuario logueado, no se cargan caballos");
        return;
      }
      try {
        const res = await fetch(
          `http://localhost:5000/api/caballos/usuario/${idUsuario}`
        );
        if (!res.ok) throw new Error("Error al cargar caballos");
        const data = await res.json();
        const listaCaballos = data.caballos || data;
        setCaballos(listaCaballos);
        if (listaCaballos.length > 0) {
          setCaballoSeleccionado(listaCaballos[0].Id || listaCaballos[0].id);
        }
      } catch (err) {
        console.error("No se pudieron cargar los caballos", err);
      }
    }

    fetchEntrenamiento();
    fetchCaballos();
  }, [id, idUsuario]);

  useEffect(() => {
    if (
      entrenamiento &&
      entrenamiento.Ejercicios &&
      entrenamiento.Ejercicios.length > 0
    ) {
      const duracionMinutos =
        entrenamiento.Ejercicios[indiceEjercicio].DuracionSegundos ||
        entrenamiento.Ejercicios[indiceEjercicio].duracionSeg;
      setTiempoRestante(duracionMinutos * 60); // Pasar minutos a segundos
      setActivo(false);
      clearInterval(timerRef.current);
    }
  }, [entrenamiento, indiceEjercicio]);

  useEffect(() => {
    if (activo && tiempoRestante > 0) {
      timerRef.current = setInterval(() => {
        setTiempoRestante((t) => t - 1);
      }, 1000);
    } else if (tiempoRestante === 0 && activo) {
      if (indiceEjercicio < entrenamiento.Ejercicios.length - 1) {
        setIndiceEjercicio((i) => i + 1);
      } else {
        setFinalizado(true);
      }
      setActivo(false);
    }
    return () => clearInterval(timerRef.current);
  }, [activo, tiempoRestante, indiceEjercicio, entrenamiento]);

  async function registrarEntrenamiento(progreso, estado = "Completado") {
    if (!idUsuario) {
      alert("Usuario no identificado. Debes iniciar sesión.");
      return;
    }

    const datos = {
      IdCaballo: caballoSeleccionado || null,
      IdEntrenamiento: entrenamiento.Id || entrenamiento.id,
      Fecha: new Date().toISOString().split("T")[0],
      Notas: notas,
      Progreso: progreso,
      RegistradoPorId: idUsuario,
      Estado: estado,
    };

    try {
      const res = await fetch("http://localhost:5000/api/historial", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(datos),
      });

      if (!res.ok) throw new Error("Error al registrar el entrenamiento");
      const json = await res.json();
      setExito(true);

      setTimeout(() => {
        navigate(`/entrenamientos/${id}`);
      }, 3000);
    } catch (error) {
      console.error("Error al guardar entrenamiento:", error);
      alert("Ocurrió un error al guardar el entrenamiento.");
    }
  }

  if (error) return <p className="p-6 text-red-600">{error}</p>;
  if (!entrenamiento) return <p className="p-6">Cargando entrenamiento...</p>;

  const ejercicioActual = entrenamiento.Ejercicios[indiceEjercicio];

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Entrenamientos", href: "/entrenamientos" },
    {
      label: entrenamiento.Nombre || entrenamiento.nombre,
      href: `/entrenamientos/${id}`,
    },
  ];

  if (finalizado) {
    const total = entrenamiento.Ejercicios.length;
    const completados = indiceEjercicio + 1;
    const progreso = Math.round((completados / total) * 100);

    return (
      <div>
        <LayoutConHeader
          links={links}
          handleLogout={handleLogout}
        ></LayoutConHeader>
        <div className="max-w-2xl mx-auto p-6 text-center bg-white shadow rounded">
          <h1 className="text-2xl font-bold mb-4">Entrenamiento Finalizado</h1>
          <p className="mb-2">
            Progreso: <strong>{progreso}%</strong>
          </p>

          <label className="block mb-2 font-semibold">
            Selecciona el caballo:
          </label>
          <select
            value={caballoSeleccionado || ""}
            onChange={(e) =>
              setCaballoSeleccionado(
                e.target.value === "" ? null : e.target.value
              )
            }
            className="mb-4 p-2 border rounded w-full"
          >
            <option value="">Sin caballo</option>
            {caballos.map((cab) => (
              <option key={cab.Id || cab.id} value={cab.Id || cab.id}>
                {cab.Nombre || cab.nombre}
              </option>
            ))}
          </select>

          {exito ? (
            <p className="text-green-600 text-lg font-semibold">
              Entrenamiento guardado exitosamente. Redirigiendo...
            </p>
          ) : (
            <>
              <textarea
                className="w-full p-3 border rounded mb-4"
                rows="4"
                placeholder="Escribe tus notas aquí..."
                value={notas}
                onChange={(e) => setNotas(e.target.value)}
              />
              <button
                onClick={() =>
                  registrarEntrenamiento(
                    progreso,
                    progreso === 100 ? "Completado" : "Incompleto"
                  )
                }
                className="px-6 py-2 bg-green-600 text-white rounded hover:bg-green-700"
              >
                Guardar en historial
              </button>
            </>
          )}
        </div>
      </div>
    );
  }

  return (
    <div>
      <LayoutConHeader
          links={links}
          handleLogout={handleLogout}
        ></LayoutConHeader>
      <div className="max-w-3xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-4 text-center">
          {entrenamiento.Nombre || entrenamiento.nombre} - Modo Guiado
        </h1>
        <div className="bg-white shadow-md rounded-lg p-6 text-center">
          <h2 className="text-2xl font-semibold mb-2">
            {ejercicioActual.Nombre || ejercicioActual.nombre}
          </h2>

          {ejercicioActual.ImagenURL && (
            <img
              src={ejercicioActual.ImagenURL}
              alt={`Imagen de ${
                ejercicioActual.Nombre || ejercicioActual.nombre
              }`}
              className="mx-auto mb-4 max-h-48 object-contain"
            />
          )}

          <p className="text-gray-700 mb-4">
            {ejercicioActual.Descripcion || ejercicioActual.descripcion}
          </p>

          <div className="text-5xl font-mono mb-6">
            {formatTime(tiempoRestante)}
          </div>

          <div className="flex justify-center gap-4 mb-6">
            {!activo ? (
              <button
                onClick={() => setActivo(true)}
                className="px-6 py-2 bg-green-600 text-white rounded hover:bg-green-700"
              >
                Iniciar
              </button>
            ) : (
              <button
                onClick={() => setActivo(false)}
                className="px-6 py-2 bg-yellow-500 text-white rounded hover:bg-yellow-600"
              >
                Pausar
              </button>
            )}
            <button
              onClick={() => {
                setActivo(false);
                const duracionMinutos =
                  ejercicioActual.DuracionSegundos ||
                  ejercicioActual.duracionSeg;
                setTiempoRestante(duracionMinutos * 60); // Pasar minutos a segundos
              }}
              className="px-6 py-2 bg-red-600 text-white rounded hover:bg-red-700"
            >
              Reiniciar
            </button>
          </div>

          <div className="flex justify-between">
            <button
              onClick={() => {
                setActivo(false);
                setIndiceEjercicio((i) => Math.max(i - 1, 0));
              }}
              disabled={indiceEjercicio === 0}
              className={`px-4 py-2 rounded ${
                indiceEjercicio === 0
                  ? "bg-gray-300 cursor-not-allowed"
                  : "bg-blue-600 text-white hover:bg-blue-700"
              }`}
            >
              Anterior
            </button>

            <div className="flex gap-4 justify-end mt-4">
              {indiceEjercicio < entrenamiento.Ejercicios.length - 1 && (
                <button
                  onClick={() => {
                    setActivo(false);
                    setIndiceEjercicio((i) => i + 1);
                  }}
                  className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                >
                  Siguiente
                </button>
              )}

              <button
                onClick={() => {
                  setActivo(false);
                  setFinalizado(true);
                }}
                className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
              >
                Finalizar
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
