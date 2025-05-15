import { useState } from "react";
import caballo from "../assets/placeholder-horse.jpg";

export default function Login({ onLoginSuccess }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

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
        alert("Inicio de sesión correcto");
        onLoginSuccess(data); // puede pasar datos como ID, rol, etc.
      } else {
        alert("Credenciales incorrectas");
      }
    } catch (error) {
      alert("Error al iniciar sesión.");
    }
  };

  return (
    <div className="flex h-screen">
      <div className="md:block w-4/5 relative overflow-hidden bg-red-100">
      <div className="relative">
        <img
          className="w-full h-full object-cover scale-90"
          src={caballo}
          alt="caballo"
        />
        <div
          className="absolute bottom-0 left-0 w-full h-24 bg-gradient-to-t from-white to-transparent pointer-events-none"></div>
        </div>
        <div class="relative z-10 p-8">
          <p class="text-lg text-gray-700">
            Este es un ejemplo de contenido debajo de la imagen, con un
            difuminado que mezcla la imagen con el fondo blanco.
          </p>
        </div>
      </div>

      <div className="w-[50px]">
        <p></p>
      </div>

      <div
        className="w-full max-w-xs md:w-2/5 flex items-center justify-center p-4"
        style={{ maxWidth: "320px" }}
      >
        <div className="w-full bg-white shadow-xl rounded-2xl p-6">
          <h2 className="text-2xl font-bold mb-4">Iniciar Sesión</h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <input
              type="email"
              placeholder="Correo electrónico"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full p-2 border rounded"
              required
            />
            <input
              type="password"
              placeholder="Contraseña"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full p-2 border rounded"
              required
            />
            <button
              type="submit"
              className="w-full bg-green-600 text-white p-2 rounded hover:bg-green-700"
            >
              Iniciar sesión
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
