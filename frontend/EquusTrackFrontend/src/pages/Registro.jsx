import { Link } from "react-router-dom";

export default function Registro() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-100 px-4">
      <div className="bg-white p-8 rounded-2xl shadow-lg w-full max-w-md">
        <h2 className="text-2xl font-bold text-center mb-6 text-slate-800">
          Crea tu cuenta
        </h2>
        <form className="space-y-4">
          <div className="flex gap-2">
            <input
              type="text"
              placeholder="Nombre"
              className="w-1/2 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400"
            />
            <input
              type="text"
              placeholder="Apellido"
              className="w-1/2 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400"
            />
          </div>
          <input
            type="email"
            placeholder="Correo electrónico"
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400"
          />
          <input
            type="password"
            placeholder="Contraseña"
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400"
          />
          <select
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400 text-slate-500"
          >
            <option value="">Rol</option>
            <option value="jinete">Jinete</option>
            <option value="entrenador">Entrenador</option>
          </select>
          <input
            type="date"
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400 text-slate-500"
          />
          <select
            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring focus:border-blue-400 text-slate-500"
          >
            <option value="">Género</option>
            <option value="jinete">Jinete</option>
            <option value="amazona">Amazona</option>
          </select>
          <button
            type="submit"
            className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded transition"
          >
            Registrarse
          </button>
        </form>
        <div className="mt-4 text-center text-sm text-slate-600">
          ¿Ya tienes una cuenta?{" "}
          <Link to="/" className="text-blue-600 underline">
            Inicia sesión
          </Link>
        </div>
      </div>
    </div>
  );
}
