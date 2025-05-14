import { useEffect, useState } from "react";

export default function PantallaPrincipal({ usuario }) {
  const [caballos, setCaballos] = useState([]);

  useEffect(() => {
    const cargarCaballos = async () => {
      try {
        const res = await fetch("http://localhost:5000/caballos", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ idUsuario: usuario.id, rol: usuario.rol }),
        });

        const data = await res.json();
        setCaballos(data.caballos);
      } catch (error) {
        alert("Error al cargar los caballos");
      }
    };

    cargarCaballos();
  }, [usuario]);

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Bienvenido, {usuario.nombre}</h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
        {caballos.map((caballo) => (
          <div
            key={caballo.id}
            className="bg-white p-4 rounded-xl shadow-md hover:shadow-xl transition"
          >
            <img
              src={caballo.foto || "/placeholder-horse.jpg"}
              alt={caballo.nombre}
              className="w-full h-40 object-cover rounded-xl mb-3"
            />
            <h2 className="text-lg font-semibold">{caballo.nombre}</h2>
          </div>
        ))}
      </div>
    </div>
  );
}
