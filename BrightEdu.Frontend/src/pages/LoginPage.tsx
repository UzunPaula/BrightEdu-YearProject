import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../features/auth/AuthContext";

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
    <section className="section">
      <div className="page-title">
        <span className="eyebrow">{t("login.eyebrow")}</span>
        <h1>{t("login.title")}</h1>
        <p className="muted">{t("login.subtitle")}</p>
      </div>

      <form className="panel form-panel" onSubmit={handleSubmit}>
        <label className="field">
          <span>{t("login.email")}</span>
          <input value={email} onChange={(e) => setEmail(e.target.value)} type="email" required />
        </label>

        <label className="field">
          <span>{t("login.password")}</span>
          <input value={password} onChange={(e) => setPassword(e.target.value)} type="password" required />
        </label>

        {error ? <p className="error-text">{error}</p> : null}

        <div className="actions">
          <button className="btn-primary" type="submit" disabled={isSubmitting}>
            {isSubmitting ? t("login.submitting") : t("login.submit")}
          </button>
        </div>
      </form>
    </section>
  );
}
