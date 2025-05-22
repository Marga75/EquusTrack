import caballo from "../assets/horse.jpg";
import logo from "../assets/logo.webp";
import { Link, useLocation } from "react-router-dom";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../components/AuthContext";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const { usuario, login } = useAuth();

  const location = useLocation();
  const [mensajeExito, setMensajeExito] = useState("");

  useEffect(() => {
    // Si hay mensaje en el estado de la navegación, mostrarlo temporalmente
    if (location.state?.mensaje) {
      setMensajeExito(location.state.mensaje);
      // Borra el mensaje después de 3 segundos
      const timer = setTimeout(() => setMensajeExito(""), 5000);
      return () => clearTimeout(timer);
    }
  }, [location.state]);

  // Redirige automáticamente si ya hay un usuario logueado
  useEffect(() => {
    if (usuario) {
      navigate("/dashboard");
    }
  }, [usuario, navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const res = await fetch("http://localhost:5000/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });

      const data = await res.json();

      if (data.exito) {
        login(data.usuario);
        navigate("/dashboard"); // redirige al dashboard si inicia sesión
      } else {
        setError("Credenciales incorrectas");
      }
    } catch (error) {
      setError("Error al conectar con el servidor");
    }
  };

  return (
    <div className="flex h-screen font-sans">
      <div className="w-1/2 relative bg-black">
        <img
          src={caballo}
          alt="Caballo"
          className="h-full w-full object-cover opacity-80"
        />
        <div className="absolute bottom-10 left-8 right-8 text-white text-center backdrop-blur-sm bg-black/40 p-4 rounded">
          <p className="text-sm leading-relaxed">
            Bienvenido a nuestra plataforma.
            <br></br>
            Tu espacio para gestionar el perfil de tu mejor compañero. Registra,
            organiza y consulta fácilmente toda la información sobre tus
            caballos, entrenamientos y cuidados. Diseñado para jinetes,
            entrenadores y administradores que quieren llevar el control de
            forma sencilla y eficiente.
          </p>
        </div>
      </div>

      <div className="w-1/2 flex items-center justify-center bg-gradient-to-br from-slate-200/70 to-slate-400/60 backdrop-blur-md">
        <div className="max-w-sm w-full p-8 bg-white/10 backdrop-blur rounded-lg shadow-md">
          <div className="flex items-center gap-2 justify-center mb-6">
            <img src={logo} alt="logo" />
          </div>
          <h2 className="text-xl font-semibold text-center mb-4">
            Iniciar Sesión
          </h2>

          {mensajeExito && (
            <div
              className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-4 text-center transition-opacity duration-500"
              style={{ opacity: mensajeExito ? 1 : 0 }}
              role="alert"
            >
              {mensajeExito}
            </div>
          )}

          <form className="space-y-4" onSubmit={handleSubmit}>
            <div>
              <label className="block text-sm text-slate-700 mb-1">
                Correo electrónico
              </label>
              <input
                type="text"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400"
              />
            </div>
            <div>
              <label className="block text-sm text-slate-700 mb-1">
                Contraseña
              </label>
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400"
              />
            </div>

            {error && <p className="text-red-600 text-sm">{error}</p>}

            <button
              type="submit"
              className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded transition"
            >
              Iniciar Sesión
            </button>
          </form>
          <div className="mt-4 text-center text-sm text-slate-600">
            ¿No tienes cuenta?{" "}
            <Link to="/registro" className="text-blue-600 underline">
              Regístrate aquí
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
