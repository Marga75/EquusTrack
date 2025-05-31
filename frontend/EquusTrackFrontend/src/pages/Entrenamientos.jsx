import { useState } from "react";
import LayoutConHeader from "../components/Header";
import { useNavigate } from "react-router-dom";

const entrenamientosMock = [
  {
    id: 1,
    nombre: "Resistencia Básica",
    tipo: "Pie a tierra",
    duracion: 30,
    imagen: "https://via.placeholder.com/400x200?text=Resistencia",
  },
  {
    id: 2,
    nombre: "Técnica de Salto",
    tipo: "Montado",
    duracion: 20,
    imagen: "https://via.placeholder.com/400x200?text=Salto",
  },
  {
    id: 3,
    nombre: "Entrenamiento Combinado",
    tipo: "Jinete",
    duracion: 45,
    imagen: "https://via.placeholder.com/400x200?text=Combinado",
  },
];

export default function Entrenamientos() {
  const navigate = useNavigate();

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Historial Entrenamientos", href: "/historial" },
  ];

  const handleLogout = () => {
    logout();
    navigate("/", { replace: true });
  };

  const [filtroTipo, setFiltroTipo] = useState("");
  const [filtroDuracion, setFiltroDuracion] = useState("");

  const entrenamientosFiltrados = entrenamientosMock.filter((ent) => {
    return (
      (filtroTipo === "" || ent.tipo === filtroTipo) &&
      (filtroDuracion === "" || ent.duracion <= parseInt(filtroDuracion))
    );
  });

  return (
    <div>
      <div>
        {/* Barra superior */}
        <LayoutConHeader
          links={links}
          handleLogout={handleLogout}
        ></LayoutConHeader>
      </div>
      <div className="p-6">
        <h1 className="text-3xl font-bold mb-4 text-center">Entrenamientos</h1>

        <div className="flex flex-col md:flex-row gap-4 mb-6">
          <select
            value={filtroTipo}
            onChange={(e) => setFiltroTipo(e.target.value)}
            className="p-2 border rounded-lg"
          >
            <option value="">Todos los tipos</option>
            <option value="Pie a tierra">Pie a tierra</option>
            <option value="Montado">Montado</option>
            <option value="Jinete">Jinete</option>
          </select>

          <select
            value={filtroDuracion}
            onChange={(e) => setFiltroDuracion(e.target.value)}
            className="p-2 border rounded-lg"
          >
            <option value="">Todas las duraciones</option>
            <option value="20">Hasta 20 min</option>
            <option value="30">Hasta 30 min</option>
            <option value="45">Hasta 45 min</option>
          </select>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {entrenamientosFiltrados.map((ent) => (
            <div
              key={ent.id}
              className="bg-white shadow-md rounded-xl overflow-hidden hover:shadow-xl transition"
            >
              <img
                src={ent.imagen}
                alt={ent.nombre}
                className="w-full h-48 object-cover"
              />
              <div className="p-4">
                <h2 className="text-xl font-semibold">{ent.nombre}</h2>
                <p className="text-gray-600 text-sm mt-1">
                  {ent.tipo} · {ent.duracion} min
                </p>
                <button 
                onClick={() => navigate(`/entrenamientos/${ent.id}`)}
                className="mt-4 w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition"
                >
                  Ver Detalles
                </button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
