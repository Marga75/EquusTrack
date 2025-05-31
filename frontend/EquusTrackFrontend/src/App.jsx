import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./components/AuthContext";
import Login from "./pages/Login";
import Registro from "./pages/Registro";
import Dashboard from "./pages/Dashboard";
import PrivateRoute from "./components/PrivateRoute";
import CaballoDetalle from "./pages/CaballoDetalle";
import Entrenamientos from "./pages/Entrenamientos";
import EntrenamientoDetalle from "./pages/EntrenamientoDetalle";
import EntrenamientoGuiado from "./pages/EntrenamientoGuiado";
import HistorialEntrenamientos from "./pages/HistorialEntrenamientos";
import HistorialDetalle from "./pages/HistorialDetalle";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/registro" element={<Registro />} />
          <Route
            path="/dashboard"
            element={
              <PrivateRoute>
                <Dashboard />
              </PrivateRoute>
            }
          />
          <Route
            path="/caballo/:id"
            element={
              <PrivateRoute>
                <CaballoDetalle />
              </PrivateRoute>
            }
          />
          <Route path="/entrenamientos" element={<Entrenamientos />} />
          <Route
            path="/entrenamientos/:id"
            element={
              <PrivateRoute>
                <EntrenamientoDetalle />
              </PrivateRoute>
            }
          />
          <Route
            path="/entrenamientos/:id/guiado"
            element={
              <PrivateRoute>
                <EntrenamientoGuiado />
              </PrivateRoute>
            }
          />
          <Route path="/historial" element={<HistorialEntrenamientos />} />
          <Route
            path="/historial/:id"
            element={
              <PrivateRoute>
                <HistorialDetalle />
              </PrivateRoute>
            }
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
