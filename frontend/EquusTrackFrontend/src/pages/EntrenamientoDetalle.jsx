import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import LayoutConHeader from "../components/Header";
import { useAuth } from "../components/AuthContext";

export default function EntrenamientoDetalle() {
  const { logout } = useAuth();
  const { id } = useParams();
  const navigate = useNavigate();
  const [entrenamiento, setEntrenamiento] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchEntrenamientoDetalle() {
      try {
        const res = await fetch(`http://localhost:5000/api/entrenamientos/${id}`);
        if (!res.ok) throw new Error("Error al cargar el detalle del entrenamiento");
        const data = await res.json();
        console.log("Respuesta del backend:", data);

        if (!data || typeof data !== "object") {
          throw new Error("Datos inválidos");
        }

        setEntrenamiento(data.entrenamiento);
      } catch (error) {
        console.error(error);
        setError("No se pudo cargar el entrenamiento");
      }
    }
    fetchEntrenamientoDetalle();
  }, [id]);

  if (error) return <p className="p-6 text-red-600">{error}</p>;
  if (!entrenamiento) return <p className="p-6">Cargando entrenamiento...</p>;

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Entrenamientos", href: "/entrenamientos" },
  ];

  const handleLogout = () => {
    logout();
    navigate("/", { replace: true });
  };

  return (
    <div>
      <LayoutConHeader
          links={links}
          handleLogout={handleLogout}
        ></LayoutConHeader>
      <div className="max-w-4xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-4 text-center">
          {entrenamiento.Nombre}
        </h1>
        <img
          src={entrenamiento.Imagen || "/imagen-default.jpg"}
          alt={entrenamiento.Nombre}
          className="w-full h-64 object-cover rounded-xl mb-4"
        />
        <p className="text-gray-700 mb-2">{entrenamiento.Descripcion}</p>
        <p className="text-sm text-gray-600 mb-6">
          Tipo: <strong>{entrenamiento.Tipo}</strong> · Duración:{" "}
          <strong>{entrenamiento.DuracionTotalSegundos} min</strong>
        </p>

        <h2 className="text-xl font-semibold mb-2">Ejercicios</h2>
        {Array.isArray(entrenamiento.Ejercicios) && entrenamiento.Ejercicios.length > 0 ? (
          <ul className="space-y-2 mb-6">
            {entrenamiento.Ejercicios.map((ej) => (
              <li key={ej.Id} className="bg-gray-100 p-3 rounded-lg shadow-sm">
                <strong>{ej.Nombre}</strong> —{" "}
                <span className="text-sm text-gray-600">{ej.DuracionSegundos}</span>
              </li>
            ))}
          </ul>
        ) : (
          <p className="text-gray-500 italic mb-6">Este entrenamiento no tiene ejercicios.</p>
        )}

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
