import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState, useRef } from "react";
import LayoutConHeader from "../components/Header";

const entrenamientosMock = [
  {
    id: 1,
    nombre: "Resistencia Básica",
    tipo: "Pie a tierra",
    duracion: 30,
    descripcion: "Entrenamiento ideal para mejorar la resistencia general del caballo.",
    imagen: "https://via.placeholder.com/400x200?text=Resistencia",
    ejercicios: [
      { id: 1, nombre: "Paso prolongado", duracionSeg: 300, descripcion: "Camina a paso lento pero constante." },
      { id: 2, nombre: "Trote en círculo", duracionSeg: 600, descripcion: "Mantén el trote en círculos amplios." },
      { id: 3, nombre: "Paso relajado", duracionSeg: 300, descripcion: "Relaja y camina tranquilamente para bajar ritmo." },
    ],
  },
  {
    id: 2,
    nombre: "Técnica de Salto",
    tipo: "Montado",
    duracion: 20,
    descripcion: "Técnica enfocada en mejorar los saltos con obstáculos.",
    imagen: "https://via.placeholder.com/400x200?text=Salto",
    ejercicios: [
      { id: 1, nombre: "Calentamiento paso y trote", duracionSeg: 300, descripcion: "Calienta con paso y trote ligero." },
      { id: 2, nombre: "Saltos en línea", duracionSeg: 600, descripcion: "Practica saltos consecutivos." },
      { id: 3, nombre: "Vuelta a la calma", duracionSeg: 300, descripcion: "Relaja el caballo con paso suave." },
    ],
  },
];

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

  useEffect(() => {
    const data = entrenamientosMock.find((e) => e.id === parseInt(id));
    setEntrenamiento(data);
    setIndiceEjercicio(0);
  }, [id]);

  useEffect(() => {
    if (entrenamiento) {
      setTiempoRestante(entrenamiento.ejercicios[indiceEjercicio].duracionSeg);
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
      // Pasar al siguiente ejercicio automáticamente
      if (indiceEjercicio < entrenamiento.ejercicios.length - 1) {
        setIndiceEjercicio((i) => i + 1);
      } else {
        alert("¡Has completado el entrenamiento!");
        navigate(`/entrenamientos/${id}`);
      }
      setActivo(false);
    }
    return () => clearInterval(timerRef.current);
  }, [activo, tiempoRestante, indiceEjercicio, entrenamiento, id, navigate]);

  if (!entrenamiento) return <p className="p-6">Cargando entrenamiento...</p>;

  const ejercicioActual = entrenamiento.ejercicios[indiceEjercicio];

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Entrenamientos", href: "/entrenamientos" },
    { label: entrenamiento.nombre, href: `/entrenamientos/${id}` },
  ];

  return (
    <div>
      <LayoutConHeader links={links} handleLogout={() => navigate("/", { replace: true })} />
      <div className="max-w-3xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-4 text-center">{entrenamiento.nombre} - Modo Guiado</h1>
        <div className="bg-white shadow-md rounded-lg p-6 text-center">
          <h2 className="text-2xl font-semibold mb-2">{ejercicioActual.nombre}</h2>
          <p className="text-gray-700 mb-4">{ejercicioActual.descripcion}</p>

          <div className="text-5xl font-mono mb-6">{formatTime(tiempoRestante)}</div>

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
                setTiempoRestante(ejercicioActual.duracionSeg);
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
                if (indiceEjercicio < entrenamiento.ejercicios.length - 1) {
                  setIndiceEjercicio((i) => i + 1);
                } else {
                  alert("¡Has completado el entrenamiento!");
                  navigate(`/entrenamientos/${id}`);
                }
              }}
              className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            >
              {indiceEjercicio === entrenamiento.ejercicios.length - 1 ? "Finalizar" : "Siguiente"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
