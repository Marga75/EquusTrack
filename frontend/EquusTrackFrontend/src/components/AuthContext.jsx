import { createContext, useContext, useEffect, useState } from "react";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [usuario, setUsuario] = useState(null);

  useEffect(() => {
    try {
      const usuarioGuardado = localStorage.getItem("usuario");
      if (usuarioGuardado && usuarioGuardado !== "undefined") {
        setUsuario(JSON.parse(usuarioGuardado));
      }
    } catch (error) {
      console.error("Error parseando usuario guardado:", error);
      setUsuario(null);
    }
  }, []);

  const login = (usuarioData) => {
    setUsuario(usuarioData);
    localStorage.setItem("usuario", JSON.stringify(usuarioData));
  };

  const logout = () => {
    setUsuario(null);
    localStorage.removeItem("usuario");
  };

  return (
    <AuthContext.Provider value={{ usuario, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

// Hook personalizado para usarlo fácil
export const useAuth = () => useContext(AuthContext);
