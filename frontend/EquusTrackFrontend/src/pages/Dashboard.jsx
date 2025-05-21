import caballo1 from "../assets/caballo1.jpg";
import caballo2 from "../assets/caballo2.webp";
import caballo3 from "../assets/caballo3.jpg";
import userPhoto from "../assets/user.jpg";
import logo from "../assets/logo.webp";
import { Plus } from "lucide-react";
import { useAuth } from "../components/AuthContext";
import { useNavigate } from "react-router-dom";

export default function Dashboard() {
  const { usuario, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  return (
    <div className="min-h-screen bg-slate-50">
      {/* Barra superior */}
      <header className="flex items-center justify-between px-8 py-4 bg-white shadow">
        <div className="flex items-center gap-3">
          <img src={logo} alt="Logo" className="w-10 h-10" />
        </div>
        <nav className="flex gap-6 text-slate-700 font-medium items-center">
          <a href="#" className="hover:text-blue-600">Inicio</a>
          <a href="#" className="hover:text-blue-600">Mis Caballos</a>
          <a href="#" className="hover:text-blue-600">Entrenamientos</a>
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

        {/* Caballos */}
        <div className="col-span-3 grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {/* Tarjetas */}
          {[{ src: caballo1, nombre: "Bella" }, { src: caballo2, nombre: "Luna" }, { src: caballo3, nombre: "Rocky" }].map((caballo, i) => (
            <div key={i} className="bg-white rounded-xl shadow overflow-hidden">
              <img src={caballo.src} alt={caballo.nombre} className="w-full h-40 object-cover" />
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
