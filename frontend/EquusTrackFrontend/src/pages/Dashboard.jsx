import caballo1 from "../assets/caballo1.jpg";
import caballo2 from "../assets/caballo2.webp";
import caballo3 from "../assets/caballo3.jpg";
import userPhoto from "../assets/user.jpg";
import logo from "../assets/logo.webp";
import { Plus } from "lucide-react";
import { useAuth } from "../components/AuthContext";
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";

export default function Dashboard() {
  const { usuario, logout } = useAuth();
  const navigate = useNavigate();

  const [caballos, setCaballos] = useState([]);

  useEffect(() => {
    if (!usuario?.id) return; // si no hay usuario, no hace nada

    async function fetchCaballos() {
      try {
        const res = await fetch(
          `http://localhost:5000/api/caballos/usuario/${usuario.id}`
        );
        if (!res.ok) throw new Error("Error al cargar caballos");
        const data = await res.json();
        setCaballos(data.caballos || []);
      } catch (error) {
        console.error("Error cargando caballos:", error);
      }
    }

    fetchCaballos();
  }, [usuario]);

  const handleLogout = () => {
    logout();
    navigate("/", { replace: true });
  };

  return (
    <div className="min-h-screen bg-slate-50">
      {/* Barra superior */}
      <header className="flex flex-col items-center justify-between px-8 py-4 bg-white shadow">
        {/* Logo arriba centrado */}
        <div className="mb-4">
          <img src={logo} alt="Logo" className="w-80 mx-auto" />
        </div>
        {/* Navegación */}
        <nav className="w-full flex items-center justify-between gap-6 text-slate-700 font-medium">
          {/* Links centrados */}
          <div className="flex gap-6 mx-auto">
            <a href="#" className="hover:text-blue-600">
              Inicio
            </a>
            <a href="#" className="hover:text-blue-600">
              Mis Caballos
            </a>
            <a href="#" className="hover:text-blue-600">
              Entrenamientos
            </a>
          </div>

          {/* Botón a la derecha */}
          <button
            onClick={handleLogout}
            className="ml-4 bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded text-sm"
          >
            Cerrar sesión
          </button>
        </nav>
      </header>

      {/* Contenido principal */}
      <main className="p-8 grid grid-cols-1 md:grid-cols-4 gap-6">
        {/* Perfil usuario */}
        <div className="col-span-1 bg-white rounded-xl shadow p-6 flex flex-col items-center text-center">
          <img
            src={userPhoto}
            alt="Usuario"
            className="w-24 h-24 rounded-full mb-4 object-cover"
          />
          <h2 className="text-lg font-bold text-slate-800">
            {usuario?.nombre || "Usuario"}
          </h2>
          <p className="text-slate-500 text-sm capitalize">
            {usuario?.rol || "jinete"}
          </p>
        </div>

        {/* Caballos 
        <div className="col-span-3 grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          Tarjetas 
          {[
            { src: caballo1, nombre: "Bella" },
            { src: caballo2, nombre: "Luna" },
            { src: caballo3, nombre: "Rocky" },
          ].map((caballo, i) => (
            <div key={i} className="bg-white rounded-xl shadow overflow-hidden">
              <img
                src={caballo.src}
                alt={caballo.nombre}
                className="w-full h-40 object-cover"
              />
              <p className="text-center py-2 font-medium">{caballo.nombre}</p>
            </div>
          ))} */}

        {/* Caballos */}
        <div className="col-span-3 grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {/* Si no hay caballos, muestra una imagen fija (opcional) */}
          {caballos.length === 0 && (
            <>
              {[caballo1, caballo2, caballo3].map((src, i) => (
                <div
                  key={i}
                  className="bg-white rounded-xl shadow overflow-hidden"
                >
                  <img
                    src={src}
                    alt={`Caballo ${i + 1}`}
                    className="w-full h-40 object-cover"
                  />
                  <p className="text-center py-2 font-medium">
                    Caballo {i + 1}
                  </p>
                </div>
              ))}
            </>
          )}

          {/* Mapea los caballos traídos del backend */}
          {caballos.map((caballo) => (
            <div
              key={caballo.id}
              className="bg-white rounded-xl shadow overflow-hidden"
            >
              <img
                src={caballo.fotoUrl || caballo1} // o cualquier fallback que tengas
                alt={caballo.nombre}
                className="w-full h-40 object-cover"
              />
              <p className="text-center py-2 font-medium">{caballo.nombre}</p>
            </div>
          ))}

          {/* Añadir nuevo */}
          <div className="bg-white rounded-xl shadow flex items-center justify-center h-48 cursor-pointer hover:bg-slate-100 transition">
            <Plus className="w-10 h-10 text-slate-400" />
          </div>
        </div>
      </main>
    </div>
  );
}
