import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../features/auth/AuthContext";

export function RegisterPage() {
  const navigate = useNavigate();
  const { register } = useAuth();
  const { t } = useTranslation();
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    preferredLanguage: "1"
  });
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await register({
        ...form,
        preferredLanguage: Number(form.preferredLanguage)
      });
      navigate("/courses");
    } catch (err) {
      setError(err instanceof Error ? err.message : t("register.submit"));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <section className="section">
      <div className="page-title">
        <span className="eyebrow">{t("register.eyebrow")}</span>
        <h1>{t("register.title")}</h1>
        <p className="muted">{t("register.subtitle")}</p>
      </div>

      <form className="panel form-panel" onSubmit={handleSubmit}>
        <div className="grid grid-2">
          <label className="field">
            <span>{t("register.firstName")}</span>
            <input
              value={form.firstName}
              onChange={(e) => setForm((current) => ({ ...current, firstName: e.target.value }))}
              required
            />
          </label>

          <label className="field">
            <span>{t("register.lastName")}</span>
            <input
              value={form.lastName}
              onChange={(e) => setForm((current) => ({ ...current, lastName: e.target.value }))}
              required
            />
          </label>
        </div>

        <label className="field">
          <span>{t("register.email")}</span>
          <input
            type="email"
            value={form.email}
            onChange={(e) => setForm((current) => ({ ...current, email: e.target.value }))}
            required
          />
        </label>

        <label className="field">
          <span>{t("register.password")}</span>
          <input
            type="password"
            value={form.password}
            onChange={(e) => setForm((current) => ({ ...current, password: e.target.value }))}
            required
          />
        </label>

        <label className="field">
          <span>{t("register.preferredLanguage")}</span>
          <select
            value={form.preferredLanguage}
            onChange={(e) => setForm((current) => ({ ...current, preferredLanguage: e.target.value }))}
          >
            <option value="1">{t("register.langRo")}</option>
            <option value="2">{t("register.langEn")}</option>
            <option value="3">{t("register.langRu")}</option>
          </select>
        </label>

        {error ? <p className="error-text">{error}</p> : null}

        <div className="actions">
          <button className="btn-primary" type="submit" disabled={isSubmitting}>
            {isSubmitting ? t("register.submitting") : t("register.submit")}
          </button>
        </div>
      </form>
    </section>
  );
}
