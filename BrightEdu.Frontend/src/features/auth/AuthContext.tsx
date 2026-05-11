import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import type { AuthResponse } from "../../shared/types/api";
import { brightEduApi } from "../../shared/api/brightEduApi";

type AuthState = {
  user: AuthResponse | null;
  isRestoring: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (payload: RegisterPayload) => Promise<void>;
  logout: () => void;
};

type RegisterPayload = {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  preferredLanguage: number;
};

const STORAGE_KEY = "brightedu.auth";

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthResponse | null>(null);
  const [isRestoring, setIsRestoring] = useState(true);

  useEffect(() => {
    const persisted = localStorage.getItem(STORAGE_KEY);
    if (persisted) {
      try {
        setUser(JSON.parse(persisted) as AuthResponse);
      } catch {
        localStorage.removeItem(STORAGE_KEY);
      }
    }
    setIsRestoring(false);
  }, []);

  const persist = (authResponse: AuthResponse | null) => {
    setUser(authResponse);
    if (authResponse) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(authResponse));
    } else {
      localStorage.removeItem(STORAGE_KEY);
    }
  };

  const value: AuthState = {
    user,
    isRestoring,
    async login(email, password) {
      const result = await brightEduApi.login({ email, password });
      persist(result);
    },
    async register(payload) {
      const result = await brightEduApi.register(payload);
      persist(result);
    },
    logout() {
      persist(null);
    }
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used inside AuthProvider.");
  }

  return context;
}
