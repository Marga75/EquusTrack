import LayoutConHeader from "../components/Header";
import { useNavigate } from "react-router-dom";

const historialMock = [
  {
    id: 1,
    nombreEntrenamiento: "Resistencia Básica",
    fecha: "2025-05-31",
    progreso: 85,
    notas: "Muy buen rendimiento, mantener ritmo.",
  },
  {
    id: 2,
    nombreEntrenamiento: "Técnica de Salto",
    fecha: "2025-05-29",
    progreso: 70,
    notas: "Faltó concentración en los últimos saltos.",
  },
];

export default function HistorialEntrenamientos() {
  const navigate = useNavigate();

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Entrenamientos", href: "/entrenamientos" },
  ];

  return (
    <div>
      <LayoutConHeader links={links} />
      <div className="max-w-4xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-6 text-center">
          Historial de Entrenamientos
        </h1>
        {historialMock.length === 0 ? (
          <p className="text-center text-gray-600">
            No hay entrenamientos realizados aún.
          </p>
        ) : (
          <div className="space-y-6">
            {historialMock.map((item) => (
              <div
                key={item.id}
                className="bg-white rounded-lg shadow p-4 flex flex-col md:flex-row md:items-center md:justify-between"
              >
                <div>
                  <h2 className="text-xl font-semibold">
                    {item.nombreEntrenamiento}
                  </h2>
                  <p className="text-gray-600 text-sm">
                    {new Date(item.fecha).toLocaleDateString()}
                  </p>
                </div>
                <div className="mt-2 md:mt-0 flex items-center gap-6">
                  <div>
                    <p className="font-semibold text-center">Progreso</p>
                    <p className="text-green-600 font-bold text-lg text-center">
                      {item.progreso}%
                    </p>
                  </div>
                  <div className="max-w-xs">
                    <p className="font-semibold">Notas</p>
                    <p className="text-gray-700 text-sm">{item.notas}</p>
                  </div>
                  <button
                    onClick={() => navigate(`/historial/${item.id}`)}
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
