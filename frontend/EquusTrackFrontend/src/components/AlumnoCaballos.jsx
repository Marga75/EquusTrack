import { useState, useEffect } from "react";
import caballo1 from "../assets/caballo1.jpg";

export default function AlumnoCaballos({ alumno }) {
  const [caballos, setCaballos] = useState([]);
  const [mostrar, setMostrar] = useState(false);

  useEffect(() => {
    async function fetchCaballosAlumno() {
      try {
        const res = await fetch(`http://localhost:5000/api/caballos/usuario/${alumno.id}`);
        if (!res.ok) throw new Error("Error cargando caballos del alumno");
        const data = await res.json();
        const rawCaballos = data.caballos || data;
        const caballosNormalizados = rawCaballos.map((c) => ({
          id: c.Id ?? c.id,
          nombre: c.Nombre ?? c.nombre,
          fotoUrl: c.FotoUrl ?? c.fotoUrl,
          edad: c.Edad ?? c.edad,
          raza: c.Raza ?? c.raza,
          color: c.Color ?? c.color,
        }));
        setCaballos(caballosNormalizados);
      } catch (error) {
        console.error(error);
      }
    }

    fetchCaballosAlumno();
  }, [alumno.id]);

  return (
    <div className="mb-4 border rounded p-3">
      <h4
        className="cursor-pointer font-semibold mb-2"
        onClick={() => setMostrar(!mostrar)}
      >
        {alumno.nombre} {alumno.apellido} {mostrar ? "▲" : "▼"}
      </h4>

      {mostrar && (
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {caballos.length === 0 && <p>Este alumno no tiene caballos</p>}
          {caballos.map((caballo) => (
            <div key={caballo.id} className="bg-white rounded shadow cursor-pointer hover:scale-[1.01] transition">
              <img
                src={(caballo.fotoUrl || "").trim() || caballo1}
                alt={caballo.nombre}
                className="w-full h-40 object-cover"
              />
              <p className="text-center py-2 font-bold text-lg">{caballo.nombre}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
