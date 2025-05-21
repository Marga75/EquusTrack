import caballo from "../assets/horse.jpg";
import logo from "../assets/logo.webp";
import { Link } from "react-router-dom";

export default function Login() {
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
            Tu espacio para gestionar el perfil de tu mejor compañero.
            Registra, organiza y consulta fácilmente toda la información sobre tus caballos, entrenamientos y cuidados. Diseñado para jinetes, entrenadores y administradores que quieren llevar el control de forma sencilla y eficiente.
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
          <form className="space-y-4">
            <div>
              <label className="block text-sm text-slate-700 mb-1">
                Correo electrónico
              </label>
              <input
                type="text"
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400"
              />
            </div>
            <div>
              <label className="block text-sm text-slate-700 mb-1">
                Contraseña
              </label>
              <input
                type="password"
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400"
              />
            </div>
            <button
              type="submit"
              className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded transition"
            >
              Iniciar Sesión
            </button>
          </form>
          <div className="mt-4 text-center text-sm text-slate-600">
             ¿No tienes cuenta? <Link to="/registro" className="text-blue-600 underline">Regístrate aquí</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
