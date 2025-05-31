import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
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
      { id: 1, nombre: "Paso prolongado", duracion: "5 min" },
      { id: 2, nombre: "Trote en círculo", duracion: "10 min" },
      { id: 3, nombre: "Paso relajado", duracion: "5 min" },
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
      { id: 1, nombre: "Calentamiento paso y trote", duracion: "5 min" },
      { id: 2, nombre: "Saltos en línea", duracion: "10 min" },
      { id: 3, nombre: "Vuelta a la calma", duracion: "5 min" },
    ],
  },
];

export default function EntrenamientoDetalle() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [entrenamiento, setEntrenamiento] = useState(null);

  useEffect(() => {
    // Simulación de carga desde API
    const data = entrenamientosMock.find((e) => e.id === parseInt(id));
    setEntrenamiento(data);
  }, [id]);

  if (!entrenamiento) return <p className="p-6">Cargando entrenamiento...</p>;

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Entrenamientos", href: "/entrenamientos" },
  ];

  return (
    <div>
      <LayoutConHeader links={links} handleLogout={() => navigate("/", { replace: true })} />
      <div className="max-w-4xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-4 text-center">{entrenamiento.nombre}</h1>
        <img
          src={entrenamiento.imagen}
          alt={entrenamiento.nombre}
          className="w-full h-64 object-cover rounded-xl mb-4"
        />
        <p className="text-gray-700 mb-2">{entrenamiento.descripcion}</p>
        <p className="text-sm text-gray-600 mb-6">
          Tipo: <strong>{entrenamiento.tipo}</strong> · Duración: <strong>{entrenamiento.duracion} min</strong>
        </p>

        <h2 className="text-xl font-semibold mb-2">Ejercicios</h2>
        <ul className="space-y-2 mb-6">
          {entrenamiento.ejercicios.map((ej) => (
            <li key={ej.id} className="bg-gray-100 p-3 rounded-lg shadow-sm">
              <strong>{ej.nombre}</strong> — <span className="text-sm text-gray-600">{ej.duracion}</span>
            </li>
          ))}
        </ul>

        <button
          onClick={() => navigate(`/entrenamientos/${id}/guiado`)}
          className="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 transition"
        >
          Iniciar Entrenamiento
        </button>
      </div>
    </div>
  );
}
