function FormularioCaballo({ usuarioId, onClose, onGuardado }) {
  const [form, setForm] = useState({ nombre: "", edad: "", raza: "", fotoUrl: "" });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const res = await fetch("http://localhost:5000/api/caballos", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({form, usuarioId: usuarioId}),
      });
      if (!res.ok) throw new Error("Error al guardar el caballo");
      onGuardado(); // Actualiza el dashboard
      onClose(); // Cierra el formulario
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="bg-white p-6 rounded shadow space-y-4">
      <input placeholder="Nombre" value={form.nombre} onChange={e => setForm({ ...form, nombre: e.target.value })} className="border w-full p-2 rounded" required />
      <input placeholder="Edad" value={form.edad} onChange={e => setForm({ ...form, edad: e.target.value })} className="border w-full p-2 rounded" required />
      <input placeholder="Raza" value={form.raza} onChange={e => setForm({ ...form, raza: e.target.value })} className="border w-full p-2 rounded" required />
      <input placeholder="URL de foto" value={form.fotoUrl} onChange={e => setForm({ ...form, fotoUrl: e.target.value })} className="border w-full p-2 rounded" />
      <button className="bg-blue-500 text-white px-4 py-2 rounded">Guardar</button>
    </form>
  );
}
