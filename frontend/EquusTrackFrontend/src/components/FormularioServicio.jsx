import { useState } from "react";

export default function FormularioServicio({
  tipo,
  onClose,
  idCaballo,
  onInsert,
}) {
  const [nombre, setNombre] = useState("");
  const [descripcion, setDescripcion] = useState("");
  const [costo, setCosto] = useState("");
  const [fecha, setFecha] = useState("");
  const [loading, setLoading] = useState(false);

  // Textos personalizados según el tipo
  const campos = {
    herrador: {
      nombre: "Nombre del herrador",
      descripcion: "", // no se usa
      mostrarDescripcion: false,
    },
    veterinario: {
      nombre: "Nombre del veterinario",
      descripcion: "Tratamiento realizado",
      mostrarDescripcion: true,
    },
    fisio: {
      nombre: "Nombre del fisioterapeuta",
      descripcion: "Descripción de la sesión",
      mostrarDescripcion: true,
    },
  };

  async function handleSubmit(e) {
    e.preventDefault();
    setLoading(true);

    const urlBase = "http://localhost:5000/api/caballos";
    const endpoint = `${urlBase}/${idCaballo}/${tipo}`;

    // Construcción del cuerpo según tipo
    let bodyData = {
      nombre,
      costo: parseFloat(costo),
      fecha, // la fecha siempre la enviamos
      idCaballo,
    };

    // Para veterinario y fisio añadimos descripcion
    if (campos[tipo].mostrarDescripcion) {
      bodyData.descripcion = descripcion;
    }

    // Para herrador el nombre debe enviarse con clave "herradorNombre"
    if (tipo === "herrador") {
      bodyData = {
        idCaballo,
        fecha,
        herradorNombre: nombre,
        costo: parseFloat(costo),
      };
    } else if (tipo === "veterinario") {
      bodyData = {
        idCaballo,
        fecha,
        descripcion,
        veterinarioNombre: nombre,
        costo: parseFloat(costo),
      };
    } else if (tipo === "fisio") {
      bodyData = {
        idCaballo,
        fecha,
        descripcion,
        profesional: nombre,
        costo: parseFloat(costo),
      };
    }

    try {
      const res = await fetch(endpoint, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(bodyData),
      });

      const data = await res.json();

      if (data.exito) {
        alert(
          `${tipo.charAt(0).toUpperCase() + tipo.slice(1)} guardado con éxito`
        );
        onInsert && onInsert();
        onClose();
      } else {
        alert("Error al guardar");
      }
    } catch (error) {
      alert("Error en la conexión");
      console.error(error);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex justify-center items-center">
      <form
        onSubmit={handleSubmit}
        className="bg-white p-6 rounded shadow max-w-sm w-full"
      >
        <h2 className="text-xl font-semibold mb-4">
          Añadir {tipo.charAt(0).toUpperCase() + tipo.slice(1)}
        </h2>

        <label className="block mb-2">
          Fecha:
          <input
            type="date"
            className="border p-2 w-full mt-1"
            value={fecha}
            onChange={(e) => setFecha(e.target.value)}
            required
          />
        </label>

        <label className="block mb-2">
          {campos[tipo].nombre}:
          <input
            type="text"
            className="border p-2 w-full mt-1"
            value={nombre}
            placeholder={campos[tipo].nombre}
            onChange={(e) => setNombre(e.target.value)}
            required
          />
        </label>

        <label className="block mb-2">
          {campos[tipo].descripcion || "Descripción"}:
          <textarea
            className="border p-2 w-full mt-1"
            value={descripcion}
            placeholder={campos[tipo].descripcion}
            onChange={(e) => setDescripcion(e.target.value)}
            required={campos[tipo].mostrarDescripcion}
            disabled={!campos[tipo].mostrarDescripcion}
          />
        </label>

        <label className="block mb-4">
          Costo:
          <input
            type="number"
            step="0.01"
            min="0"
            className="border p-2 w-full mt-1"
            value={costo}
            placeholder="Costo"
            onChange={(e) => setCosto(e.target.value)}
            required
          />
        </label>

        <div className="flex justify-end gap-2">
          <button
            type="button"
            className="px-4 py-2 bg-gray-300 rounded"
            onClick={onClose}
          >
            Cancelar
          </button>
          <button
            type="submit"
            className="px-4 py-2 bg-blue-600 text-white rounded"
            disabled={loading}
          >
            {loading ? "Guardando..." : "Guardar"}
          </button>
        </div>
      </form>
    </div>
  );
}
