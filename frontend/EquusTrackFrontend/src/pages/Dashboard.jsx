import caballo1 from "../assets/caballo1.jpg";
import userPhoto from "../assets/user.jpg";
import { Plus } from "lucide-react";
import { useAuth } from "../components/AuthContext";
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import FormularioCaballo from "../components/FormularioCaballo";
import LayoutConHeader from "../components/Header";
import SeleccionarEntrenador from "../components/SeleccionarEntrenador";
import EntrenadorPerfil from "../components/EntrenadorPerfil";
import AlumnoCaballos from "../components/AlumnoCaballos";

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

  const links = [
    { label: "Entrenamientos", href: "/entrenamientos" },
    { label: "Historial Entrenamientos", href: "/historial" },
  ];

  const [alumnos, setAlumnos] = useState([]); // Alumnos aceptados

  useEffect(() => {
    if (!usuario?.id || usuario.rol !== "Entrenador") return;

    async function fetchAlumnos() {
      try {
        const res = await fetch(
          `http://localhost:5000/relacion/alumnos/entrenador/${usuario.id}`
        );
        if (!res.ok) throw new Error("Error al cargar alumnos");
        const data = await res.json();
        setAlumnos(data);
      } catch (error) {
        console.error(error);
      }
    }

    fetchAlumnos();
  }, [usuario]);

  function calcularEdad(fechaNacimiento) {
    const nacimiento = new Date(fechaNacimiento);
    const hoy = new Date();
    let edad = hoy.getFullYear() - nacimiento.getFullYear();
    const m = hoy.getMonth() - nacimiento.getMonth();
    if (m < 0 || (m === 0 && hoy.getDate() < nacimiento.getDate())) {
      edad--;
    }
    return edad;
  }

  return (
    <div className="min-h-screen bg-slate-50">
      {/* Barra superior */}
      <LayoutConHeader
        links={links}
        handleLogout={handleLogout}
      ></LayoutConHeader>

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
              <strong>Fecha de nacimiento:</strong>{" "}
              {new Date(usuario?.fechaNacimiento).toLocaleDateString()}
            </li>
            <li>
              <strong>Edad:</strong> {calcularEdad(usuario?.fechaNacimiento)}{" "}
              años
            </li>
            <li>
              <strong>Género:</strong> {usuario?.genero}
            </li>
          </ul>

          {usuario?.rol === "Jinete" && (
            <SeleccionarEntrenador usuario={usuario} />
          )}
          {usuario?.rol === "Entrenador" && (
            <EntrenadorPerfil usuario={usuario} />
          )}
        </div>

        {/* Caballos */}
        <div className="col-span-3">
          <h3 className="mb-4 font-semibold text-xl">Tus caballos</h3>
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
            {caballos.map((c) => (
              <div
                key={c.id}
                onClick={() => navigate(`/caballo/${c.id}`)}
                className="bg-white rounded-xl shadow overflow-hidden cursor-pointer hover:scale-[1.01] transition"
              >
                <img
                  src={(c.fotoUrl || "").trim() || caballo1}
                  alt={c.nombre}
                  className="w-full h-60 object-cover"
                />
                <p className="text-center py-6 font-bold text-2xl">
                  {c.nombre}
                </p>
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

          {usuario?.rol === "Entrenador" && alumnos.length > 0 && (
            <>
              <h3 className="mt-10 mb-4 font-semibold text-xl">
                Caballos de tus alumnos
              </h3>
              {alumnos.map((alumno) => {
                console.log("Alumno recibido:", alumno);
                return (
                  <AlumnoCaballos
                    key={alumno.Id || alumno.id}
                    alumno={alumno}
                  />
                );
              })}
            </>
          )}
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
