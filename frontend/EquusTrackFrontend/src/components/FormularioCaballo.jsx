import { useState } from "react";

export default function FormularioCaballo({ usuarioId, onClose, onGuardado }) {
  const [form, setForm] = useState({
    nombre: "",
    fechaNacimiento: "",
    fechaAdopcion: "",
    raza: "",
    color: "",
  });
  const [fotoArchivo, setFotoArchivo] = useState(null);
  const [previewUrl, setPreviewUrl] = useState("");

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
    data.append("nombre", form.nombre);
    data.append("fechaNacimiento", form.fechaNacimiento);
    data.append("fechaAdopcion", form.fechaAdopcion);
    data.append("raza", form.raza);
    data.append("color", form.color);
    data.append("idUsuario", usuarioId);
    if (fotoArchivo) {
      data.append("fotoUrl", fotoArchivo);
    }

    try {
      const res = await fetch("http://localhost:5000/api/caballos", {
        method: "POST",
        body: data,
      });

      if (!res.ok) throw new Error("Error al guardar el caballo");

      onGuardado();
      onClose();
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <form
        onSubmit={handleSubmit}
        className="bg-white p-6 rounded-xl shadow w-96 space-y-4"
        encType="multipart/form-data"
      >
        <h2 className="text-lg font-bold mb-2">Nuevo Caballo</h2>

        <input
          type="text"
          placeholder="Nombre"
          value={form.nombre}
          onChange={(e) => setForm({ ...form, nombre: e.target.value })}
          className="border w-full p-2 rounded"
          required
        />

        <label className="block text-sm font-medium">Fecha de nacimiento</label>
        <input
          type="date"
          value={form.fechaNacimiento}
          onChange={(e) => setForm({ ...form, fechaNacimiento: e.target.value })}
          className="border w-full p-2 rounded"
          required
        />

        <label className="block text-sm font-medium">Fecha de adopción</label>
        <input
          type="date"
          value={form.fechaAdopcion}
          onChange={(e) => setForm({ ...form, fechaAdopcion: e.target.value })}
          className="border w-full p-2 rounded"
          required
        />

        <input
          type="text"
          placeholder="Raza"
          value={form.raza}
          onChange={(e) => setForm({ ...form, raza: e.target.value })}
          className="border w-full p-2 rounded"
          required
        />

        <input
          type="text"
          placeholder="Color"
          value={form.color}
          onChange={(e) => setForm({ ...form, color: e.target.value })}
          className="border w-full p-2 rounded"
        />

        {/* Input para cargar la foto */}
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
}
