import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../features/auth/AuthContext";
import { brightEduApi } from "../shared/api/brightEduApi";
import { ErrorPanel } from "../shared/components/ErrorPanel";
import { LoadingPanel } from "../shared/components/LoadingPanel";
import type { StudentCourseProgress } from "../shared/types/api";

function ProgressBar({ pct }: { pct: number }) {
  return (
    <div style={{ height: 6, borderRadius: 999, background: "rgba(144,70,207,0.15)", overflow: "hidden" }}>
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

export function MyCoursesPage() {
  const { user } = useAuth();
  const { t } = useTranslation();
  const [courses, setCourses] = useState<StudentCourseProgress[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user?.accessToken) { setIsLoading(false); return; }
    brightEduApi.getStudentDashboard(user.accessToken)
      .then(d => setCourses(d.courses))
      .catch(() => setError(t("common.errorLoad")))
      .finally(() => setIsLoading(false));
  }, [user?.accessToken, t]);

  if (isLoading) return <LoadingPanel text={t("common.loading")} />;
  if (error) return <ErrorPanel message={error} />;

  return (
    <section className="section">
      <div className="page-title">
        <span className="eyebrow">{t("myCourses.eyebrow")}</span>
        <h1>{t("myCourses.title")}</h1>
        <p className="muted">{t("myCourses.subtitle")}</p>
      </div>

      {courses.length === 0 ? (
        <div className="panel" style={{ textAlign: "center", padding: "3rem 2rem" }}>
          <div style={{ fontSize: "2.5rem", marginBottom: "1rem" }}>📚</div>
          <p style={{ fontWeight: 600, marginBottom: "0.5rem" }}>{t("myCourses.empty")}</p>
          <p className="muted" style={{ marginBottom: "1.5rem" }}>{t("myCourses.emptyHint")}</p>
          <Link to="/courses" className="btn-primary" style={{ textDecoration: "none" }}>
            {t("myCourses.browseCourses")}
          </Link>
        </div>
      ) : (
        <div className="grid grid-2">
          {courses.map(course => (
            <article key={course.courseId} className="course-card">
              <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", gap: "0.5rem" }}>
                <h3 style={{ margin: 0, fontSize: "1rem" }}>{course.title}</h3>
                <span
                  className="pill"
                  style={{
                    flexShrink: 0,
                    background: course.completionPercentage >= 100 ? "rgba(34,197,94,0.15)" : undefined,
                    color: course.completionPercentage >= 100 ? "#22c55e" : undefined,
                  }}
                >
                  {Math.round(course.completionPercentage)}%
                </span>
              </div>

              <ProgressBar pct={Math.round(course.completionPercentage)} />

              <p className="muted" style={{ fontSize: "0.82rem", marginTop: "0.5rem" }}>
                {course.completedLessons}/{course.totalLessons} {t("myCourses.lessons")}
                {" · "}
                {t("myCourses.enrolledAt")} {new Date(course.enrolledAt).toLocaleDateString()}
              </p>

              <div className="actions">
                <Link className="btn-secondary" to={`/courses/${course.slug}`}>
                  {course.completionPercentage >= 100 ? t("myCourses.review") : t("myCourses.continue")}
                </Link>
              </div>
            </article>
          ))}
        </div>
      )}
    </section>
  );
}
