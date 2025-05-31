import { useParams, useNavigate } from "react-router-dom";
import LayoutConHeader from "../components/Header";

// Datos mock para ejemplo (en backend luego irán por fetch)
const historialMock = [
  {
    id: 1,
    nombreEntrenamiento: "Resistencia Básica",
    fecha: "2025-05-31",
    progreso: 85,
    notas: "Muy buen rendimiento, mantener ritmo.",
    tipo: "Pie a tierra",
    duracion: 30,
  },
  {
    id: 2,
    nombreEntrenamiento: "Técnica de Salto",
    fecha: "2025-05-29",
    progreso: 70,
    notas: "Faltó concentración en los últimos saltos.",
    tipo: "Montado",
    duracion: 20,
  },
];

export default function HistorialDetalle() {
  const { id } = useParams();
  const navigate = useNavigate();

  // Buscar el registro por id
  const registro = historialMock.find((item) => item.id === parseInt(id));

  if (!registro) {
    return (
      <div className="p-6 text-center">
        <h2 className="text-2xl font-bold mb-4">Registro no encontrado</h2>
        <button
          className="bg-blue-600 text-white px-4 py-2 rounded"
          onClick={() => navigate(-1)}
        >
          Volver
        </button>
      </div>
    );
  }

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Historial", href: "/historial" },
  ];

  return (
    <div>
      <LayoutConHeader links={links} />
      <div className="max-w-3xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-6 text-center">Detalle del Entrenamiento</h1>
        <div className="bg-white shadow rounded p-6 space-y-4">
          <h2 className="text-2xl font-semibold">{registro.nombreEntrenamiento}</h2>
          <p>
            <strong>Fecha:</strong>{" "}
            {new Date(registro.fecha).toLocaleDateString()}
          </p>
          <p>
            <strong>Tipo:</strong> {registro.tipo}
          </p>
          <p>
            <strong>Duración:</strong> {registro.duracion} minutos
          </p>
          <p>
            <strong>Progreso:</strong> {registro.progreso}%
          </p>
          <p>
            <strong>Notas:</strong>
            <br />
            {registro.notas}
          </p>
          <button
            className="mt-4 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition"
            onClick={() => navigate(-1)}
          >
            Volver al historial
          </button>
        </div>
      </div>
    </div>
  );
}
