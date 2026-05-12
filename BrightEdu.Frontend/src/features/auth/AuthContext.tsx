import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import type { AuthResponse } from "../../shared/types/api";
import { brightEduApi } from "../../shared/api/brightEduApi";
import i18n from "../../shared/i18n";

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

const LANG_MAP: Record<number, string> = { 1: "ro", 2: "en", 3: "ru" };

function applyLanguage(preferredLanguage: number) {
  const code = LANG_MAP[preferredLanguage] ?? "ro";
  if (i18n.language !== code) {
    void i18n.changeLanguage(code);
  }
}

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthResponse | null>(null);
  const [isRestoring, setIsRestoring] = useState(true);

  useEffect(() => {
    const persisted = localStorage.getItem(STORAGE_KEY);
    if (persisted) {
      try {
        const parsed = JSON.parse(persisted) as AuthResponse;
        if (!parsed.firstName) {
          localStorage.removeItem(STORAGE_KEY);
        } else {
          setUser(parsed);
          applyLanguage(parsed.preferredLanguage ?? 1);
        }
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
      applyLanguage(authResponse.preferredLanguage ?? 1);
    } else {
      localStorage.removeItem(STORAGE_KEY);
      void i18n.changeLanguage("ro");
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
