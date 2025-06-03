import { useParams, useNavigate } from "react-router-dom";
import LayoutConHeader from "../components/Header";
import { useEffect, useState } from "react";
import { useAuth } from "../components/AuthContext";

export default function HistorialDetalle() {
  const { logout } = useAuth();
  const { id } = useParams();
  const navigate = useNavigate();
  const [historialDetalle, setHistorialDetalle] = useState(null);
  const [error, setError] = useState(null);

  const handleLogout = () => {
    logout();
    navigate("/", { replace: true });
  };

  useEffect(() => {
    async function fetchHistorialDetalle() {
      try {
        const res = await fetch(`http://localhost:5000/api/historial/${id}`);
        if (!res.ok)
          throw new Error("Error al cargar el detalle del historial");
        const data = await res.json();
        console.log("Respuesta del backend:", data);

        if (!data || typeof data !== "object") {
          throw new Error("Datos inválidos");
        }

        setHistorialDetalle(data.detalle);
      } catch (error) {
        console.error(error);
        setError("No se pudo cargar el entrenamiento");
      }
    }
    fetchHistorialDetalle();
  }, [id]);

  if (error) {
    return (
      <div className="p-6 text-center">
        <h2 className="text-2xl font-bold mb-4">{error}</h2>
        <button
          className="bg-blue-600 text-white px-4 py-2 rounded"
          onClick={() => navigate(-1)}
        >
          Volver
        </button>
      </div>
    );
  }

  if (!historialDetalle) {
    return (
      <div className="p-6 text-center">
        <p>Cargando...</p>
      </div>
    );
  }

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Historial", href: "/historial" },
  ];

  const tipoMap = {
    PieATierra: "Pie a tierra",
    Montado: "Montado",
    Jinete: "Jinete",
  };

  return (
    <div>
      <LayoutConHeader
          links={links}
          handleLogout={handleLogout}
        ></LayoutConHeader>
      <div className="max-w-3xl mx-auto p-6">
        <h1 className="text-3xl font-bold mb-6 text-center">
          Detalle del Entrenamiento
        </h1>
        <div className="bg-white shadow rounded p-6 space-y-4">
          <h2 className="text-2xl font-semibold">
            {historialDetalle.NombreEntrenamiento}
          </h2>
          <p>
            <strong>Fecha:</strong>{" "}
            {new Date(historialDetalle.Fecha).toLocaleDateString()}
          </p>
          <p>
            <strong>Tipo:</strong> {tipoMap[historialDetalle.Tipo] || historialDetalle.Tipo}
          </p>
          <p>
            <strong>Duración:</strong> {historialDetalle.Duracion} minutos
          </p>
          <p>
            <strong>Progreso:</strong> {historialDetalle.Progreso}%
          </p>
          <p>
            <strong>Notas:</strong>
            <br />
            {historialDetalle.Notas}
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
