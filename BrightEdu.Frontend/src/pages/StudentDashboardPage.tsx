import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../features/auth/AuthContext";
import { brightEduApi } from "../shared/api/brightEduApi";
import { ErrorPanel } from "../shared/components/ErrorPanel";
import { LoadingPanel } from "../shared/components/LoadingPanel";
import type { StudentDashboard } from "../shared/types/api";

export function StudentDashboardPage() {
  const { user } = useAuth();
  const { t } = useTranslation();
  const [dashboard, setDashboard] = useState<StudentDashboard | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (!user?.accessToken) { setIsLoading(false); return; }
    brightEduApi.getStudentDashboard(user.accessToken)
      .then(setDashboard)
      .catch(err => setError(err instanceof Error ? err.message : t("student.loading")))
      .finally(() => setIsLoading(false));
  }, [user?.accessToken]);

  if (!user?.accessToken) return <ErrorPanel message={t("student.notAuthenticated")} />;
  if (isLoading) return <LoadingPanel text={t("student.loading")} />;
  if (error) return <ErrorPanel message={error} />;
  if (!dashboard) return null;

  const totalLessons = dashboard.courses.reduce((s, c) => s + c.totalLessons, 0);
  const totalCompleted = dashboard.courses.reduce((s, c) => s + c.completedLessons, 0);
  const overallPct = totalLessons === 0 ? 0 : Math.round((totalCompleted / totalLessons) * 100);
  const completedCourses = dashboard.courses.filter(c => c.completionPercentage >= 100).length;
  const lastActivity = dashboard.recentLessons[0]
    ? new Date(dashboard.recentLessons[0].lastOpenedAt).toLocaleDateString()
    : "—";

  return (
    <section className="section">
      <div className="page-title">
        <span className="eyebrow">{t("student.eyebrow")}</span>
        <h1>{t("student.title")}</h1>
        <p className="muted">{t("student.subtitle")}</p>
      </div>

      {/* ─── Metrici ─────────────────────────────────────────────────── */}
      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(170px, 1fr))", gap: "1rem", marginBottom: "0.5rem" }}>
        <StatCard value={dashboard.courses.length} label={t("student.statEnrolled")} icon="📚" />
        <StatCard value={`${totalCompleted}/${totalLessons}`} label={t("student.statLessons")} icon="✅" />
        <StatCard value={`${overallPct}%`} label={t("student.statProgress")} icon="📈" accent={overallPct > 0} />
        <StatCard value={completedCourses} label={t("student.statCompleted")} icon="🎓" />
        <StatCard value={lastActivity} label={t("student.statLastActivity")} icon="🕐" small />
      </div>

      {/* ─── Progres global ──────────────────────────────────────────── */}
      {totalLessons > 0 && (
        <div className="panel" style={{ padding: "1rem 1.25rem" }}>
          <div style={{ display: "flex", justifyContent: "space-between", fontSize: "0.85rem", marginBottom: "0.5rem" }}>
            <span style={{ fontWeight: 600 }}>{t("student.overallProgress")}</span>
            <span className="muted">{totalCompleted} / {totalLessons} {t("student.lessonsLabel")}</span>
          </div>
          <ProgressBar pct={overallPct} />
        </div>
      )}

      {/* ─── Cursuri înscrise ─────────────────────────────────────────── */}
      <div className="section">
        <div className="section-header">
          <div>
            <span className="eyebrow">{t("student.myCoursesEyebrow")}</span>
            <h2>{t("student.myCoursesTitle")}</h2>
          </div>
        </div>
        {dashboard.courses.length === 0 ? (
          <div className="panel"><p className="muted">{t("student.noEnrollments")}</p></div>
        ) : (
          <div className="grid grid-2">
            {dashboard.courses.map(course => (
              <article key={course.courseId} className="course-card">
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", gap: "0.5rem" }}>
                  <h3 style={{ margin: 0, fontSize: "1rem" }}>{course.title}</h3>
                  <span className="pill" style={{ flexShrink: 0 }}>{Math.round(course.completionPercentage)}%</span>
                </div>
                <ProgressBar pct={Math.round(course.completionPercentage)} style={{ marginTop: "0.75rem" }} />
                <p className="muted" style={{ fontSize: "0.82rem", marginTop: "0.5rem" }}>
                  {course.completedLessons}/{course.totalLessons} {t("student.lessonsLabel")} · {t("student.enrolledAt")} {new Date(course.enrolledAt).toLocaleDateString()}
                </p>
                <div className="actions">
                  <Link className="btn-secondary" to={`/courses/${course.slug}`}>{t("student.continueCourse")}</Link>
                </div>
              </article>
            ))}
          </div>
        )}
      </div>

      {/* ─── Quiz stats ──────────────────────────────────────────────── */}
      {dashboard.quizStats.totalAttempts > 0 && (
        <div className="section">
          <div className="section-header">
            <div>
              <span className="eyebrow">{t("student.quizEyebrow")}</span>
              <h2>{t("student.quizTitle")}</h2>
            </div>
          </div>
          <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(150px, 1fr))", gap: "1rem" }}>
            <StatCard value={dashboard.quizStats.totalAttempts} label={t("student.quizAttempts")} icon="📝" />
            <StatCard value={dashboard.quizStats.uniqueQuizzes} label={t("student.quizUnique")} icon="🧩" />
            <StatCard
              value={`${dashboard.quizStats.totalPassed}/${dashboard.quizStats.totalAttempts}`}
              label={t("student.quizPassed")}
              icon="✅"
            />
            <StatCard
              value={`${dashboard.quizStats.averageScore}%`}
              label={t("student.quizAvg")}
              icon="📊"
              accent={dashboard.quizStats.averageScore >= 70}
            />
            <StatCard
              value={`${dashboard.quizStats.bestScore}%`}
              label={t("student.quizBest")}
              icon="🏆"
              accent={dashboard.quizStats.bestScore >= 70}
            />
          </div>
        </div>
      )}

      {/* ─── Activitate recentă ───────────────────────────────────────── */}
      <div className="section">
        <div className="section-header">
          <div>
            <span className="eyebrow">{t("student.recentEyebrow")}</span>
            <h2>{t("student.recentTitle")}</h2>
          </div>
        </div>
        {dashboard.recentLessons.length === 0 ? (
          <div className="panel"><p className="muted">{t("student.noActivity")}</p></div>
        ) : (
          <div className="history-list">
            {dashboard.recentLessons.map(lesson => (
              <article key={lesson.lessonId} className="content-block">
                <div style={{ display: "flex", alignItems: "center", gap: "0.5rem", marginBottom: "0.2rem" }}>
                  <span style={{ fontSize: "0.75rem", fontWeight: 600, opacity: 0.5, textTransform: "uppercase", letterSpacing: "0.05em" }}>
                    {lesson.courseTitle ?? "—"}
                  </span>
                  <span style={{ opacity: 0.3, fontSize: "0.75rem" }}>›</span>
                  <span className={`pill ${lesson.isCompleted ? "pill-success" : ""}`} style={{ fontSize: "0.72rem", padding: "0.1rem 0.5rem" }}>
                    {lesson.isCompleted ? t("student.completed") : t("student.inProgress")}
                  </span>
                </div>
                <strong style={{ display: "block", fontSize: "0.95rem" }}>{lesson.lessonTitle}</strong>
                <p className="muted" style={{ fontSize: "0.82rem", marginTop: "0.25rem" }}>
                  {t("student.lastOpened")}: {new Date(lesson.lastOpenedAt).toLocaleString()}
                </p>
                <div className="actions">
                  <Link className="btn-secondary" to={`/lessons/${lesson.lessonId}`}>{t("student.openLesson")}</Link>
                </div>
              </article>
            ))}
          </div>
        )}
      </div>
    </section>
  );
}

