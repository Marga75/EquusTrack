import { useState } from "react";

export default function Registro() {
  const [formData, setFormData] = useState({
    nombre: "",
    apellido: "",
    email: "",
    password: "",
    rol: "Jinete",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log("Enviando datos:", formData);

    try {
      console.log("Iniciando solicitud fetch...");
      const res = await fetch("http://localhost:5000/registrar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData),
      });
      console.log("Respuesta recibida:", res.status, res.statusText);

      // Leer el texto de la respuesta primero
      const textResponse = await res.text();
      console.log("Respuesta como texto:", textResponse);

      // Solo intentar parsear si hay contenido
      if (textResponse) {
        try {
          const result = JSON.parse(textResponse);
          console.log("Respuesta parseada:", result);
          alert(result.mensaje || "Usuario registrado correctamente");
        } catch (jsonError) {
          console.error("Error al parsear la respuesta JSON:", jsonError);
          alert("Respuesta recibida del servidor, pero no es un JSON válido");
        }
      } else {
        alert("El servidor respondió, pero sin contenido");
      }
    } catch (error) {
      console.error("Error completo:", error);
      alert(`Error al conectar con el servidor: ${error.message}`);
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10 p-6 bg-white shadow-xl rounded-2xl">
      <h2 className="text-2xl font-bold mb-4">Registro de Usuario</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        <input
          name="nombre"
          value={formData.nombre}
          onChange={handleChange}
          placeholder="Nombre"
          className="w-full p-2 border rounded"
          required
        />
        <input
          name="apellido"
          value={formData.apellido}
          onChange={handleChange}
          placeholder="Apellido"
          className="w-full p-2 border rounded"
          required
        />
        <input
          name="email"
          type="email"
          value={formData.email}
          onChange={handleChange}
          placeholder="Correo electrónico"
          className="w-full p-2 border rounded"
          required
        />
        <input
          name="password"
          type="password"
          value={formData.password}
          onChange={handleChange}
          placeholder="Contraseña"
          className="w-full p-2 border rounded"
          required
        />
        <select
          name="rol"
          value={formData.rol}
          onChange={handleChange}
          className="w-full p-2 border rounded"
        >
          <option value="Jinete">Jinete</option>
          <option value="Entrenador">Entrenador</option>
        </select>
        <button
          type="submit"
          className="w-full bg-blue-600 text-white p-2 rounded hover:bg-blue-700"
        >
          Registrarse
        </button>
      </form>
    </div>
  );
}
