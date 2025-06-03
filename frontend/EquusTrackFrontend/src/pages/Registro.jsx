import { useState, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";

export default function Registro() {
  const [formData, setFormData] = useState({
    nombre: "",
    apellido: "",
    email: "",
    password: "",
    rol: "Jinete",
    fechaNacimiento: "",
    genero: "Masculino",
  });

  const [fotoArchivo, setFotoArchivo] = useState(null);
  const [previewUrl, setPreviewUrl] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    setError("");
  }, [formData, fotoArchivo]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleFotoChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setFotoArchivo(file);
      setPreviewUrl(URL.createObjectURL(file));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const data = new FormData();
    data.append("nombre", formData.nombre);
    data.append("apellido", formData.apellido);
    data.append("email", formData.email);
    data.append("password", formData.password);
    data.append("rol", formData.rol);
    data.append("fechaNacimiento", formData.fechaNacimiento);
    data.append("genero", formData.genero);

    if (fotoArchivo) {
      data.append("foto", fotoArchivo);
    }

    try {
      const res = await fetch("http://localhost:5000/registrar", {
        method: "POST",
        body: data,
      });

      const text = await res.text();
      const result = text ? JSON.parse(text) : null;

      if (result?.exito) {
        navigate("/", { state: { mensaje: "Usuario registrado con éxito" } });
      } else {
        setError(result?.mensaje || "Error al registrar usuario");
      }
    } catch (error) {
      console.error("Error al registrar:", error);
      setError("Error al conectar con el servidor");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-100 px-4">
      <div className="bg-white p-8 rounded-2xl shadow-lg w-full max-w-md">
        <h2 className="text-2xl font-bold text-center mb-6 text-slate-800">
          Crea tu cuenta
        </h2>
        <form className="space-y-4" onSubmit={handleSubmit}>
          <div className="flex gap-2">
            <input
              name="nombre"
              value={formData.nombre}
              onChange={handleChange}
              type="text"
              placeholder="Nombre"
              required
              className="w-1/2 px-3 py-2 border border-gray-300 rounded-md"
            />
            <input
              name="apellido"
              value={formData.apellido}
              onChange={handleChange}
              type="text"
              placeholder="Apellido"
              required
              className="w-1/2 px-3 py-2 border border-gray-300 rounded-md"
            />
          </div>
          <input
            name="email"
            value={formData.email}
            onChange={handleChange}
            type="email"
            placeholder="Correo electrónico"
            required
            className="w-full px-3 py-2 border border-gray-300 rounded-md"
          />
          <input
            name="password"
            value={formData.password}
            onChange={handleChange}
            type="password"
            placeholder="Contraseña"
            required
            className="w-full px-3 py-2 border border-gray-300 rounded-md"
          />
          <select
            name="rol"
            value={formData.rol}
            onChange={handleChange}
            required
            className="w-full px-3 py-2 border border-gray-300 rounded-md text-slate-500"
          >
            <option value="">Rol</option>
            <option value="Jinete">Jinete</option>
            <option value="Entrenador">Entrenador</option>
          </select>
          <input
            name="fechaNacimiento"
            value={formData.fechaNacimiento}
            onChange={handleChange}
            type="date"
            required
            className="w-full px-3 py-2 border border-gray-300 rounded-md text-slate-500"
          />
          <select
            name="genero"
            value={formData.genero}
            onChange={handleChange}
            required
            className="w-full px-3 py-2 border border-gray-300 rounded-md text-slate-500"
          >
            <option value="">Género</option>
            <option value="Masculino">Masculino</option>
            <option value="Femenino">Femenino</option>
            <option value="Otro">Otro</option>
          </select>

          {/* Campo para cargar la foto */}
          <div>
            <input
              type="file"
              accept="image/*"
              onChange={handleFotoChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md"
            />
            {previewUrl && (
              <img
                src={previewUrl}
                alt="Vista previa"
                className="mt-2 w-24 h-24 object-cover rounded-full mx-auto"
              />
            )}
          </div>

          <button
            type="submit"
            className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded transition"
          >
            Registrarse
          </button>
          {error && (
            <p className="text-red-600 text-sm text-center mt-2">{error}</p>
          )}
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
