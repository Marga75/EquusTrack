import { useEffect, useState } from "react";
import LayoutConHeader from "../components/Header";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../components/AuthContext";

export default function Entrenamientos() {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Historial Entrenamientos", href: "/historial" },
  ];

  const handleLogout = () => {
    logout();
    navigate("/", { replace: true });
  };

  const [entrenamientos, setEntrenamientos] = useState(null);
  const [filtroTipo, setFiltroTipo] = useState("");
  const [filtroDuracion, setFiltroDuracion] = useState("");

  const entrenamientosFiltrados = (entrenamientos || []).filter((ent) => {
    const duracionMin = ent.DuracionTotalSegundos / 60; // convertir a minutos

    const filtroDuracionOk =
      filtroDuracion === "" ||
      (filtroDuracion === "0-10" && duracionMin >= 0 && duracionMin <= 10) ||
      (filtroDuracion === "20-30" && duracionMin >= 20 && duracionMin <= 30) ||
      (filtroDuracion === "40+" && duracionMin > 40);

    const filtroTipoOk = filtroTipo === "" || ent.Tipo === filtroTipo;

    return filtroTipoOk && filtroDuracionOk;
  });

  useEffect(() => {
    async function fetchEntrenamiento() {
      try {
        const res = await fetch(`http://localhost:5000/api/entrenamientos`);
        if (!res.ok)
          throw new Error("Error al cargar el detalle de los entrenamientos");
        const data = await res.json();
        console.log("Respuesta del backend:", data);

        if (data.exito) {
          setEntrenamientos(data.entrenamientos);
        } else {
          setEntrenamientos(null);
        }
      } catch (error) {
        console.error(error);
      }
    }
    fetchEntrenamiento();
  }, []);

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
            <option value="PieATierra">Pie a tierra</option>
            <option value="Montado">Montado</option>
            <option value="Jinete">Jinete</option>
          </select>

          <select
            value={filtroDuracion}
            onChange={(e) => setFiltroDuracion(e.target.value)}
            className="p-2 border rounded-lg"
          >
            <option value="">Todas las duraciones</option>
            <option value="0-10">De 0 a 10 min</option>
            <option value="20-30">De 20 a 30 min</option>
            <option value="40+">Más de 40 min</option>
          </select>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {entrenamientosFiltrados.map((ent) => (
            <div
              key={ent.Id}
              className="bg-white shadow-md rounded-xl overflow-hidden hover:shadow-xl transition"
            >
              <img
                src={ent.Imagen || "/placeholder.jpg"}
                alt={ent.Titulo}
                className="w-full h-48 object-cover"
              />
              <div className="p-4">
                <h2 className="text-xl font-semibold">{ent.Titulo}</h2>
                <p className="text-gray-600 text-sm mt-1">
                  {ent.Tipo} · {ent.DuracionTotalSegundos} min
                </p>
                <button
                  onClick={() => navigate(`/entrenamientos/${ent.Id}`)}
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
