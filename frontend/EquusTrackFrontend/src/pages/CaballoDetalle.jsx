import caballo1 from "../assets/caballo1.jpg";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export default function CaballoDetalle() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [caballo, setCaballo] = useState(null);

  useEffect(() => {
    // Aquí llamas a tu API para obtener el detalle del caballo por id
    async function fetchCaballoDetalle() {
      try {
        const res = await fetch(`http://localhost:5000/api/caballos/${id}`);
        if (!res.ok) throw new Error("Error al cargar el detalle del caballo");
        const data = await res.json();
        console.log("Respuesta del backend:", data);

        if (data.exito && data.caballo) {
          const c = data.caballo;
          setCaballo({
            id: c.Id,
            nombre: c.Nombre,
            fechaNacimiento: c.FechaNacimiento,
            raza: c.Raza,
            color: c.Color,
            fechaAdopcion: c.FechaAdopcion,
            fotoUrl: c.FotoUrl,
            idEntrenador: c.IdEntrenador,
            idUsuario: c.IdUsuario,
          });
        } else {
          setCaballo(null);
        }
      } catch (error) {
        console.error(error);
      }
    }
    fetchCaballoDetalle();
  }, [id]);

  if (!caballo) return <p>Cargando detalles...</p>;

  function calcularEdad(fechaNacimiento) {
    const nacimiento = new Date(fechaNacimiento);
    const hoy = new Date();
    let edad = hoy.getFullYear() - nacimiento.getFullYear();
    const m = hoy.getMonth() - nacimiento.getMonth();
    if (m < 0 || (m === 0 && hoy.getDate() < nacimiento.getDate())) {
      edad--;
    }
    return edad;
  }

  function calcularTiempoConCaballo(fechaAdopcion) {
    const inicio = new Date(fechaAdopcion);
    const hoy = new Date();

    const años = hoy.getFullYear() - inicio.getFullYear();
    const meses =
      hoy.getMonth() -
      inicio.getMonth() +
      (hoy.getDate() < inicio.getDate() ? -1 : 0);

    const totalMeses = años * 12 + meses;
    const mostrarAños = Math.floor(totalMeses / 12);
    const mostrarMeses = totalMeses % 12;

    return `${mostrarAños} años y ${mostrarMeses} meses`;
  }

  return (
    <div className="p-4 max-w-6xl mx-auto">
      {/* Volver al Dashboard */}
      <button
        className="mb-4 text-blue-600 underline"
        onClick={() => navigate("/dashboard")}
      >
        ← Volver al Dashboard
      </button>

      {/* Header (puedes mover esto al layout general si es global) */}
      <div className="bg-blue-200 p-4 mb-4 rounded">
        <h1 className="text-3xl font-bold text-center">Header</h1>
        <div className="mt-2 text-center text-lg">Navbar dentro del header</div>
      </div>

      {/* Contenedor principal con dos columnas */}
      <div className="flex flex-col md:flex-row gap-6">
        {/* Columna izquierda */}
        <div className="flex flex-col items-center gap-4 md:w-1/3">
          <img
            src={caballo.fotoUrl?.trim() || caballo1}
            alt={caballo.nombre}
            className="w-full max-w-xs object-cover rounded border"
          />
          <div className="border p-4 w-full text-center rounded shadow">
            <p>
              <strong>Nombre:</strong> {caballo.nombre}
            </p>
            <p>
              <strong>Fecha de nacimiento:</strong>{" "}
              {new Date(caballo.fechaNacimiento).toLocaleDateString()}
            </p>
            <p>
              <strong>Edad:</strong> {calcularEdad(caballo.fechaNacimiento)}{" "}
              años
            </p>
            <p>
              <strong>Raza:</strong> {caballo.raza}
            </p>
            <p>
              <strong>Color:</strong> {caballo.color}
            </p>
            <p>
              <strong>Fecha de adopción:</strong>{" "}
              {new Date(caballo.fechaAdopcion).toLocaleDateString()}
            </p>
          </div>
        </div>

        {/* Columna derecha */}
        <div className="flex flex-col gap-4 md:w-2/3">
          <div className="border p-4 rounded shadow">
            <h2 className="text-xl font-semibold mb-2">
              Tiempo que llevas con el caballo
            </h2>
            <p>{calcularTiempoConCaballo(caballo.fechaAdopcion)}</p>
          </div>

          <div className="border p-4 rounded shadow">
            <h2 className="text-xl font-semibold mb-2">Herrador</h2>
            <p>
              <strong>Última herrada:</strong> {/* Ejemplo */}12/05/2025
            </p>
            <p>
              <strong>Próxima herrada:</strong> {/* Ejemplo */}12/06/2025
            </p>
          </div>

          <div className="border p-4 rounded shadow">
            <h2 className="text-xl font-semibold mb-2">Veterinario</h2>
            <p>
              <strong>Último incidente:</strong> {/* Ejemplo */}Revisión de
              rutina - 01/05/2025
            </p>
          </div>

          <div className="border p-4 rounded shadow">
            <h2 className="text-xl font-semibold mb-2">Fisio</h2>
            <p>
              <strong>Última cita:</strong> {/* Ejemplo */}20/04/2025
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
