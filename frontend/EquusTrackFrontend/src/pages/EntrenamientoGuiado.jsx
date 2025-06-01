import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState, useRef } from "react";
import LayoutConHeader from "../components/Header";

function formatTime(segundos) {
  const min = Math.floor(segundos / 60);
  const seg = segundos % 60;
  return `${min}:${seg.toString().padStart(2, "0")}`;
}

export default function EntrenamientoGuiado() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [entrenamiento, setEntrenamiento] = useState(null);
  const [indiceEjercicio, setIndiceEjercicio] = useState(0);
  const [tiempoRestante, setTiempoRestante] = useState(0);
  const [activo, setActivo] = useState(false);
  const timerRef = useRef(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchEntrenamiento() {
      try {
        const res = await fetch(
          `http://localhost:5000/api/entrenamientos/${id}`
        );
        if (!res.ok) throw new Error("Error al cargar el entrenamiento guiado");
        const data = await res.json();
        // Ajusta según cómo venga la respuesta
        const entren = data.entrenamiento || data;
        if (!entren) throw new Error("Entrenamiento no encontrado");
        setEntrenamiento(entren);
        setIndiceEjercicio(0);
      } catch (err) {
        console.error(err);
        setError("No se pudo cargar el entrenamiento guiado");
      }
    }
    fetchEntrenamiento();
  }, [id]);

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
        alert("¡Has completado el entrenamiento!");
        navigate(`/entrenamientos/${id}`);
      }
      setActivo(false);
    }
    return () => clearInterval(timerRef.current);
  }, [activo, tiempoRestante, indiceEjercicio, entrenamiento, id, navigate]);

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

  return (
    <div>
      <LayoutConHeader
        links={links}
        handleLogout={() => navigate("/", { replace: true })}
      />
      <div className="max-w-3xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-4 text-center">
          {entrenamiento.Nombre || entrenamiento.nombre} - Modo Guiado
        </h1>
        <div className="bg-white shadow-md rounded-lg p-6 text-center">
          <h2 className="text-2xl font-semibold mb-2">
            {ejercicioActual.Nombre || ejercicioActual.nombre}
          </h2>
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

            <button
              onClick={() => {
                setActivo(false);
                if (indiceEjercicio < entrenamiento.Ejercicios.length - 1) {
                  setIndiceEjercicio((i) => i + 1);
                } else {
                  alert("¡Has completado el entrenamiento!");
                  navigate(`/entrenamientos/${id}`);
                }
              }}
              className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              {indiceEjercicio === entrenamiento.Ejercicios.length - 1
                ? "Finalizar"
                : "Siguiente"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
