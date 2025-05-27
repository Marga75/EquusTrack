import logo from "../assets/logo.webp";
import { useState } from "react";
import { Menu, X } from "lucide-react";

const LayoutConHeader = ({ children, links = [], handleLogout }) => {
  const [menuAbierto, setMenuAbierto] = useState(false);

  return (
    <>
      <header className="flex flex-col items-center justify-between px-8 py-4 bg-white shadow">
        {/* Logo */}
        <div className="mb-4">
          <img src={logo} alt="Logo" className="w-80 mx-auto" />
        </div>

        {/* Contenedor del menú (horizontal en md+, vertical en móvil) */}
        <div className="flex justify-between items-center md:hidden px-4">
          <div></div> {/* espacio a la izquierda si quieres algo */}
          <button
            onClick={() => setMenuAbierto(!menuAbierto)}
            className="text-slate-700"
          >
            {menuAbierto ? <X size={28} /> : <Menu size={28} />}
          </button>
        </div>

        {/* Menú escritorio */}
        <nav className="hidden md:flex w-full items-center justify-between px-8 text-slate-700 font-medium">
          <div className="flex gap-6 mx-auto">
            {links.map((link, index) => (
              <a key={index} href={link.href} className="hover:text-blue-600">
                {link.label}
              </a>
            ))}
          </div>
          <button
            onClick={handleLogout}
            className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded text-sm"
          >
            Cerrar sesión
          </button>
        </nav>

        {/* Menú móvil (desplegable) */}
        {menuAbierto && (
          <div className="md:hidden flex flex-col items-center mt-4 gap-4 text-slate-700 font-medium">
            {links.map((link, index) => (
              <a
                key={index}
                href={link.href}
                className="hover:text-blue-600"
                onClick={() => setMenuAbierto(false)}
              >
                {link.label}
              </a>
            ))}
            <button
              onClick={() => {
                setMenuAbierto(false);
                handleLogout();
              }}
              className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded text-sm"
            >
              Cerrar sesión
            </button>
          </div>
        )}
      </header>
      <main>{children}</main>
    </>
  );
};

export default LayoutConHeader;
