import caballo1 from "../assets/caballo1.jpg";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export default function CaballoDetalle() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [caballo, setCaballo] = useState(null);

  useEffect(() => {
    // Aquí llamas a tu API para obtener el detalle del caballo por id
    async function fetchCaballoDetalle() {
      try {
        const res = await fetch(`http://localhost:5000/api/caballos/${id}`);
        if (!res.ok) throw new Error("Error al cargar el detalle del caballo");
        const data = await res.json();
        setCaballo(data);
      } catch (error) {
        console.error(error);
      }
    }
    fetchCaballoDetalle();
  }, [id]);

  if (!caballo) return <p>Cargando detalles...</p>;

  return (
    <div className="p-8 max-w-3xl mx-auto">
      <button
        className="mb-4 text-blue-600 underline"
        onClick={() => navigate("/dashboard")}
      >
        ← Volver al Dashboard
      </button>

      <h1 className="text-3xl font-bold mb-6">{caballo.nombre}</h1>

      <img
        src={caballo.fotoUrl?.trim() || caballo1}
        alt={caballo.nombre}
        className="w-full max-w-md object-cover rounded mb-6"
      />

      <div className="text-lg space-y-2">
        <p>
          <strong>Edad:</strong> {caballo.edad}
        </p>
        <p>
          <strong>Raza:</strong> {caballo.raza}
        </p>
        <p>
          <strong>Color:</strong> {caballo.color}
        </p>
        {/* Aquí más detalles si quieres */}
      </div>
    </div>
  );
}
