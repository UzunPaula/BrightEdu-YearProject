import { Link, Outlet } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../../features/auth/AuthContext";

export function MainLayout() {
  const { t, i18n } = useTranslation();
  const { user, logout } = useAuth();

  const LANGS = [
    { code: "ro", label: "🇷🇴 RO" },
    { code: "en", label: "🇬🇧 EN" },
    { code: "ru", label: "🇷🇺 RU" },
  ];

  return (
    <>
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
              <>
                <Link to="/student">{t("nav.dashboard")}</Link>
                <span className="muted nav-user">{user.email}</span>
                <button type="button" className="btn-secondary" onClick={logout}>
                  {t("nav.logout")}
                </button>
              </>
            ) : (
              <>
                <Link to="/login">{t("nav.login")}</Link>
                <Link to="/register">{t("nav.register")}</Link>
              </>
            )}
            <select
              value={i18n.language}
              onChange={e => void i18n.changeLanguage(e.target.value)}
              style={{
                padding: "0.35rem 0.6rem",
                borderRadius: 8,
                border: "1px solid var(--line)",
                background: "var(--surface)",
                color: "inherit",
                fontSize: "0.85rem",
                cursor: "pointer",
              }}
              aria-label="Selectează limba"
            >
              {LANGS.map(l => (
                <option key={l.code} value={l.code}>{l.label}</option>
              ))}
            </select>
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
