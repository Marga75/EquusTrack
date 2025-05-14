import "./App.css";
import { useState } from "react";
import Registro from "./components/Registro";
import Login from "./components/Login";

function App() {
  const [usuario, setUsuario] = useState(null);

  return (
    
    /* <>
      <div className="min-h-screen bg-gray-100 flex items-center justify-center">
        <Registro />
      </div>
    </> */

    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      {!usuario ? (
        <Login onLoginSuccess={(data) => setUsuario(data)} />
      ) : (
        <div>
          <h1 className="text-xl font-bold">Bienvenido, {usuario.nombre}</h1>
          {/* Aquí iría la pantalla principal con caballos */}
        </div>
      )}
    </div>

  );
}

export default App;
