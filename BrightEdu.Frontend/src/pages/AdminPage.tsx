import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { brightEduApi } from "../shared/api/brightEduApi";
import { ErrorPanel } from "../shared/components/ErrorPanel";
import { LoadingPanel } from "../shared/components/LoadingPanel";
import { useToast } from "../shared/components/Toast";
import { useConfirm } from "../shared/components/ConfirmDialog";
import { useAuth } from "../features/auth/AuthContext";
import type {
  AdminCourse,
  AdminModule,
  AdminLesson,
  AdminQuiz,
  AdminQuestion,
  AdminAnswer,
  AdminMediaAsset,
  AdminLessonFull,
  AdminAttachment,
  CreateCoursePayload,
  UpdateCoursePayload,
  CreateModulePayload,
  UpdateModulePayload,
  CreateLessonPayload,
  UpdateLessonPayload,
  CreateQuizPayload,
  UpdateQuizPayload,
  AnswerInputPayload,
  EntityTranslation
} from "../shared/types/api";

type Tab = "courses" | "modules" | "lessons" | "quiz";

type QuizTarget = { type: "lesson" | "module"; id: string; title: string };

// ─── Shared form shell ────────────────────────────────────────────────────────

function FormShell({ title, onBack, children }: { title: string; onBack: () => void; children: React.ReactNode }) {
  const { t } = useTranslation();
  return (
    <div>
      <div style={{ marginBottom: "1.5rem" }}>
        <button className="btn-secondary" style={{ fontSize: "0.85rem", marginBottom: "1rem" }} onClick={onBack}>
          {t("common.back")}
        </button>
        <h2 style={{ margin: 0 }}>{title}</h2>
      </div>
      <div className="panel form-panel">{children}</div>
    </div>
  );
}

// ─── Translations Panel ───────────────────────────────────────────────────────

const LANG_OPTIONS = [
  { code: "ro", label: "🇷🇴 Română" },
  { code: "en", label: "🇬🇧 English" },
  { code: "ru", label: "🇷🇺 Русский" },
];

type TranslationsPanelProps =
  | { entityId: string; entityType: "course"; token: string }
  | { entityId: string; entityType: "module"; token: string }
  | { entityId: string; entityType: "lesson"; token: string };

function TranslationsPanel({ entityId, entityType, token }: TranslationsPanelProps) {
  const { t } = useTranslation();
  const toast = useToast();
  const [activeLang, setActiveLang] = useState("ro");
  const [translations, setTranslations] = useState<EntityTranslation[]>([]);
  const [form, setForm] = useState<{ title: string; field2: string; field3: string }>({ title: "", field2: "", field3: "" });
  const [saving, setSaving] = useState(false);

  const loadTranslations = async () => {
    try {
      let data: EntityTranslation[] = [];
      if (entityType === "course") data = await brightEduApi.adminGetCourseTranslations(entityId, token);
      else if (entityType === "module") data = await brightEduApi.adminGetModuleTranslations(entityId, token);
      else data = await brightEduApi.adminGetLessonTranslations(entityId, token);
      setTranslations(data);
    } catch { /* skip */ }
  };

  useEffect(() => { void loadTranslations(); }, [entityId]);

  useEffect(() => {
    const rec = translations.find(x => x.lang === activeLang);
    setForm({ title: rec?.title ?? "", field2: rec?.field2 ?? "", field3: rec?.field3 ?? "" });
  }, [activeLang, translations]);

  const handleSave = async () => {
    if (!form.title.trim()) { toast.error(t("common.titleRequired")); return; }
    setSaving(true);
    try {
      if (entityType === "course")
        await brightEduApi.adminUpsertCourseTranslation(entityId, activeLang, { title: form.title, shortDescription: form.field2, fullDescription: form.field3 }, token);
      else if (entityType === "module")
        await brightEduApi.adminUpsertModuleTranslation(entityId, activeLang, { title: form.title, description: form.field2 }, token);
      else
        await brightEduApi.adminUpsertLessonTranslation(entityId, activeLang, { title: form.title, summary: form.field2 || form.title }, token);
      toast.success(t("admin.translationSaved"));
      await loadTranslations();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorSave"));
    } finally {
      setSaving(false);
    }
  };

  const field2Label = entityType === "course" ? t("admin.shortDesc") : entityType === "module" ? t("admin.moduleDesc") : t("admin.lessonSummary") + " *";
  const field3Label = entityType === "course" ? t("admin.fullDesc") : null;

  return (
    <div style={{ marginTop: "2rem", borderTop: "1px solid var(--line)", paddingTop: "1.5rem" }}>
      <h4 style={{ margin: "0 0 1rem", fontSize: "0.95rem" }}>{t("admin.translations")}</h4>
      <div style={{ display: "flex", gap: "0.25rem", marginBottom: "1.25rem", borderBottom: "1px solid var(--line)", paddingBottom: "0" }}>
        {LANG_OPTIONS.map(l => {
          const hasTranslation = translations.some(tr => tr.lang === l.code);
          return (
            <button
              key={l.code}
              className={activeLang === l.code ? "btn-primary" : "btn-secondary"}
              style={{ fontSize: "0.82rem", padding: "0.3rem 0.85rem", position: "relative" }}
              onClick={() => setActiveLang(l.code)}
            >
              {l.label}
              {hasTranslation && <span style={{ position: "absolute", top: 2, right: 4, fontSize: "0.5rem", color: "var(--accent-strong)" }}>●</span>}
            </button>
          );
        })}
      </div>
      <div style={{ display: "grid", gap: "0.85rem" }}>
        <div>
          <label className="muted" style={{ display: "block", marginBottom: "0.3rem", fontWeight: 600, fontSize: "0.85rem" }}>{t("admin.translationTitle")}</label>
          <input className="input" value={form.title} onChange={e => setForm(f => ({ ...f, title: e.target.value }))} placeholder={t("admin.translationTitlePlaceholder")} />
        </div>
        <div>
          <label className="muted" style={{ display: "block", marginBottom: "0.3rem", fontWeight: 600, fontSize: "0.85rem" }}>{field2Label}</label>
          <input className="input" value={form.field2} onChange={e => setForm(f => ({ ...f, field2: e.target.value }))} placeholder={field2Label} />
        </div>
        {field3Label && (
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.3rem", fontWeight: 600, fontSize: "0.85rem" }}>{field3Label}</label>
            <textarea className="input" rows={3} value={form.field3} onChange={e => setForm(f => ({ ...f, field3: e.target.value }))} placeholder={field3Label} />
          </div>
        )}
        <div>
          <button className="btn-primary" onClick={() => void handleSave()} disabled={saving} style={{ fontSize: "0.85rem" }}>
            {saving ? t("common.saving") : t("admin.saveTranslation", { lang: activeLang.toUpperCase() })}
          </button>
        </div>
      </div>
    </div>
  );
}

// ─── Courses Tab ──────────────────────────────────────────────────────────────

