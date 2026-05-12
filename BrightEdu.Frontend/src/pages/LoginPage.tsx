import { useState, type FormEvent } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../features/auth/AuthContext";
import { PasswordInput } from "../shared/components/PasswordInput";

const inputStyle: React.CSSProperties = {
  padding: "0.72rem 1rem",
  borderRadius: 12,
  border: "1.5px solid var(--line)",
  background: "var(--input-bg)",
  fontSize: "0.95rem",
  color: "var(--text)",
  outline: "none",
  width: "100%",
  font: "inherit",
  transition: "border-color 0.15s",
};

export function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const { t } = useTranslation();
  const [email, setEmail] = useState("admin@brightedu.local");
  const [password, setPassword] = useState("Admin123!");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);
    try {
      await login(email, password);
      navigate("/courses");
    } catch (err) {
      setError(err instanceof Error ? err.message : t("login.submit"));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div style={{
      minHeight: "calc(100vh - 78px)",
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      padding: "2rem",
    }}>
      <div style={{
        width: "100%",
        maxWidth: 460,
        background: "var(--card-bg)",
        backdropFilter: "blur(12px)",
        borderRadius: 28,
        border: "1px solid var(--line)",
        boxShadow: "var(--shadow)",
        padding: "2.5rem 2.5rem 2rem",
      }}>
        {/* Logo */}
        <div style={{ display: "flex", alignItems: "center", gap: "0.65rem", marginBottom: "2rem" }}>
          <span style={{
            width: "2.2rem", height: "2.2rem",
            background: "linear-gradient(135deg, var(--accent) 0%, var(--accent-strong) 100%)",
            borderRadius: "0.7rem", display: "grid", placeItems: "center",
            color: "white", fontWeight: 900, fontSize: "0.95rem",
            boxShadow: "var(--shadow)",
          }}>B</span>
          <span style={{ fontWeight: 800, fontSize: "1.05rem" }}>BrightEdu</span>
        </div>

        {/* Heading */}
        <div style={{ marginBottom: "1.75rem" }}>
          <span style={{
            display: "inline-flex", padding: "0.3rem 0.7rem", borderRadius: 999,
            background: "rgba(247,108,157,0.12)", color: "var(--accent-strong)",
            fontWeight: 700, fontSize: "0.75rem", letterSpacing: "0.05em",
            textTransform: "uppercase", marginBottom: "0.65rem",
          }}>
            {t("login.eyebrow")}
          </span>
          <h1 style={{ fontSize: "1.75rem", fontWeight: 800, margin: "0 0 0.4rem", lineHeight: 1.1 }}>
            {t("login.title")}
          </h1>
          <p style={{ color: "var(--muted)", margin: 0, fontSize: "0.9rem", lineHeight: 1.5 }}>
            {t("login.subtitle")}
          </p>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
          <label style={{ display: "flex", flexDirection: "column", gap: "0.4rem" }}>
            <span style={{ fontSize: "0.83rem", fontWeight: 600, color: "var(--muted)" }}>
              {t("login.email")}
            </span>
            <input
              type="email"
              value={email}
              onChange={e => setEmail(e.target.value)}
              required
              style={inputStyle}
              onFocus={e => (e.currentTarget.style.borderColor = "var(--accent-strong)")}
              onBlur={e => (e.currentTarget.style.borderColor = "var(--line)")}
            />
          </label>

          <label style={{ display: "flex", flexDirection: "column", gap: "0.4rem" }}>
            <span style={{ fontSize: "0.83rem", fontWeight: 600, color: "var(--muted)" }}>
              {t("login.password")}
            </span>
            <PasswordInput
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
              inputStyle={inputStyle}
              onFocus={e => (e.currentTarget.style.borderColor = "var(--accent-strong)")}
              onBlur={e => (e.currentTarget.style.borderColor = "var(--line)")}
            />
          </label>

          {error && (
            <div style={{
              padding: "0.65rem 1rem", borderRadius: 10,
              background: "rgba(220,38,38,0.08)", border: "1px solid rgba(220,38,38,0.2)",
              color: "#dc2626", fontSize: "0.88rem",
            }}>
              {error}
            </div>
          )}

          <button
            type="submit"
            disabled={isSubmitting}
            style={{
              marginTop: "0.25rem", padding: "0.8rem", borderRadius: 12, border: "none",
              background: isSubmitting
                ? "var(--line)"
                : "linear-gradient(135deg, var(--accent) 0%, var(--accent-strong) 100%)",
              color: "white", fontWeight: 700, fontSize: "0.95rem",
              cursor: isSubmitting ? "not-allowed" : "pointer",
              boxShadow: isSubmitting ? "none" : "0 4px 16px rgba(144,70,207,0.35)",
              transition: "opacity 0.15s",
            }}
          >
            {isSubmitting ? t("login.submitting") : t("login.submit")}
          </button>
        </form>

        <p style={{ textAlign: "center", marginTop: "1.5rem", color: "var(--muted)", fontSize: "0.88rem" }}>
          {t("login.noAccount")}{" "}
          <Link to="/register" style={{ color: "var(--accent-strong)", fontWeight: 700 }}>
            {t("nav.register")}
          </Link>
        </p>
      </div>
    </div>
  );
}
