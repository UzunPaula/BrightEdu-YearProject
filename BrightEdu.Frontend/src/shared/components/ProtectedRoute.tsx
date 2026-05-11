import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../../features/auth/AuthContext";

export function ProtectedRoute() {
  const { user, isRestoring } = useAuth();

  if (isRestoring) return null;
  if (!user) return <Navigate to="/login" replace />;

  return <Outlet />;
}
