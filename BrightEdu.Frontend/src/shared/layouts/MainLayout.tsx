import { useState, useRef, useEffect } from "react";
import { Link, Outlet } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../features/auth/AuthContext";
import { useTheme } from "../../features/theme/ThemeContext";
import { useVantaNet } from "../../features/theme/useVantaNet";

const LANGS = [
  { code: "ro", flagSrc: "https://flagcdn.com/ro.svg", label: "RO" },
  { code: "en", flagSrc: "https://flagcdn.com/us.svg", label: "EN" },
  { code: "ru", flagSrc: "https://flagcdn.com/ru.svg", label: "RU" },
];

function LangPicker() {
  const { i18n } = useTranslation();
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  const current = LANGS.find(l => l.code === i18n.language) ?? LANGS[0];

  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, []);

  return (
    <div ref={ref} style={{ position: "relative" }}>
      <button
        type="button"
        onClick={() => setOpen(o => !o)}
        style={{
          display: "flex", alignItems: "center", gap: "0.4rem",
          padding: "0.35rem 0.65rem", borderRadius: 8,
          border: "1px solid var(--line)", background: "var(--btn-secondary-bg)",
          color: "inherit", fontSize: "0.85rem", cursor: "pointer", lineHeight: 1,
        }}
        aria-haspopup="listbox"
        aria-expanded={open}
      >
        <img src={current.flagSrc} alt={current.label} style={{ width: 20, height: 14, objectFit: "cover", borderRadius: 2 }} />
        <span>{current.label}</span>
        <span style={{ fontSize: "0.6rem", opacity: 0.6 }}>▾</span>
      </button>

      {open && (
        <ul
          role="listbox"
          style={{
            position: "absolute", right: 0, top: "calc(100% + 6px)",
            margin: 0, padding: "0.25rem 0", listStyle: "none",
            background: "var(--surface)", border: "1px solid var(--line)",
            borderRadius: 8, boxShadow: "0 4px 16px rgba(0,0,0,0.12)",
            minWidth: "7rem", zIndex: 999,
          }}
        >
          {LANGS.map(l => (
            <li key={l.code} role="option" aria-selected={l.code === i18n.language}>
              <button
                type="button"
                onClick={() => { void i18n.changeLanguage(l.code); setOpen(false); }}
                style={{
                  display: "flex", alignItems: "center", gap: "0.55rem",
                  width: "100%", padding: "0.4rem 0.75rem",
                  border: "none",
                  background: l.code === i18n.language ? "rgba(144,70,207,0.1)" : "transparent",
                  color: "inherit", fontSize: "0.85rem", cursor: "pointer",
                  textAlign: "left", fontWeight: l.code === i18n.language ? 600 : 400,
                }}
              >
                <img src={l.flagSrc} alt={l.label} style={{ width: 20, height: 14, objectFit: "cover", borderRadius: 2 }} />
                <span>{l.label}</span>
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

function ThemeToggle() {
  const { theme, toggle } = useTheme();
  const dark = theme === "dark";
  return (
    <button
      type="button"
      onClick={toggle}
      aria-label={dark ? "Switch to light mode" : "Switch to dark mode"}
      style={{
        position: "relative",
        width: "3.4rem",
        height: "1.8rem",
        borderRadius: 999,
        border: "1.5px solid var(--line)",
        background: dark
          ? "linear-gradient(135deg, var(--accent-strong), #6622aa)"
          : "rgba(255,255,255,0.9)",
        cursor: "pointer",
        padding: 0,
        transition: "background 0.3s",
        flexShrink: 0,
      }}
    >
      <span style={{
        position: "absolute",
        top: "50%",
        left: dark ? "calc(100% - 1.55rem)" : "0.18rem",
        transform: "translateY(-50%)",
        width: "1.35rem",
        height: "1.35rem",
        borderRadius: "50%",
        background: dark
          ? "linear-gradient(135deg, #c084fc, #818cf8)"
          : "linear-gradient(135deg, #f76c9d, #ffb86c)",
        display: "grid",
        placeItems: "center",
        fontSize: "0.72rem",
        transition: "left 0.3s",
        boxShadow: "0 2px 6px rgba(0,0,0,0.25)",
        lineHeight: 1,
        color: "white",
        fontWeight: 700,
      }}>
        {dark ? "☾" : "☀"}
      </span>
    </button>
  );
}

function UserDropdown() {
  const { t } = useTranslation();
  const { user, logout } = useAuth();
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, []);

  if (!user) return null;

  const initials = (user.email?.[0] ?? "U").toUpperCase();

  return (
    <div ref={ref} style={{ position: "relative" }}>
      <button
        type="button"
        onClick={() => setOpen(o => !o)}
        style={{
          display: "flex", alignItems: "center", gap: "0.5rem",
          padding: "0.3rem 0.65rem 0.3rem 0.3rem",
          borderRadius: 999, border: "1px solid var(--line)",
          background: "var(--btn-secondary-bg)", color: "inherit",
          fontSize: "0.85rem", cursor: "pointer",
        }}
      >
        <span style={{
          width: "1.8rem", height: "1.8rem", borderRadius: "50%",
          background: "linear-gradient(135deg, var(--accent), var(--accent-strong))",
          color: "#fff", display: "grid", placeItems: "center",
          fontWeight: 700, fontSize: "0.78rem", flexShrink: 0,
        }}>
          {initials}
        </span>
        <span style={{ maxWidth: 130, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
          {user.email}
        </span>
        <span style={{ fontSize: "0.6rem", opacity: 0.6 }}>▾</span>
      </button>

      {open && (
        <div style={{
          position: "absolute", right: 0, top: "calc(100% + 6px)",
          background: "var(--surface)", border: "1px solid var(--line)",
          borderRadius: 10, boxShadow: "0 4px 20px rgba(0,0,0,0.15)",
          minWidth: "13rem", zIndex: 999, overflow: "hidden",
        }}>
          <div style={{ padding: "0.65rem 1rem 0.5rem", borderBottom: "1px solid var(--line)" }}>
            <div style={{ fontWeight: 700, fontSize: "0.9rem" }}>{user.email}</div>
          </div>
          {[
            { to: "/student", label: t("nav.dashboard") },
            { to: "/courses", label: t("nav.myCourses") },
          ].map(item => (
            <Link
              key={item.to}
              to={item.to}
              onClick={() => setOpen(false)}
              style={{
                display: "block", padding: "0.55rem 1rem",
                color: "inherit", textDecoration: "none", fontSize: "0.9rem",
                transition: "background 0.15s",
              }}
              onMouseEnter={e => (e.currentTarget.style.background = "rgba(144,70,207,0.08)")}
              onMouseLeave={e => (e.currentTarget.style.background = "transparent")}
            >
              {item.label}
            </Link>
          ))}
          <div style={{ borderTop: "1px solid var(--line)", marginTop: "0.25rem" }}>
            <button
              type="button"
              onClick={() => { setOpen(false); logout(); }}
              style={{
                display: "block", width: "100%", padding: "0.55rem 1rem",
                textAlign: "left", border: "none", background: "transparent",
                color: "var(--danger, #e05)", fontSize: "0.9rem", cursor: "pointer",
                transition: "background 0.15s",
              }}
              onMouseEnter={e => (e.currentTarget.style.background = "rgba(220,50,50,0.07)")}
              onMouseLeave={e => (e.currentTarget.style.background = "transparent")}
            >
              {t("nav.logout")}
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

export function MainLayout() {
  const { t } = useTranslation();
  const { user } = useAuth();
  const { theme } = useTheme();

  useVantaNet("vanta-bg", theme);

  return (
    <>
      <div id="vanta-bg" />
      <div id="vanta-overlay" />

      <header className="topbar">
        <div className="page-shell topbar-inner">
          <Link to="/" className="brand">
            <span className="brand-badge">B</span>
            <span>BrightEdu</span>
          </Link>

          <nav className="nav-links">
            <Link to="/">{t("nav.home")}</Link>
            <Link to="/courses">{t("nav.courses")}</Link>
            {user?.roles.includes("Admin") && <Link to="/admin">{t("nav.admin")}</Link>}
            {user ? (
              <UserDropdown />
            ) : (
              <>
                <Link to="/login">{t("nav.login")}</Link>
                <Link to="/register">{t("nav.register")}</Link>
              </>
            )}
            <ThemeToggle />
            <LangPicker />
          </nav>
        </div>
      </header>

      <main className="page-shell">
        <Outlet />
      </main>

      <footer className="page-shell footer">
        BrightEdu — {t("hero.eyebrow")}
      </footer>
    </>
  );
}
