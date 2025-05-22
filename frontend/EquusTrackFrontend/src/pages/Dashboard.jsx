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
  const [mostrarFormulario, setMostrarFormulario] = useState(false);

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

  useEffect(() => {
    if (!usuario?.id) return;
    fetchCaballos();
  }, [usuario]);

  const handleLogout = () => {
    logout();
    navigate("/", { replace: true });
  };

  const FormularioCaballo = ({ onClose, onGuardado }) => {
    const [form, setForm] = useState({
      nombre: "",
      edad: "",
      raza: "",
      fotoUrl: "",
    });

    const handleSubmit = async (e) => {
      e.preventDefault();
      try {
        const res = await fetch("http://localhost:5000/api/caballos", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            ...form,
            usuarioId: usuario.id, // Asumiendo que necesitas asociarlo
          }),
        });
        if (!res.ok) throw new Error("Error al guardar el caballo");
        await fetchCaballos();
        onGuardado();
      } catch (err) {
        console.error(err);
      }
    };

    return (
      <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
        <form
          onSubmit={handleSubmit}
          className="bg-white p-6 rounded-xl shadow w-96 space-y-4"
        >
          <h2 className="text-lg font-bold mb-2">Nuevo Caballo</h2>
          <input
            placeholder="Nombre"
            value={form.nombre}
            onChange={(e) => setForm({ ...form, nombre: e.target.value })}
            className="border w-full p-2 rounded"
            required
          />
          <input
            placeholder="Edad"
            value={form.edad}
            onChange={(e) => setForm({ ...form, edad: e.target.value })}
            className="border w-full p-2 rounded"
            required
          />
          <input
            placeholder="Raza"
            value={form.raza}
            onChange={(e) => setForm({ ...form, raza: e.target.value })}
            className="border w-full p-2 rounded"
            required
          />
          <input
            placeholder="URL de foto"
            value={form.fotoUrl}
            onChange={(e) => setForm({ ...form, fotoUrl: e.target.value })}
            className="border w-full p-2 rounded"
          />
          <div className="flex justify-end gap-2">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 rounded bg-gray-300"
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="px-4 py-2 rounded bg-blue-500 text-white"
            >
              Guardar
            </button>
          </div>
        </form>
      </div>
    );
  };

  return (
    <div className="min-h-screen bg-slate-50">
      {/* Barra superior */}
      <header className="flex flex-col items-center justify-between px-8 py-4 bg-white shadow">
        <div className="mb-4">
          <img src={logo} alt="Logo" className="w-80 mx-auto" />
        </div>
        <nav className="w-full flex items-center justify-between gap-6 text-slate-700 font-medium">
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
            {usuario?.nombre} {usuario?.apellido}
          </h2>
          <p className="text-slate-500 text-sm capitalize mb-2">
            {usuario?.rol}
          </p>
          <ul className="mt-2 text-sm text-slate-600 space-y-1 text-left w-full">
            <li>
              <strong>Email:</strong> {usuario?.email}
            </li>
            <li>
              <strong>Fecha de nacimiento:</strong> {usuario?.fechaNacimiento}
            </li>
            <li>
              <strong>Género:</strong> {usuario?.genero}
            </li>
          </ul>
        </div>

        {/* Caballos */}
        <div className="col-span-3 grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {caballos.length === 0 &&
            [caballo1, caballo2, caballo3].map((src, i) => (
              <div
                key={i}
                className="bg-white rounded-xl shadow overflow-hidden"
              >
                <img
                  src={src}
                  alt={`Caballo ${i + 1}`}
                  className="w-full h-60 object-cover"
                />
                <p className="text-center py-1 font-medium">Caballo {i + 1}</p>
              </div>
            ))}

          {caballos.map((caballo) => (
            <div
              key={caballo.id}
              onClick={() => navigate(`/caballo/${caballo.id}`)}
              className="bg-white rounded-xl shadow overflow-hidden cursor-pointer hover:scale-[1.01] transition"
            >
              <img
                src={caballo.fotoUrl || caballo1}
                alt={caballo.nombre}
                className="w-full h-60 object-cover"
              />
              <p className="text-center py-1 font-medium">{caballo.nombre}</p>
            </div>
          ))}

          {/* Botón agregar */}
          <div
            onClick={() => setMostrarFormulario(true)}
            className="bg-white rounded-xl shadow flex items-center justify-center h-48 cursor-pointer hover:bg-slate-100 transition"
          >
            <Plus className="w-10 h-10 text-slate-400" />
          </div>
        </div>
      </main>

      {/* Modal formulario */}
      {mostrarFormulario && (
        <FormularioCaballo
          usuarioId={usuario.id}
          onClose={() => setMostrarFormulario(false)}
          onGuardado={() => {
            fetchCaballos();
            setMostrarFormulario(false);
          }}
        />
      )}
    </div>
  );
}