function CoursesTab({ token, onSelectCourse }: { token: string; onSelectCourse: (c: AdminCourse) => void }) {
  const { t } = useTranslation();
  const toast = useToast();
  const { confirm, dialogNode } = useConfirm();

  const [courses, setCourses] = useState<AdminCourse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [view, setView] = useState<"list" | "form">("list");
  const [editing, setEditing] = useState<AdminCourse | null>(null);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState<CreateCoursePayload>({ title: "", shortDescription: "", fullDescription: "", level: "Beginner" });

  const load = async () => {
    try {
      setLoading(true);
      setCourses(await brightEduApi.adminGetCourses(token));
    } catch (e) {
      setError(e instanceof Error ? e.message : t("common.errorLoad"));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { void load(); }, []);

  const openCreate = () => {
    setEditing(null);
    setForm({ title: "", shortDescription: "", fullDescription: "", level: "Beginner" });
    setView("form");
  };

  const openEdit = (c: AdminCourse) => {
    setEditing(c);
    setForm({ title: c.title, shortDescription: c.shortDescription ?? "", fullDescription: c.fullDescription ?? "", level: c.level });
    setView("form");
  };

  const handleSave = async () => {
    if (!form.title.trim()) return;
    setSaving(true);
    try {
      if (editing) {
        await brightEduApi.adminUpdateCourse(editing.id, form as UpdateCoursePayload, token);
        toast.success(t("admin.courseUpdated", { title: form.title }));
      } else {
        await brightEduApi.adminCreateCourse(form, token);
        toast.success(t("admin.courseCreated", { title: form.title }));
      }
      setView("list");
      await load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorSave"));
    } finally {
      setSaving(false);
    }
  };

  const handlePublish = async (id: string, title: string) => {
    try {
      await brightEduApi.adminPublishCourse(id, token);
      toast.success(t("admin.coursePublished", { title }));
      await load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorPublish"));
    }
  };

  const handleArchive = async (id: string, title: string) => {
    if (!await confirm(t("admin.archiveCourseConfirm", { title }))) return;
    try {
      await brightEduApi.adminArchiveCourse(id, token);
      toast.info(t("admin.courseArchived", { title }));
      await load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorSave"));
    }
  };

  if (view === "form") {
    return (
      <FormShell title={editing ? t("admin.editCourse", { title: editing.title }) : t("admin.newCourseTitle")} onBack={() => setView("list")}>
        <div style={{ display: "grid", gap: "1rem" }}>
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.translationTitle")}</label>
            <input className="input" value={form.title} onChange={e => setForm(f => ({ ...f, title: e.target.value }))} placeholder={t("admin.courseTitlePlaceholder")} />
          </div>
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.level")}</label>
            <select className="input" value={form.level} onChange={e => setForm(f => ({ ...f, level: e.target.value }))}>
              <option value="Beginner">Beginner</option>
              <option value="Intermediate">Intermediate</option>
              <option value="Advanced">Advanced</option>
            </select>
          </div>
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.shortDesc")}</label>
            <input className="input" value={form.shortDescription ?? ""} onChange={e => setForm(f => ({ ...f, shortDescription: e.target.value }))} placeholder={t("admin.shortDescPlaceholder")} />
          </div>
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.fullDesc")}</label>
            <textarea className="input" rows={4} value={form.fullDescription ?? ""} onChange={e => setForm(f => ({ ...f, fullDescription: e.target.value }))} placeholder={t("admin.fullDescPlaceholder")} />
          </div>
          <div style={{ display: "flex", gap: "0.75rem", marginTop: "0.5rem" }}>
            <button className="btn-primary" onClick={handleSave} disabled={saving}>{saving ? t("common.saving") : t("common.save")}</button>
            <button className="btn-secondary" onClick={() => setView("list")}>{t("common.cancel")}</button>
          </div>
          {editing && <TranslationsPanel entityId={editing.id} entityType="course" token={token} />}
        </div>
      </FormShell>
    );
  }

  if (loading) return <LoadingPanel message={t("admin.loadingCourses")} />;
  if (error) return <ErrorPanel message={error} />;

  return (
    <div>
      {dialogNode}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1.5rem" }}>
        <h2 style={{ margin: 0 }}>{t("admin.courses", { count: courses.length })}</h2>
        <button className="btn-primary" onClick={openCreate}>{t("admin.newCourse")}</button>
      </div>

      {courses.length === 0 ? (
        <p className="muted">{t("admin.noCourses")}</p>
      ) : (
        <div style={{ display: "grid", gap: "1rem" }}>
          {courses.map(c => (
            <article key={c.id} className="panel" style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", gap: "1rem" }}>
              <div>
                <div className="pill-row" style={{ marginBottom: "0.5rem" }}>
                  <span className="pill">{c.level}</span>
                  <span className={`pill ${c.state === "Published" ? "pill-green" : c.state === "Archived" ? "pill-red" : ""}`}>{c.state}</span>
                </div>
                <h3 style={{ margin: "0 0 0.25rem" }}>{c.title}</h3>
                {c.shortDescription && <p className="muted" style={{ margin: "0 0 0.5rem", fontSize: "0.875rem" }}>{c.shortDescription}</p>}
                <p className="muted" style={{ margin: 0, fontSize: "0.8rem" }}>
                  {t("admin.moduleCount", { count: c.moduleCount })} · {t("admin.lessonCountLabel", { count: c.lessonCount })} · {t("admin.createdAt", { date: new Date(c.createdAt).toLocaleDateString() })}
                </p>
              </div>
              <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem", minWidth: "max-content" }}>
                <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => onSelectCourse(c)}>{t("admin.goToModules")}</button>
                <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => openEdit(c)}>{t("common.edit")}</button>
                {c.state !== "Published" && (
                  <button className="btn-primary" style={{ fontSize: "0.8rem" }} onClick={() => handlePublish(c.id, c.title)}>{t("common.publish")}</button>
                )}
                {c.state === "Published" && (
                  <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => handleArchive(c.id, c.title)}>{t("common.archive")}</button>
                )}
              </div>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}

// ─── Modules Tab ──────────────────────────────────────────────────────────────

function ModulesTab({
  token,
  course,
  onSelectModule,
  onSelectQuiz,
  onModulesChanged
}: {
  token: string;
  course: AdminCourse;
  onSelectModule: (m: AdminModule) => void;
  onSelectQuiz: (target: QuizTarget) => void;
  onModulesChanged: () => void;
}) {
  const { t } = useTranslation();
  const toast = useToast();
  const { confirm, dialogNode } = useConfirm();

  const [modules, setModules] = useState<AdminModule[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [view, setView] = useState<"list" | "form">("list");
  const [editing, setEditing] = useState<AdminModule | null>(null);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState<{ title: string; description: string; order: number }>({ title: "", description: "", order: 1 });

  const load = async () => {
    try {
      setLoading(true);
      setModules(await brightEduApi.adminGetModules(course.id, token));
    } catch (e) {
      setError(e instanceof Error ? e.message : t("common.errorLoad"));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { void load(); }, [course.id]);

  const openCreate = () => {
    setEditing(null);
    setForm({ title: "", description: "", order: (modules.length || 0) + 1 });
    setView("form");
  };

  const openEdit = (m: AdminModule) => {
    setEditing(m);
    setForm({ title: m.title, description: m.description ?? "", order: m.order });
    setView("form");
  };

  const handleSave = async () => {
    if (!form.title.trim()) return;
    setSaving(true);
    try {
      if (editing) {
        const payload: UpdateModulePayload = { title: form.title, description: form.description || undefined, order: form.order };
        await brightEduApi.adminUpdateModule(editing.id, payload, token);
        toast.success(t("admin.moduleUpdated", { title: form.title }));
      } else {
        const payload: CreateModulePayload = { courseId: course.id, title: form.title, description: form.description || undefined, order: form.order };
        await brightEduApi.adminCreateModule(course.id, payload, token);
        toast.success(t("admin.moduleCreated", { title: form.title }));
      }
      setView("list");
      await load();
      onModulesChanged();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorSave"));
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: string, title: string) => {
    if (!await confirm(t("admin.deleteModuleConfirm", { title }))) return;
    try {
      await brightEduApi.adminDeleteModule(id, token);
      toast.info(t("admin.moduleDeleted", { title }));
      await load();
      onModulesChanged();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorDelete"));
    }
  };

  if (view === "form") {
    return (
      <FormShell title={editing ? t("admin.editModule", { title: editing.title }) : t("admin.newModuleTitle")} onBack={() => setView("list")}>
        <div style={{ display: "grid", gap: "1rem" }}>
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.translationTitle")}</label>
            <input className="input" value={form.title} onChange={e => setForm(f => ({ ...f, title: e.target.value }))} placeholder={t("admin.moduleTitlePlaceholder")} />
          </div>
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.moduleDesc")}</label>
            <textarea className="input" rows={3} value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))} placeholder={t("admin.moduleDescPlaceholder")} />
          </div>
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.order")}</label>
            <input className="input" type="number" min={1} value={form.order} onChange={e => setForm(f => ({ ...f, order: Number(e.target.value) }))} />
          </div>
          <div style={{ display: "flex", gap: "0.75rem", marginTop: "0.5rem" }}>
            <button className="btn-primary" onClick={handleSave} disabled={saving}>{saving ? t("common.saving") : t("common.save")}</button>
            <button className="btn-secondary" onClick={() => setView("list")}>{t("common.cancel")}</button>
          </div>
          {editing && <TranslationsPanel entityId={editing.id} entityType="module" token={token} />}
        </div>
      </FormShell>
    );
  }

  if (loading) return <LoadingPanel message={t("admin.loadingModules")} />;
  if (error) return <ErrorPanel message={error} />;

  return (
    <div>
      {dialogNode}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1.5rem" }}>
        <h2 style={{ margin: 0 }}>{t("admin.modulesTitle", { title: course.title })}</h2>
        <button className="btn-primary" onClick={openCreate}>{t("admin.newModule")}</button>
      </div>

      {modules.length === 0 ? (
        <p className="muted">{t("admin.noModules")}</p>
      ) : (
        <div style={{ display: "grid", gap: "0.75rem" }}>
          {modules.map(m => (
            <article key={m.id} className="panel" style={{ display: "flex", justifyContent: "space-between", alignItems: "center", gap: "1rem" }}>
              <div>
                <span className="muted" style={{ fontSize: "0.8rem" }}>#{m.order}</span>
                <h4 style={{ margin: "0.25rem 0" }}>{m.title}</h4>
                {m.description && <p className="muted" style={{ margin: 0, fontSize: "0.875rem" }}>{m.description}</p>}
                <p className="muted" style={{ margin: "0.25rem 0 0", fontSize: "0.8rem" }}>{t("admin.lessonCountLabel", { count: m.lessonCount })} · {m.state}</p>
              </div>
              <div style={{ display: "flex", gap: "0.5rem" }}>
                <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => onSelectModule(m)}>{t("admin.goToLessons")}</button>
                <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => onSelectQuiz({ type: "module", id: m.id, title: m.title })}>{t("lesson.quiz")}</button>
                <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => openEdit(m)}>{t("common.edit")}</button>
                <button className="btn-danger" style={{ fontSize: "0.8rem", minHeight: "36px", padding: "0 0.85rem" }} onClick={() => handleDelete(m.id, m.title)}>{t("common.delete")}</button>
              </div>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}

// ─── Lessons Tab ──────────────────────────────────────────────────────────────

function LessonsTab({
  token,
  course,
  module,
  onSelectQuiz
}: {
  token: string;
  course: AdminCourse;
  module: AdminModule;
  onSelectQuiz: (target: QuizTarget) => void;
}) {
  const { t } = useTranslation();
  const toast = useToast();
  const { confirm, dialogNode } = useConfirm();

  const [lessons, setLessons] = useState<AdminLesson[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [view, setView] = useState<"list" | "form" | "content">("list");
  const [editing, setEditing] = useState<AdminLesson | null>(null);
  const [contentLessonId, setContentLessonId] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState<CreateLessonPayload & { codeEditorEnabled: boolean }>({
    courseId: course.id, moduleId: module.id, order: 1, title: "", summary: "", estimatedMinutes: 15, codeEditorEnabled: false
  });

  const load = async () => {
    try {
      setLoading(true);
      const all = await brightEduApi.adminGetLessons(course.id, token);
      setLessons(all.filter(l => l.moduleId === module.id));
    } catch (e) {
      setError(e instanceof Error ? e.message : t("common.errorLoad"));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
    setView("list");
  }, [course.id, module.id]);

  const openCreate = () => {
    setEditing(null);
    setForm({ courseId: course.id, moduleId: module.id, order: (lessons.length || 0) + 1, title: "", summary: "", estimatedMinutes: 15, codeEditorEnabled: false });
    setView("form");
  };

  const openEdit = (l: AdminLesson) => {
    setEditing(l);
    setForm({ courseId: course.id, moduleId: l.moduleId, order: l.order, title: l.title, summary: l.summary, estimatedMinutes: l.estimatedMinutes, codeEditorEnabled: l.codeEditorEnabled });
    setView("form");
  };

  const handleSave = async () => {
    if (!form.title.trim()) return;
    setSaving(true);
    try {
      if (editing) {
        const payload: UpdateLessonPayload = { title: form.title, summary: form.summary, order: form.order, estimatedMinutes: form.estimatedMinutes, codeEditorEnabled: form.codeEditorEnabled };
        await brightEduApi.adminUpdateLesson(editing.id, payload, token);
        toast.success(t("admin.lessonUpdated", { title: form.title }));
      } else {
        await brightEduApi.adminCreateLesson(course.id, form, token);
        toast.success(t("admin.lessonCreated", { title: form.title }));
      }
      setView("list");
      await load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorSave"));
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: string, title: string) => {
    if (!await confirm(t("admin.deleteLessonConfirm", { title }))) return;
    try {
      await brightEduApi.adminDeleteLesson(id, token);
      toast.info(t("admin.lessonDeleted", { title }));
      await load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorDelete"));
    }
  };

  const handlePublish = async (id: string, title: string) => {
    try {
      await brightEduApi.adminPublishLesson(id, token);
      toast.success(t("admin.lessonPublished", { title }));
      await load();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorPublish"));
    }
  };

  if (view === "content" && contentLessonId) {
    return (
      <LessonContentEditor
        token={token}
        lessonId={contentLessonId}
        onDone={() => { setView("list"); setContentLessonId(null); }}
      />
    );
  }

  if (view === "form") {
    return (
      <FormShell title={editing ? t("admin.editLesson", { title: editing.title }) : t("admin.newLessonTitle")} onBack={() => setView("list")}>
        <div style={{ display: "grid", gap: "1rem" }}>
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.translationTitle")}</label>
            <input className="input" value={form.title} onChange={e => setForm(f => ({ ...f, title: e.target.value }))} placeholder={t("admin.lessonTitlePlaceholder")} />
          </div>
          <div>
            <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.lessonSummary")}</label>
            <textarea className="input" rows={3} value={form.summary} onChange={e => setForm(f => ({ ...f, summary: e.target.value }))} placeholder={t("admin.lessonSummaryPlaceholder")} />
          </div>
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
            <div>
              <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.order")}</label>
              <input className="input" type="number" min={1} value={form.order} onChange={e => setForm(f => ({ ...f, order: Number(e.target.value) }))} />
            </div>
            <div>
              <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.estimatedMinutes")}</label>
              <input className="input" type="number" min={1} value={form.estimatedMinutes} onChange={e => setForm(f => ({ ...f, estimatedMinutes: Number(e.target.value) }))} />
            </div>
          </div>
          <div style={{ display: "flex", alignItems: "center", gap: "0.6rem" }}>
            <input type="checkbox" id="codeEditor" checked={form.codeEditorEnabled} onChange={e => setForm(f => ({ ...f, codeEditorEnabled: e.target.checked }))} />
            <label htmlFor="codeEditor" className="muted">{t("admin.enableCodeEditor")}</label>
          </div>
          <div style={{ display: "flex", gap: "0.75rem", marginTop: "0.5rem" }}>
            <button className="btn-primary" onClick={handleSave} disabled={saving}>{saving ? t("common.saving") : t("common.save")}</button>
            <button className="btn-secondary" onClick={() => setView("list")}>{t("common.cancel")}</button>
          </div>
          {editing && <TranslationsPanel entityId={editing.id} entityType="lesson" token={token} />}
        </div>
      </FormShell>
    );
  }

  if (loading) return <LoadingPanel message={t("admin.loadingLessons")} />;
  if (error) return <ErrorPanel message={error} />;

  return (
    <div>
      {dialogNode}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1.5rem" }}>
        <div>
          <h2 style={{ margin: "0 0 0.2rem" }}>{t("admin.lessonsTitle", { title: module.title })}</h2>
          <p className="muted" style={{ margin: 0, fontSize: "0.85rem" }}>{course.title}</p>
        </div>
        <button className="btn-primary" onClick={openCreate}>{t("admin.newLesson")}</button>
      </div>

      {lessons.length === 0 ? (
        <p className="muted">{t("admin.noLessons")}</p>
      ) : (
        <div style={{ display: "grid", gap: "0.75rem" }}>
          {lessons.map(l => (
            <article key={l.id} className="panel" style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", gap: "1rem" }}>
              <div>
                <div className="pill-row" style={{ marginBottom: "0.4rem" }}>
                  <span className="pill">#{l.order}</span>
                  <span className={`pill ${l.state === "Published" ? "pill-green" : ""}`}>{l.state}</span>
                  {l.hasQuiz && <span className="pill">{t("lesson.quiz")}</span>}
                  {l.codeEditorEnabled && <span className="pill">Code</span>}
                </div>
                <h4 style={{ margin: "0 0 0.25rem" }}>{l.title}</h4>
                {l.summary && <p className="muted" style={{ margin: 0, fontSize: "0.875rem" }}>{l.summary}</p>}
                <p className="muted" style={{ margin: "0.25rem 0 0", fontSize: "0.8rem" }}>{l.estimatedMinutes} {t("lesson.minutes")}</p>
              </div>
              <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem", minWidth: "max-content" }}>
                <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => { setContentLessonId(l.id); setView("content"); }}>{t("admin.lessonContent")}</button>
                <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => onSelectQuiz({ type: "lesson", id: l.id, title: l.title })}>{t("lesson.quiz")}</button>
                <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => openEdit(l)}>{t("common.edit")}</button>
                {l.state !== "Published" && (
                  <button className="btn-primary" style={{ fontSize: "0.8rem" }} onClick={() => handlePublish(l.id, l.title)}>{t("common.publish")}</button>
                )}
                <button className="btn-danger" style={{ fontSize: "0.8rem", minHeight: "36px", padding: "0 0.85rem" }} onClick={() => handleDelete(l.id, l.title)}>{t("common.delete")}</button>
              </div>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}

// ─── Media Picker Modal ───────────────────────────────────────────────────────

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5164";

function MediaPicker({
  token,
  onSelect,
  onClose,
  filter
}: {
  token: string;
  onSelect: (asset: AdminMediaAsset) => void;
  onClose: () => void;
  filter?: "image" | "pdf" | "video";
}) {
  const { t } = useTranslation();
  const toast = useToast();
  const [assets, setAssets] = useState<AdminMediaAsset[]>([]);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [dragOver, setDragOver] = useState(false);

  const loadAssets = () => {
    brightEduApi.adminGetMedia(token)
      .then(all => {
        const filtered = filter
          ? all.filter(a =>
              filter === "image" ? a.mimeType.startsWith("image/") :
              filter === "pdf" ? a.mimeType === "application/pdf" :
              a.mimeType.startsWith("video/"))
          : all;
        setAssets(filtered);
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  };

  useEffect(() => { loadAssets(); }, []);

  const accept = filter === "image" ? "image/*"
    : filter === "pdf" ? "application/pdf"
    : filter === "video" ? "video/mp4,video/webm"
    : "image/*,application/pdf,video/mp4,video/webm";

  const handleFiles = async (files: FileList | null) => {
    if (!files || files.length === 0) return;
    setUploading(true);
    let uploaded = 0;
    for (const file of Array.from(files)) {
      try {
        await brightEduApi.adminUploadMedia(file, token);
        uploaded++;
      } catch (e) {
        toast.error(`${file.name}: ${e instanceof Error ? e.message : "eroare"}`);
      }
    }
    if (uploaded > 0) {
      toast.success(t("admin.filesUploaded", { count: uploaded }));
      setLoading(true);
      loadAssets();
    }
    setUploading(false);
  };

  const headerTitle = filter ? t("admin.selectFileType", { type: filter }) : t("admin.selectFile");
  const emptyMsg = filter ? t("admin.noFilesType", { type: filter }) : t("admin.noFiles");

  return (
    <div style={{
      position: "fixed", inset: 0, zIndex: 1000,
      background: "rgba(0,0,0,0.55)", display: "flex", alignItems: "center", justifyContent: "center"
    }}>
      <div style={{
        background: "var(--surface, #fff)", borderRadius: 20, padding: "1.5rem",
        width: "min(90vw, 720px)", maxHeight: "85vh", display: "flex", flexDirection: "column", gap: "1rem"
      }}>
        {/* Header */}
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <h3 style={{ margin: 0 }}>{headerTitle}</h3>
          <button className="btn-secondary" style={{ fontSize: "0.85rem" }} onClick={onClose}>{t("common.close")}</button>
        </div>

        {/* Upload zone */}
        <div
          onDragOver={e => { e.preventDefault(); setDragOver(true); }}
          onDragLeave={() => setDragOver(false)}
          onDrop={e => { e.preventDefault(); setDragOver(false); void handleFiles(e.dataTransfer.files); }}
          style={{
            border: `2px dashed ${dragOver ? "var(--accent-strong)" : "var(--line)"}`,
            borderRadius: 14, padding: "1rem 1.5rem",
            display: "flex", alignItems: "center", gap: "1rem",
            background: dragOver ? "rgba(144,70,207,0.05)" : "transparent",
            transition: "all 0.15s"
          }}
        >
          <span className="muted" style={{ flex: 1, fontSize: "0.875rem" }}>
            {uploading ? t("admin.uploading") : t("admin.dragHere")}
          </span>
          <label className="btn-primary" style={{ cursor: "pointer", display: "inline-flex", alignItems: "center", minHeight: 34, padding: "0 1rem", borderRadius: 999, fontWeight: 700, fontSize: "0.85rem", whiteSpace: "nowrap" }}>
            {t("admin.uploadFile")}
            <input type="file" multiple accept={accept} style={{ display: "none" }} onChange={e => void handleFiles(e.target.files)} disabled={uploading} />
          </label>
        </div>

        {/* Grid */}
        {loading ? (
          <p className="muted">{t("common.loading")}</p>
        ) : assets.length === 0 ? (
          <p className="muted">{emptyMsg}</p>
        ) : (
          <div style={{ overflowY: "auto", display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(140px, 1fr))", gap: "0.75rem" }}>
            {assets.map(asset => (
              <button
                key={asset.id}
                onClick={() => onSelect(asset)}
                style={{
                  background: "var(--surface-soft)", border: "2px solid var(--line)",
                  borderRadius: 12, padding: "0.5rem", cursor: "pointer", textAlign: "left",
                  display: "flex", flexDirection: "column", gap: "0.4rem"
                }}
              >
                <div style={{ height: 80, borderRadius: 8, overflow: "hidden", background: "#eee", display: "flex", alignItems: "center", justifyContent: "center" }}>
                  {asset.mimeType.startsWith("image/") ? (
                    <img src={`${API_BASE}${asset.url}`} alt={asset.fileName} style={{ width: "100%", height: "100%", objectFit: "cover" }} />
                  ) : (
                    <span style={{ fontSize: "2rem" }}>{asset.mimeType === "application/pdf" ? "📄" : "🎬"}</span>
                  )}
                </div>
                <span style={{ fontSize: "0.75rem", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }} title={asset.fileName}>
                  {asset.fileName}
                </span>
              </button>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

// ─── Lesson Content Editor ────────────────────────────────────────────────────

type LocalBlock = {
  id: string;
  blockType: string;
  order: number;
  configJson: string;
};

function buildConfigJson(blockType: string, fields: Record<string, string>): string {
  if (blockType === "Text") return JSON.stringify({ content: fields.content ?? "" });
  if (blockType === "Image") return JSON.stringify({ url: fields.url ?? "", alt: fields.alt ?? "" });
  if (blockType === "Video") return JSON.stringify({ title: fields.title ?? "", url: fields.url ?? "" });
  if (blockType === "PdfEmbed") return JSON.stringify({ title: fields.title ?? "", url: fields.url ?? "" });
  if (blockType === "CodeEditor") return JSON.stringify({ language: fields.language ?? "javascript", starterCode: fields.starterCode ?? "" });
  return "{}";
}

function parseConfigJson(configJson: string): Record<string, string> {
  try { return JSON.parse(configJson) as Record<string, string>; }
  catch { return {}; }
}

function BlockEditor({ block, onChange, onRemove, onMoveUp, onMoveDown, isFirst, isLast, token }: {
  block: LocalBlock;
  onChange: (updated: LocalBlock) => void;
  onRemove: () => void;
  onMoveUp: () => void;
  onMoveDown: () => void;
  isFirst: boolean;
  isLast: boolean;
  token: string;
}) {
  const { t } = useTranslation();
  const fields = parseConfigJson(block.configJson);
  const [pickerOpen, setPickerOpen] = useState(false);
  const API_B = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5164";

  const update = (key: string, value: string) => {
    const updated = { ...fields, [key]: value };
    onChange({ ...block, configJson: buildConfigJson(block.blockType, updated) });
  };

  const pickerFilter = block.blockType === "Image" ? "image" as const
    : block.blockType === "PdfEmbed" ? "pdf" as const
    : block.blockType === "Video" ? "video" as const
    : undefined;

  return (
    <div className="panel" style={{ padding: "0.85rem", display: "flex", flexDirection: "column", gap: "0.6rem" }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <span className="pill">{block.blockType}</span>
        <div style={{ display: "flex", gap: "0.4rem" }}>
          {!isFirst && <button className="btn-secondary" style={{ fontSize: "0.75rem", padding: "0 0.5rem", minHeight: 30 }} onClick={onMoveUp}>↑</button>}
          {!isLast && <button className="btn-secondary" style={{ fontSize: "0.75rem", padding: "0 0.5rem", minHeight: 30 }} onClick={onMoveDown}>↓</button>}
          <button className="btn-danger" style={{ fontSize: "0.75rem", padding: "0 0.6rem", minHeight: 30 }} onClick={onRemove}>✕</button>
        </div>
      </div>

      {block.blockType === "Text" && (
        <textarea className="input" rows={4} value={fields.content ?? ""} onChange={e => update("content", e.target.value)} placeholder={t("admin.translationTitlePlaceholder") + "..."} />
      )}

      {(block.blockType === "Image" || block.blockType === "Video" || block.blockType === "PdfEmbed") && (
        <>
          {block.blockType !== "Image" && (
            <input className="input" value={fields.title ?? ""} onChange={e => update("title", e.target.value)} placeholder={t("admin.translationTitle")} />
          )}
          <div style={{ display: "flex", gap: "0.5rem" }}>
            <input className="input" style={{ flex: 1 }} value={fields.url ?? ""} onChange={e => update("url", e.target.value)} placeholder="URL" />
            <button className="btn-secondary" style={{ fontSize: "0.8rem", whiteSpace: "nowrap" }} onClick={() => setPickerOpen(true)}>
              {t("admin.chooseFromMedia")}
            </button>
          </div>
          {fields.url && block.blockType === "Image" && (
            <img src={`${API_B}${fields.url}`} alt="" style={{ maxHeight: 120, borderRadius: 8, objectFit: "contain" }} />
          )}
          {block.blockType === "Image" && (
            <input className="input" value={fields.alt ?? ""} onChange={e => update("alt", e.target.value)} placeholder="Alt text" />
          )}
        </>
      )}

      {block.blockType === "CodeEditor" && (
        <>
          <select className="input" value={fields.language ?? "javascript"} onChange={e => update("language", e.target.value)}>
            <option value="javascript">JavaScript</option>
            <option value="typescript">TypeScript</option>
            <option value="csharp">C#</option>
            <option value="python">Python</option>
            <option value="html">HTML</option>
            <option value="css">CSS</option>
          </select>
          <textarea className="input" rows={4} value={fields.starterCode ?? ""} onChange={e => update("starterCode", e.target.value)} placeholder="Starter code..." style={{ fontFamily: "monospace" }} />
        </>
      )}

      {pickerOpen && (
        <MediaPicker
          token={token}
          filter={pickerFilter}
          onSelect={asset => {
            update("url", asset.url);
            setPickerOpen(false);
          }}
          onClose={() => setPickerOpen(false)}
        />
      )}
    </div>
  );
}

function LessonContentEditor({ token, lessonId, onDone }: {
  token: string;
  lessonId: string;
  onDone: () => void;
}) {
  const { t } = useTranslation();
  const toast = useToast();
  const { confirm, dialogNode } = useConfirm();
  const [lesson, setLesson] = useState<AdminLessonFull | null>(null);
  const [blocks, setBlocks] = useState<LocalBlock[]>([]);
  const [attachments, setAttachments] = useState<AdminAttachment[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [pickerForAttachment, setPickerForAttachment] = useState(false);
  const [attachDisplayName, setAttachDisplayName] = useState("");
  const [selectedAsset, setSelectedAsset] = useState<AdminMediaAsset | null>(null);
  const API_B = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5164";

  useEffect(() => {
    brightEduApi.adminGetLessonFull(lessonId, token)
      .then(data => {
        setLesson(data);
        setBlocks(data.contentBlocks.map(b => ({ ...b })));
        setAttachments(data.attachments.map(a => ({ ...a })));
      })
      .catch(e => toast.error(e instanceof Error ? e.message : t("common.errorLoad")))
      .finally(() => setLoading(false));
  }, [lessonId]);

  const addBlock = (blockType: string) => {
    const newOrder = blocks.length + 1;
    const defaults: Record<string, string> = {};
    const configJson = buildConfigJson(blockType, defaults);
    const newBlock: LocalBlock = { id: crypto.randomUUID(), blockType, order: newOrder, configJson };
    setBlocks(prev => [...prev, newBlock]);
  };

  const updateBlock = (idx: number, updated: LocalBlock) => {
    setBlocks(prev => prev.map((b, i) => i === idx ? updated : b));
  };

  const removeBlock = (idx: number) => {
    setBlocks(prev => prev.filter((_, i) => i !== idx).map((b, i) => ({ ...b, order: i + 1 })));
  };

  const moveBlock = (idx: number, direction: -1 | 1) => {
    setBlocks(prev => {
      const arr = [...prev];
      const target = idx + direction;
      if (target < 0 || target >= arr.length) return prev;
      [arr[idx], arr[target]] = [arr[target], arr[idx]];
      return arr.map((b, i) => ({ ...b, order: i + 1 }));
    });
  };

  const saveBlocks = async () => {
    setSaving(true);
    try {
      await brightEduApi.adminSetContentBlocks(lessonId, { blocks: blocks.map(b => ({ blockType: b.blockType, order: b.order, configJson: b.configJson })) }, token);
      toast.success(t("admin.contentSaved"));
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorSave"));
    } finally {
      setSaving(false);
    }
  };

  const handleAddAttachment = async () => {
    if (!selectedAsset || !attachDisplayName.trim()) return;
    try {
      const att = await brightEduApi.adminAddAttachment(lessonId, { mediaAssetId: selectedAsset.id, displayName: attachDisplayName.trim() }, token);
      setAttachments(prev => [...prev, { ...att, url: selectedAsset.url }]);
      setSelectedAsset(null);
      setAttachDisplayName("");
      toast.success(t("admin.attachmentAdded"));
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorCreate"));
    }
  };

  const handleRemoveAttachment = async (att: AdminAttachment) => {
    if (!await confirm(t("admin.deleteAttachmentConfirm", { name: att.displayName }))) return;
    try {
      await brightEduApi.adminRemoveAttachment(lessonId, att.id, token);
      setAttachments(prev => prev.filter(a => a.id !== att.id));
      toast.success(t("admin.attachmentRemoved"));
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorDelete"));
    }
  };

  if (loading) return <LoadingPanel message={t("common.loading")} />;
  if (!lesson) return <p className="muted">{t("admin.lessonNotFound")}</p>;

  return (
    <FormShell title={t("admin.contentLesson", { title: lesson.title })} onBack={onDone}>
      {dialogNode}
      <div style={{ display: "grid", gap: "1.5rem" }}>

        {/* Content Blocks */}
        <section>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "0.75rem" }}>
            <h3 style={{ margin: 0 }}>{t("admin.contentBlocks")}</h3>
            <div style={{ display: "flex", gap: "0.5rem", flexWrap: "wrap" }}>
              {(["Text", "Image", "Video", "PdfEmbed", "CodeEditor"] as const).map(bt => (
                <button key={bt} className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => addBlock(bt)}>
                  + {bt}
                </button>
              ))}
            </div>
          </div>
          {blocks.length === 0 ? (
            <p className="muted">{t("admin.noBlocks")}</p>
          ) : (
            <div style={{ display: "grid", gap: "0.75rem" }}>
              {blocks.map((b, idx) => (
                <BlockEditor
                  key={b.id}
                  block={b}
                  token={token}
                  isFirst={idx === 0}
                  isLast={idx === blocks.length - 1}
                  onChange={updated => updateBlock(idx, updated)}
                  onRemove={() => removeBlock(idx)}
                  onMoveUp={() => moveBlock(idx, -1)}
                  onMoveDown={() => moveBlock(idx, 1)}
                />
              ))}
            </div>
          )}
          <div style={{ marginTop: "1rem" }}>
            <button className="btn-primary" onClick={saveBlocks} disabled={saving}>
              {saving ? t("common.saving") : t("admin.saveContent")}
            </button>
          </div>
        </section>

        <hr style={{ border: "none", borderTop: "1px solid var(--line)" }} />

        {/* Attachments */}
        <section>
          <h3 style={{ margin: "0 0 0.75rem" }}>{t("admin.attachments")}</h3>
          {attachments.length === 0 ? (
            <p className="muted" style={{ marginBottom: "0.75rem" }}>{t("admin.noAttachments")}</p>
          ) : (
            <div style={{ display: "grid", gap: "0.5rem", marginBottom: "1rem" }}>
              {attachments.map(att => (
                <div key={att.id} className="panel" style={{ display: "flex", justifyContent: "space-between", alignItems: "center", padding: "0.6rem 0.85rem" }}>
                  <div>
                    <span style={{ fontWeight: 600, fontSize: "0.9rem" }}>{att.displayName}</span>
                    <span className="muted" style={{ fontSize: "0.8rem", marginLeft: "0.75rem" }}>
                      <a href={`${API_B}${att.url}`} target="_blank" rel="noreferrer" style={{ color: "inherit" }}>{att.url}</a>
                    </span>
                  </div>
                  <button className="btn-danger" style={{ fontSize: "0.8rem", padding: "0 0.75rem", minHeight: 30 }} onClick={() => void handleRemoveAttachment(att)}>
                    {t("common.delete")}
                  </button>
                </div>
              ))}
            </div>
          )}

          <div className="panel" style={{ padding: "0.85rem", display: "grid", gap: "0.6rem" }}>
            <p style={{ margin: 0, fontWeight: 600, fontSize: "0.9rem" }}>{t("admin.addAttachmentTitle")}</p>
            <input className="input" value={attachDisplayName} onChange={e => setAttachDisplayName(e.target.value)} placeholder={t("admin.displayNamePlaceholder")} />
            <div style={{ display: "flex", gap: "0.5rem", alignItems: "center" }}>
              <span className="muted" style={{ fontSize: "0.85rem", flex: 1 }}>
                {selectedAsset ? selectedAsset.fileName : t("admin.noFileSelected")}
              </span>
              <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => setPickerForAttachment(true)}>
                {t("admin.chooseFromMedia")}
              </button>
              <button className="btn-primary" style={{ fontSize: "0.8rem" }} onClick={() => void handleAddAttachment()} disabled={!selectedAsset || !attachDisplayName.trim()}>
                {t("common.add")}
              </button>
            </div>
          </div>
        </section>
      </div>

      {pickerForAttachment && (
        <MediaPicker
          token={token}
          onSelect={asset => {
            setSelectedAsset(asset);
            if (!attachDisplayName.trim()) setAttachDisplayName(asset.fileName);
            setPickerForAttachment(false);
          }}
          onClose={() => setPickerForAttachment(false)}
        />
      )}
    </FormShell>
  );
}

// ─── Question Form ────────────────────────────────────────────────────────────

type QuestionFormData = {
  text: string;
  type: string;
  order: number;
  points: number;
  answers: AnswerInputPayload[];
};

function QuestionForm({
  initial,
  nextOrder,
  onSave,
  onCancel,
  saving
}: {
  initial: QuestionFormData | null;
  nextOrder: number;
  onSave: (data: QuestionFormData) => void;
  onCancel: () => void;
  saving: boolean;
}) {
  const { t } = useTranslation();
  const [form, setForm] = useState<QuestionFormData>(
    initial ?? { text: "", type: "SingleChoice", order: nextOrder, points: 1, answers: [{ text: "", isCorrect: false, order: 1 }, { text: "", isCorrect: false, order: 2 }] }
  );

  const addAnswer = () => {
    if (form.answers.length >= 6) return;
    setForm(f => ({ ...f, answers: [...f.answers, { text: "", isCorrect: false, order: f.answers.length + 1 }] }));
  };

  const removeAnswer = (idx: number) => {
    if (form.answers.length <= 2) return;
    setForm(f => {
      const updated = f.answers.filter((_, i) => i !== idx).map((a, i) => ({ ...a, order: i + 1 }));
      return { ...f, answers: updated };
    });
  };

  const updateAnswer = (idx: number, field: keyof AnswerInputPayload, value: string | boolean | number) => {
    setForm(f => {
      const updated = f.answers.map((a, i) => i === idx ? { ...a, [field]: value } : a);
      return { ...f, answers: updated };
    });
  };

  return (
    <div style={{ display: "grid", gap: "1rem" }}>
      <div>
        <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.questionText")}</label>
        <textarea className="input" rows={3} value={form.text} onChange={e => setForm(f => ({ ...f, text: e.target.value }))} placeholder={t("admin.questionTextPlaceholder")} />
      </div>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: "1rem" }}>
        <div>
          <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.questionType")}</label>
          <select className="input" value={form.type} onChange={e => setForm(f => ({ ...f, type: e.target.value }))}>
            <option value="SingleChoice">SingleChoice</option>
            <option value="MultipleChoice">MultipleChoice</option>
          </select>
        </div>
        <div>
          <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.order")}</label>
          <input className="input" type="number" min={1} value={form.order} onChange={e => setForm(f => ({ ...f, order: Number(e.target.value) }))} />
        </div>
        <div>
          <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.points")}</label>
          <input className="input" type="number" min={1} value={form.points} onChange={e => setForm(f => ({ ...f, points: Number(e.target.value) }))} />
        </div>
      </div>

      <div>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "0.5rem" }}>
          <label className="muted" style={{ fontWeight: 600 }}>{t("admin.answersLabel", { count: form.answers.length })}</label>
          {form.answers.length < 6 && (
            <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={addAnswer}>{t("admin.addAnswer")}</button>
          )}
        </div>
        <div style={{ display: "grid", gap: "0.5rem" }}>
          {form.answers.map((a, idx) => (
            <div key={idx} style={{ display: "flex", gap: "0.5rem", alignItems: "center" }}>
              <input
                className="input"
                style={{ flex: 1 }}
                value={a.text}
                onChange={e => updateAnswer(idx, "text", e.target.value)}
                placeholder={t("admin.answerPlaceholder", { num: idx + 1 })}
              />
              <label style={{ display: "flex", alignItems: "center", gap: "0.3rem", whiteSpace: "nowrap", fontSize: "0.85rem" }}>
                <input type="checkbox" checked={a.isCorrect} onChange={e => updateAnswer(idx, "isCorrect", e.target.checked)} />
                {t("admin.correct")}
              </label>
              {form.answers.length > 2 && (
                <button className="btn-danger" style={{ fontSize: "0.75rem", padding: "0 0.6rem", minHeight: "34px" }} onClick={() => removeAnswer(idx)}>✕</button>
              )}
            </div>
          ))}
        </div>
      </div>

      <div style={{ display: "flex", gap: "0.75rem", marginTop: "0.5rem" }}>
        <button className="btn-primary" onClick={() => onSave(form)} disabled={saving}>{saving ? t("common.saving") : t("admin.saveQuestion")}</button>
        <button className="btn-secondary" onClick={onCancel}>{t("common.cancel")}</button>
      </div>
    </div>
  );
}

// ─── Quiz Tab ─────────────────────────────────────────────────────────────────

function QuizTab({ token, target }: { token: string; target: QuizTarget }) {
  const { t } = useTranslation();
  const toast = useToast();
  const { confirm, dialogNode } = useConfirm();

  const [quiz, setQuiz] = useState<AdminQuiz | null>(null);
  const [questions, setQuestions] = useState<AdminQuestion[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [view, setView] = useState<"list" | "form" | "question-form">("list");
  const [editingQuestion, setEditingQuestion] = useState<AdminQuestion | null>(null);
  const [saving, setSaving] = useState(false);
  const [quizForm, setQuizForm] = useState<{ title: string; passingScore: number; maxAttempts: number; shuffleQuestions: boolean; shuffleAnswers: boolean }>({
    title: "", passingScore: 60, maxAttempts: 3, shuffleQuestions: false, shuffleAnswers: false
  });

  const loadQuiz = async () => {
    try {
      setLoading(true);
      setError(null);
      let loaded: AdminQuiz | null = null;
      try {
        loaded = target.type === "lesson"
          ? await brightEduApi.adminGetLessonQuiz(target.id, token)
          : await brightEduApi.adminGetModuleQuiz(target.id, token);
      } catch {
        loaded = null;
      }
      setQuiz(loaded);
      if (loaded) {
        setQuizForm({ title: loaded.title, passingScore: loaded.passingScore, maxAttempts: loaded.maxAttempts, shuffleQuestions: loaded.shuffleQuestions, shuffleAnswers: loaded.shuffleAnswers });
        const qs = await brightEduApi.adminGetQuestions(loaded.id, token);
        setQuestions(qs);
      }
    } catch (e) {
      setError(e instanceof Error ? e.message : t("common.errorLoad"));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { void loadQuiz(); }, [target.type, target.id]);

  const handleCreateQuiz = async () => {
    if (!quizForm.title.trim()) return;
    setSaving(true);
    try {
      const payload: CreateQuizPayload = {
        title: quizForm.title,
        passingScore: quizForm.passingScore,
        maxAttempts: quizForm.maxAttempts,
        shuffleQuestions: quizForm.shuffleQuestions,
        shuffleAnswers: quizForm.shuffleAnswers
      };
      if (target.type === "lesson") {
        await brightEduApi.adminCreateLessonQuiz(target.id, payload, token);
      } else {
        await brightEduApi.adminCreateModuleQuiz(target.id, payload, token);
      }
      toast.success(t("admin.quizCreated"));
      setView("list");
      await loadQuiz();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorCreate"));
    } finally {
      setSaving(false);
    }
  };

  const handleUpdateQuiz = async () => {
    if (!quiz || !quizForm.title.trim()) return;
    setSaving(true);
    try {
      const payload: UpdateQuizPayload = {
        title: quizForm.title,
        passingScore: quizForm.passingScore,
        maxAttempts: quizForm.maxAttempts,
        shuffleQuestions: quizForm.shuffleQuestions,
        shuffleAnswers: quizForm.shuffleAnswers
      };
      await brightEduApi.adminUpdateQuiz(quiz.id, payload, token);
      toast.success(t("admin.quizUpdated"));
      setView("list");
      await loadQuiz();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorSave"));
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteQuiz = async () => {
    if (!quiz) return;
    if (!await confirm(t("admin.deleteQuizConfirm", { title: quiz.title }))) return;
    try {
      await brightEduApi.adminDeleteQuiz(quiz.id, token);
      toast.info(t("admin.quizDeleted"));
      setQuiz(null);
      setQuestions([]);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorDelete"));
    }
  };

  const handlePublishQuiz = async () => {
    if (!quiz) return;
    try {
      await brightEduApi.adminPublishQuiz(quiz.id, token);
      toast.success(t("admin.quizPublished"));
      await loadQuiz();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorPublish"));
    }
  };

  const handleSaveQuestion = async (data: QuestionFormData) => {
    if (!quiz) return;
    setSaving(true);
    try {
      if (editingQuestion) {
        await brightEduApi.adminUpdateQuestion(editingQuestion.id, {
          text: data.text,
          type: data.type,
          order: data.order,
          points: data.points,
          answers: data.answers
        }, token);
        toast.success(t("admin.questionUpdated"));
      } else {
        await brightEduApi.adminCreateQuestion(quiz.id, {
          quizId: quiz.id,
          text: data.text,
          type: data.type,
          order: data.order,
          points: data.points,
          answers: data.answers
        }, token);
        toast.success(t("admin.questionAdded"));
      }
      setView("list");
      setEditingQuestion(null);
      await loadQuiz();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorSave"));
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteQuestion = async (id: string) => {
    if (!await confirm(t("admin.deleteQuestionConfirm"))) return;
    try {
      await brightEduApi.adminDeleteQuestion(id, token);
      toast.info(t("admin.questionDeleted"));
      await loadQuiz();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : t("common.errorDelete"));
    }
  };

  if (loading) return <LoadingPanel message={t("admin.loadingQuiz")} />;
  if (error) return <ErrorPanel message={error} />;

  // Create quiz form
  if (!quiz && view !== "form") {
    const noQuizMsg = target.type === "lesson" ? t("admin.noQuizLesson") : t("admin.noQuizModule");
    return (
      <div>
        <div style={{ marginBottom: "1.5rem" }}>
          <h2 style={{ margin: "0 0 0.4rem" }}>{t("admin.tabQuiz", { title: target.title })}</h2>
          <p className="muted" style={{ margin: 0 }}>{noQuizMsg}</p>
        </div>
        <div className="panel form-panel">
          <h3 style={{ marginTop: 0 }}>{t("admin.createQuiz")}</h3>
          <QuizSettingsForm form={quizForm} onChange={setQuizForm} />
          <div style={{ display: "flex", gap: "0.75rem", marginTop: "1rem" }}>
            <button className="btn-primary" onClick={handleCreateQuiz} disabled={saving}>{saving ? t("admin.creating") : t("admin.createQuiz")}</button>
          </div>
        </div>
      </div>
    );
  }

  if (view === "form" && quiz) {
    return (
      <FormShell title={t("admin.editQuizTitle", { title: quiz.title })} onBack={() => setView("list")}>
        <QuizSettingsForm form={quizForm} onChange={setQuizForm} />
        <div style={{ display: "flex", gap: "0.75rem", marginTop: "1rem" }}>
          <button className="btn-primary" onClick={handleUpdateQuiz} disabled={saving}>{saving ? t("common.saving") : t("common.save")}</button>
          <button className="btn-secondary" onClick={() => setView("list")}>{t("common.cancel")}</button>
        </div>
      </FormShell>
    );
  }

  if (view === "question-form") {
    const initial: QuestionFormData | null = editingQuestion
      ? {
          text: editingQuestion.text,
          type: editingQuestion.type,
          order: editingQuestion.order,
          points: editingQuestion.points,
          answers: editingQuestion.answers.map(a => ({ text: a.text, isCorrect: a.isCorrect, order: a.order }))
        }
      : null;
    return (
      <FormShell
        title={editingQuestion ? t("admin.editQuestion") : t("admin.newQuestion")}
        onBack={() => { setView("list"); setEditingQuestion(null); }}
      >
        <QuestionForm
          initial={initial}
          nextOrder={questions.length + 1}
          onSave={handleSaveQuestion}
          onCancel={() => { setView("list"); setEditingQuestion(null); }}
          saving={saving}
        />
      </FormShell>
    );
  }

  // Main list view
  return (
    <div>
      {dialogNode}
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", marginBottom: "1.5rem" }}>
        <div>
          <h2 style={{ margin: "0 0 0.3rem" }}>{t("admin.tabQuiz", { title: target.title })}</h2>
          {quiz && (
            <div className="pill-row">
              <span className="pill">{quiz.title}</span>
              <span className={`pill ${quiz.state === "Published" ? "pill-green" : ""}`}>{quiz.state}</span>
              <span className="pill">{t("admin.quizQuestionCount", { count: quiz.questionCount })}</span>
              <span className="pill">{t("admin.quizPassing", { score: quiz.passingScore })}</span>
              <span className="pill">{t("admin.quizMaxAttempts", { count: quiz.maxAttempts })}</span>
            </div>
          )}
        </div>
        {quiz && (
          <div style={{ display: "flex", gap: "0.5rem", flexWrap: "wrap", justifyContent: "flex-end" }}>
            <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => setView("form")}>{t("admin.editSettings")}</button>
            {quiz.state !== "Published" && (
              <button className="btn-primary" style={{ fontSize: "0.8rem" }} onClick={handlePublishQuiz}>{t("admin.publishQuiz")}</button>
            )}
            <button className="btn-danger" style={{ fontSize: "0.8rem", minHeight: "36px", padding: "0 0.85rem" }} onClick={handleDeleteQuiz}>{t("admin.deleteQuiz")}</button>
          </div>
        )}
      </div>

      {quiz && (
        <>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1rem" }}>
            <h3 style={{ margin: 0 }}>{t("admin.questions", { count: questions.length })}</h3>
            <button className="btn-primary" onClick={() => { setEditingQuestion(null); setView("question-form"); }}>{t("admin.addQuestion")}</button>
          </div>

          {questions.length === 0 ? (
            <p className="muted">{t("admin.noQuestions")}</p>
          ) : (
            <div style={{ display: "grid", gap: "0.75rem" }}>
              {questions.map(q => (
                <article key={q.id} className="panel" style={{ gap: "1rem" }}>
                  <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                    <div style={{ flex: 1 }}>
                      <div className="pill-row" style={{ marginBottom: "0.4rem" }}>
                        <span className="pill">#{q.order}</span>
                        <span className="pill">{q.type}</span>
                        <span className="pill">{q.points} pt</span>
                      </div>
                      <p style={{ margin: "0 0 0.5rem", fontWeight: 500 }}>{q.text}</p>
                      <div style={{ display: "grid", gap: "0.2rem" }}>
                        {q.answers.map((a: AdminAnswer) => (
                          <div key={a.id} style={{ display: "flex", alignItems: "center", gap: "0.4rem", fontSize: "0.85rem" }}>
                            <span style={{ color: a.isCorrect ? "var(--success, #22c55e)" : "var(--muted)", fontWeight: a.isCorrect ? 600 : 400 }}>
                              {a.isCorrect ? "✓" : "○"}
                            </span>
                            <span className={a.isCorrect ? "" : "muted"}>{a.text}</span>
                          </div>
                        ))}
                      </div>
                    </div>
                    <div style={{ display: "flex", gap: "0.5rem", marginLeft: "1rem" }}>
                      <button className="btn-secondary" style={{ fontSize: "0.8rem" }} onClick={() => { setEditingQuestion(q); setView("question-form"); }}>{t("common.edit")}</button>
                      <button className="btn-danger" style={{ fontSize: "0.8rem", minHeight: "36px", padding: "0 0.85rem" }} onClick={() => handleDeleteQuestion(q.id)}>{t("common.delete")}</button>
                    </div>
                  </div>
                </article>
              ))}
            </div>
          )}
        </>
      )}
    </div>
  );
}

// ─── Quiz Settings Form (shared for create + edit) ────────────────────────────

function QuizSettingsForm({
  form,
  onChange
}: {
  form: { title: string; passingScore: number; maxAttempts: number; shuffleQuestions: boolean; shuffleAnswers: boolean };
  onChange: (f: { title: string; passingScore: number; maxAttempts: number; shuffleQuestions: boolean; shuffleAnswers: boolean }) => void;
}) {
  const { t } = useTranslation();
  return (
    <div style={{ display: "grid", gap: "1rem" }}>
      <div>
        <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.quizTitle")}</label>
        <input className="input" value={form.title} onChange={e => onChange({ ...form, title: e.target.value })} placeholder={t("admin.quizTitlePlaceholder")} />
      </div>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
        <div>
          <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.passingScore")}</label>
          <input className="input" type="number" min={0} max={100} value={form.passingScore} onChange={e => onChange({ ...form, passingScore: Number(e.target.value) })} />
        </div>
        <div>
          <label className="muted" style={{ display: "block", marginBottom: "0.4rem", fontWeight: 600 }}>{t("admin.maxAttempts")}</label>
          <input className="input" type="number" min={0} value={form.maxAttempts} onChange={e => onChange({ ...form, maxAttempts: Number(e.target.value) })} />
        </div>
      </div>
      <div style={{ display: "flex", gap: "1.5rem" }}>
        <label style={{ display: "flex", alignItems: "center", gap: "0.5rem", fontSize: "0.9rem" }}>
          <input type="checkbox" checked={form.shuffleQuestions} onChange={e => onChange({ ...form, shuffleQuestions: e.target.checked })} />
          {t("admin.shuffleQuestions")}
        </label>
        <label style={{ display: "flex", alignItems: "center", gap: "0.5rem", fontSize: "0.9rem" }}>
          <input type="checkbox" checked={form.shuffleAnswers} onChange={e => onChange({ ...form, shuffleAnswers: e.target.checked })} />
          {t("admin.shuffleAnswers")}
        </label>
      </div>
    </div>
  );
}



// ─── Main Admin Page ──────────────────────────────────────────────────────────

export function AdminPage() {
  const { user } = useAuth();
  const { t } = useTranslation();
  const [tab, setTab] = useState<Tab>("courses");
  const [selectedCourse, setSelectedCourse] = useState<AdminCourse | null>(null);
  const [selectedModule, setSelectedModule] = useState<AdminModule | null>(null);
  const [selectedQuizTarget, setSelectedQuizTarget] = useState<QuizTarget | null>(null);

  if (!user.roles.includes("Admin")) {
    return (
      <section className="section">
        <div className="page-title">
          <h1>{t("admin.accessDenied")}</h1>
          <p className="muted">{t("admin.adminOnly")}</p>
        </div>
      </section>
    );
  }

  const handleSelectCourse = async (c: AdminCourse) => {
    setSelectedCourse(c);
    setSelectedModule(null);
    setSelectedQuizTarget(null);
    setTab("modules");
  };

  const handleSelectModule = (m: AdminModule) => {
    setSelectedModule(m);
    setTab("lessons");
  };

  const handleSelectQuiz = (target: QuizTarget) => {
    setSelectedQuizTarget(target);
    setTab("quiz");
  };

  const tabStyle = (tb: Tab) => ({
    padding: "0.6rem 1.25rem",
    borderTop: "none",
    borderLeft: "none",
    borderRight: "none",
    borderBottom: tab === tb ? "2px solid var(--accent-strong, #9046cf)" : "2px solid transparent",
    background: "none",
    cursor: "pointer",
    fontWeight: tab === tb ? 600 : 400,
    color: tab === tb ? "var(--accent-strong, #9046cf)" : "inherit"
  } as React.CSSProperties);

  return (
    <section className="section">
      <div className="page-title">
        <span className="eyebrow">{t("admin.eyebrow")}</span>
        <h1>{t("admin.title")}</h1>
        <p className="muted">{t("admin.subtitlePrefix")}<strong>{user.email}</strong>.</p>
      </div>

      <div style={{ borderBottom: "1px solid var(--line)", marginBottom: "2rem", display: "flex", gap: "0.25rem", flexWrap: "wrap" }}>
        <button style={tabStyle("courses")} onClick={() => {
          setSelectedCourse(null);
          setSelectedModule(null);
          setSelectedQuizTarget(null);
          setTab("courses");
        }}>{t("admin.tabCourses")}</button>
        {selectedCourse && (
          <button style={tabStyle("modules")} onClick={() => {
            setSelectedModule(null);
            setSelectedQuizTarget(null);
            setTab("modules");
          }}>
            {t("admin.tabModules", { title: selectedCourse.title })}
          </button>
        )}
        {selectedModule && (
          <button style={tabStyle("lessons")} onClick={() => {
            setSelectedQuizTarget(null);
            setTab("lessons");
          }}>
            {t("admin.tabLessons", { title: selectedModule.title })}
          </button>
        )}
        {selectedQuizTarget && (
          <button style={tabStyle("quiz")} onClick={() => setTab("quiz")}>
            {t("admin.tabQuiz", { title: selectedQuizTarget.title })}
          </button>
        )}
      </div>

      {tab === "courses" && (
        <CoursesTab token={user.accessToken} onSelectCourse={handleSelectCourse} />
      )}

      {tab === "modules" && selectedCourse && (
        <ModulesTab
          token={user.accessToken}
          course={selectedCourse}
          onSelectModule={handleSelectModule}
          onSelectQuiz={handleSelectQuiz}
          onModulesChanged={() => {}}
        />
      )}

      {tab === "lessons" && selectedCourse && selectedModule && (
        <LessonsTab
          token={user.accessToken}
          course={selectedCourse}
          module={selectedModule}
          onSelectQuiz={handleSelectQuiz}
        />
      )}

      {tab === "quiz" && selectedQuizTarget && (
        <QuizTab
          token={user.accessToken}
          target={selectedQuizTarget}
        />
      )}

    </section>
  );
}
