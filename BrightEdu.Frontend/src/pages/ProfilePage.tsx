import { useEffect, useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { useAuth } from "../features/auth/AuthContext";
import { brightEduApi } from "../shared/api/brightEduApi";
import i18n from "../shared/i18n";
import { ErrorPanel } from "../shared/components/ErrorPanel";
import { LoadingPanel } from "../shared/components/LoadingPanel";
import type { UserProfile, UpdateProfilePayload } from "../shared/types/api";

export function ProfilePage() {
  const { user } = useAuth();
  const { t } = useTranslation();

  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const [form, setForm] = useState({ firstName: "", lastName: "", preferredLanguage: "1" });
  const [isSaving, setIsSaving] = useState(false);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [savedMsg, setSavedMsg] = useState<string | null>(null);

  useEffect(() => {
    if (!user?.accessToken) { setIsLoading(false); return; }
    brightEduApi.getProfile(user.accessToken)
      .then(p => {
        setProfile(p);
        setForm({ firstName: p.firstName, lastName: p.lastName, preferredLanguage: String(p.preferredLanguage) });
      })
      .catch(() => setLoadError(t("profile.loadError")))
      .finally(() => setIsLoading(false));
  }, [user?.accessToken, t]);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!user?.accessToken) return;
    setIsSaving(true);
    setSaveError(null);
    setSavedMsg(null);
    try {
      const payload: UpdateProfilePayload = {
        firstName: form.firstName,
        lastName: form.lastName,
        preferredLanguage: Number(form.preferredLanguage),
      };
      const updated = await brightEduApi.updateProfile(payload, user.accessToken);
      setProfile(updated);
      const langMap: Record<number, string> = { 1: "ro", 2: "en", 3: "ru" };
      void i18n.changeLanguage(langMap[payload.preferredLanguage] ?? "ro");
      setSavedMsg(t("profile.saved"));
    } catch {
      setSaveError(t("profile.saveError"));
    } finally {
      setIsSaving(false);
    }
  };

  const set = (key: keyof typeof form) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
      setForm(f => ({ ...f, [key]: e.target.value }));

  if (isLoading) return <LoadingPanel text={t("common.loading")} />;
  if (loadError) return <ErrorPanel message={loadError} />;

  const initials = ((form.lastName[0] ?? "") + (form.firstName[0] ?? "")).toUpperCase() || "U";
  const fullName = `${form.lastName} ${form.firstName}`.trim();

  return (
    <section className="section">
      <div className="page-title">
        <span className="eyebrow">{t("profile.eyebrow")}</span>
        <h1>{t("profile.title")}</h1>
        <p className="muted">{t("profile.subtitle")}</p>
      </div>

      {/* Avatar header */}
      <div className="panel" style={{ display: "flex", alignItems: "center", gap: "1.25rem", padding: "1.5rem" }}>
        <div style={{
          width: "4rem", height: "4rem", borderRadius: "50%", flexShrink: 0,
          background: "linear-gradient(135deg, var(--accent), var(--accent-strong))",
          color: "#fff", display: "grid", placeItems: "center",
          fontWeight: 800, fontSize: "1.4rem",
        }}>
          {initials}
        </div>
        <div>
          <div style={{ fontWeight: 700, fontSize: "1.1rem" }}>{fullName || profile?.email}</div>
          <div className="muted" style={{ fontSize: "0.85rem", marginTop: "0.15rem" }}>{profile?.email}</div>
        </div>
      </div>

      {/* Settings form */}
      <form onSubmit={handleSubmit}>
        <div className="section">
          <div className="section-header">
            <div>
              <span className="eyebrow">{t("profile.sectionPersonal")}</span>
            </div>
          </div>

          <div className="panel" style={{ padding: 0, overflow: "hidden" }}>
            <SettingsRow label={t("profile.email")}>
              <span className="muted" style={{ fontSize: "0.95rem" }}>{profile?.email}</span>
            </SettingsRow>

            <SettingsRow label={t("profile.lastName")} divider>
              <input
                style={fieldStyle}
                value={form.lastName}
                onChange={set("lastName")}
                required
              />
            </SettingsRow>

            <SettingsRow label={t("profile.firstName")}>
              <input
                style={fieldStyle}
                value={form.firstName}
                onChange={set("firstName")}
                required
              />
            </SettingsRow>
          </div>
        </div>

        <div className="section">
          <div className="section-header">
            <div>
              <span className="eyebrow">{t("profile.sectionPreferences")}</span>
            </div>
          </div>

          <div className="panel" style={{ padding: 0, overflow: "hidden" }}>
            <SettingsRow label={t("profile.preferredLanguage")}>
              <select style={{ ...fieldStyle, cursor: "pointer" }} value={form.preferredLanguage} onChange={set("preferredLanguage")}>
                <option value="1">{t("profile.langRo")}</option>
                <option value="2">{t("profile.langEn")}</option>
                <option value="3">{t("profile.langRu")}</option>
              </select>
            </SettingsRow>
          </div>
        </div>

        {/* Feedback */}
        {saveError && <p style={{ color: "var(--danger, #e05)", fontSize: "0.9rem", marginBottom: "0.75rem" }}>{saveError}</p>}
        {savedMsg && <p style={{ color: "#22c55e", fontSize: "0.9rem", marginBottom: "0.75rem" }}>{savedMsg}</p>}

        <div className="actions">
          <button
            type="submit"
            className="btn-primary"
            disabled={isSaving}
            style={{ opacity: isSaving ? 0.7 : 1 }}
          >
            {isSaving ? t("profile.saving") : t("profile.save")}
          </button>
        </div>
      </form>
    </section>
  );
}

const fieldStyle: React.CSSProperties = {
  padding: "0.4rem 0.65rem",
  borderRadius: 8,
  border: "1.5px solid var(--line)",
  background: "var(--input-bg)",
  fontSize: "0.9rem",
  color: "var(--text)",
  outline: "none",
  font: "inherit",
  width: "100%",
  maxWidth: 260,
};

function SettingsRow({ label, children, divider }: { label: string; children: React.ReactNode; divider?: boolean }) {
  return (
    <div style={{
      display: "flex", alignItems: "center", justifyContent: "space-between",
      padding: "0.9rem 1.25rem",
      borderTop: divider ? "1px solid var(--line)" : undefined,
      gap: "1rem",
    }}>
      <span style={{ fontWeight: 600, fontSize: "0.9rem", flexShrink: 0 }}>{label}</span>
      <div style={{ display: "flex", justifyContent: "flex-end", flex: 1 }}>{children}</div>
    </div>
  );
}
