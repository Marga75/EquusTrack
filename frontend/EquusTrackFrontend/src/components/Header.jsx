import logo from "../assets/logo.webp";

const LayoutConHeader = ({ children, links = [], handleLogout }) => {
  return (
    <>
      <header className="flex flex-col items-center justify-between px-8 py-4 bg-white shadow">
        <div className="mb-4">
          <img src={logo} alt="Logo" className="w-80 mx-auto" />
        </div>
        <nav className="w-full flex items-center justify-between gap-6 text-slate-700 font-medium">
          <div className="flex gap-6 mx-auto">
            {links.map((link, index) => (
              <a key={index} href={link.href} className="hover:text-blue-600">
                {link.label}
              </a>
            ))}
          </div>
          <button
            onClick={handleLogout}
            className="ml-4 bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded text-sm"
          >
            Cerrar sesión
          </button>
        </nav>
      </header>
      <main>{children}</main>
    </>
  );
};

export default LayoutConHeader;
