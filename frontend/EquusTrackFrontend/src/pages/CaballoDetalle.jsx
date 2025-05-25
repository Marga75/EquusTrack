import caballo1 from "../assets/caballo1.jpg";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import LayoutConHeader from "../components/Header";
import { Plus } from "lucide-react";
import FormularioServicio from "../components/FormularioServicio";
import HistorialServicios from "../components/HistorialServicios";

export default function CaballoDetalle() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [caballo, setCaballo] = useState(null);

  const links = [
    { label: "Inicio", href: "/dashboard" },
    { label: "Entrenamientos", href: "/entrenamientos" },
    { label: "Historial Entrenamientos", href: "/historial" },
  ];

  const handleLogout = () => {
    logout();
    navigate("/", { replace: true });
  };

  const [formularioVisible, setFormularioVisible] = useState(null);

  const [recargaHistorial, setRecargaHistorial] = useState(0);

  useEffect(() => {
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

  if (!caballo) {
    return <p>Cargando detalles...</p>;
  }

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

  function recargarHistorial() {
    setRecargaHistorial((v) => v + 1);
  }

  return (
    <div>
      {/* Header */}
      <div>
        <LayoutConHeader
          links={links}
          handleLogout={handleLogout}
        ></LayoutConHeader>
      </div>
      <div className="p-4 max-w-6xl mx-auto">
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
              <div
                onClick={() => setFormularioVisible("herrador")}
                className="bg-white rounded-xl shadow flex items-center justify-center h-30 cursor-pointer hover:bg-slate-100 transition"
              >
                <Plus className="w-10 h-10 text-slate-400" />
              </div>
              <HistorialServicios
                idCaballo={caballo.id}
                tipo="herrador"
                recarga={recargaHistorial}
              />
            </div>

            <div className="border p-4 rounded shadow">
              <h2 className="text-xl font-semibold mb-2">Veterinario</h2>
              <div
                onClick={() => setFormularioVisible("veterinario")}
                className="bg-white rounded-xl shadow flex items-center justify-center h-30 cursor-pointer hover:bg-slate-100 transition"
              >
                <Plus className="w-10 h-10 text-slate-400" />
              </div>
              <HistorialServicios
                idCaballo={caballo.id}
                tipo="veterinario"
                recarga={recargaHistorial}
              />
            </div>

            <div className="border p-4 rounded shadow">
              <h2 className="text-xl font-semibold mb-2">Fisio</h2>
              <div
                onClick={() => setFormularioVisible("fisio")}
                className="bg-white rounded-xl shadow flex items-center justify-center h-30 cursor-pointer hover:bg-slate-100 transition"
              >
                <Plus className="w-10 h-10 text-slate-400" />
              </div>
              <HistorialServicios
                idCaballo={caballo.id}
                tipo="fisio"
                recarga={recargaHistorial}
              />
            </div>
          </div>
        </div>
      </div>

      {/* Mostrar formulario modal solo si formularioVisible no es null */}
      {formularioVisible && (
        <FormularioServicio
          tipo={formularioVisible}
          idCaballo={caballo.id}
          onClose={() => setFormularioVisible(null)}
          onInsert={recargarHistorial}
        />
      )}
    </div>
  );
}