function StatCard({ value, label, icon, accent, small }: {
  value: string | number;
  label: string;
  icon: string;
  accent?: boolean;
  small?: boolean;
}) {
  return (
    <div className="panel" style={{ padding: "1rem 1.25rem", display: "flex", flexDirection: "column", gap: "0.35rem" }}>
      <span style={{ fontSize: "1.25rem", lineHeight: 1 }}>{icon}</span>
      <span style={{
        fontSize: small ? "1rem" : "1.5rem",
        fontWeight: 800,
        color: accent ? "var(--accent)" : "inherit",
        lineHeight: 1.1,
      }}>
        {value}
      </span>
      <span className="muted" style={{ fontSize: "0.78rem" }}>{label}</span>
    </div>
  );
}

function ProgressBar({ pct, style }: { pct: number; style?: React.CSSProperties }) {
  return (
    <div style={{
      height: 6, borderRadius: 999, background: "rgba(144,70,207,0.15)",
      overflow: "hidden", ...style,
    }}>
      <div style={{
        height: "100%", borderRadius: 999,
        width: `${Math.min(pct, 100)}%`,
        background: pct >= 100
          ? "#22c55e"
          : "linear-gradient(90deg, var(--accent), var(--accent-strong))",
        transition: "width 0.4s ease",
      }} />
    </div>
  );
}
