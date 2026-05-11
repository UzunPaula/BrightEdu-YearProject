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
    if (!user?.accessToken) {
      setIsLoading(false);
      return;
    }

    const load = async () => {
      try {
        setDashboard(await brightEduApi.getStudentDashboard(user.accessToken));
      } catch (err) {
        setError(err instanceof Error ? err.message : t("student.loading"));
      } finally {
        setIsLoading(false);
      }
    };
    void load();
  }, [user?.accessToken]);

  return (
    <section className="section">
      <div className="page-title">
        <span className="eyebrow">{t("student.eyebrow")}</span>
        <h1>{t("student.title")}</h1>
        <p className="muted">{t("student.subtitle")}</p>
      </div>

      {!user?.accessToken ? <ErrorPanel message={t("student.notAuthenticated")} /> : null}
      {isLoading ? <LoadingPanel message={t("student.loading")} /> : null}
      {error ? <ErrorPanel message={error} /> : null}

      {!isLoading && !error && dashboard ? (
        <>
          <div className="section">
            <div className="section-header">
              <div>
                <span className="eyebrow">{t("student.myCoursesEyebrow")}</span>
                <h2>{t("student.myCoursesTitle")}</h2>
              </div>
            </div>
            {dashboard.courses.length === 0 ? (
              <div className="panel">
                <p className="muted">{t("student.noEnrollments")}</p>
              </div>
            ) : (
              <div className="grid grid-2">
                {dashboard.courses.map((course) => (
                  <article key={course.courseId} className="course-card">
                    <div className="pill-row">
                      <span className="pill">{course.completionPercentage}% {t("student.completedLabel")}</span>
                      <span className="pill">
                        {course.completedLessons}/{course.totalLessons} {t("student.lessonsLabel")}
                      </span>
                    </div>
                    <h3>{course.title}</h3>
                    <p className="muted">{t("student.enrolledAt")} {new Date(course.enrolledAt).toLocaleString()}</p>
                    <div className="actions">
                      <Link className="btn-secondary" to={`/courses/${course.slug}`}>
                        {t("student.continueCourse")}
                      </Link>
                    </div>
                  </article>
                ))}
              </div>
            )}
          </div>

          <div className="section">
            <div className="section-header">
              <div>
                <span className="eyebrow">{t("student.recentEyebrow")}</span>
                <h2>{t("student.recentTitle")}</h2>
              </div>
            </div>
            {dashboard.recentLessons.length === 0 ? (
              <div className="panel">
                <p className="muted">{t("student.noActivity")}</p>
              </div>
            ) : (
              <div className="history-list">
                {dashboard.recentLessons.map((lesson) => (
                  <article key={lesson.lessonId} className="content-block">
                    {lesson.courseTitle && (
                      <span className="muted" style={{ fontSize: "0.8rem", fontWeight: 500 }}>
                        {lesson.courseTitle}
                      </span>
                    )}
                    <strong style={{ display: "block", marginTop: lesson.courseTitle ? "0.2rem" : 0 }}>
                      {lesson.courseTitle ? `${lesson.courseTitle} / ${lesson.lessonTitle}` : lesson.lessonTitle}
                    </strong>
                    <p className="muted">
                      {lesson.isCompleted ? t("student.completed") : t("student.inProgress")} |{" "}
                      {t("student.lastOpened")}: {new Date(lesson.lastOpenedAt).toLocaleString()}
                    </p>
                    <div className="actions">
                      <Link className="btn-secondary" to={`/lessons/${lesson.lessonId}`}>
                        {t("student.openLesson")}
                      </Link>
                    </div>
                  </article>
                ))}
              </div>
            )}
          </div>
        </>
      ) : null}
    </section>
  );
}
