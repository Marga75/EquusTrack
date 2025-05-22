import { useState } from "react";

export default function FormularioCaballo({ usuarioId, onClose, onGuardado }) {
  const [form, setForm] = useState({
    nombre: "",
    edad: "",
    raza: "",
    color: "",
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
          usuarioId: usuarioId,
        }),
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
          placeholder="Color"
          value={form.color}
          onChange={(e) => setForm({ ...form, color: e.target.value })}
          className="border w-full p-2 rounded"
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
}
