import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";

export default function CaballoDetalle() {
  const { id } = useParams();
  const [caballo, setCaballo] = useState(null);

  useEffect(() => {
    async function fetchCaballo() {
      try {
        const res = await fetch(`http://localhost:5000/api/caballos/${id}`);
        const data = await res.json();
        setCaballo(data);
      } catch (err) {
        console.error("Error cargando el caballo", err);
      }
    }
    fetchCaballo();
  }, [id]);

  if (!caballo) return <div className="p-8">Cargando...</div>;

  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold mb-4">{caballo.nombre}</h1>
      <p><strong>Edad:</strong> {caballo.edad}</p>
      <p><strong>Raza:</strong> {caballo.raza}</p>
      {/* Datos del veterinario y herrador */}
      <p><strong>Veterinario:</strong> {caballo.veterinario}</p>
      <p><strong>Herrador:</strong> {caballo.herrador}</p>
      {/* Muestra las herradas (si tienes una lista) */}
      <h2 className="mt-4 font-semibold">Herradas:</h2>
      <ul className="list-disc ml-5">
        {caballo.herradas?.map((h, i) => (
          <li key={i}>{h.fecha} - {h.descripcion}</li>
        ))}
      </ul>
    </div>
  );
}
