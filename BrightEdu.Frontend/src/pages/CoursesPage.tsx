import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { brightEduApi } from "../shared/api/brightEduApi";
import { ErrorPanel } from "../shared/components/ErrorPanel";
import { LoadingPanel } from "../shared/components/LoadingPanel";
import type { CourseCard } from "../shared/types/api";

export function CoursesPage() {
  const { t, i18n } = useTranslation();
  const [courses, setCourses] = useState<CourseCard[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    setIsLoading(true);
    setError(null);
    const load = async () => {
      try {
        setCourses(await brightEduApi.getCourses(i18n.language));
      } catch (err) {
        setError(err instanceof Error ? err.message : t("courses.loading"));
      } finally {
        setIsLoading(false);
      }
    };
    void load();
  }, [i18n.language]);

  return (
    <section className="section">
      <div className="page-title">
        <span className="eyebrow">{t("courses.eyebrow")}</span>
        <h1>{t("courses.title")}</h1>
        <p className="muted">{t("courses.subtitle")}</p>
      </div>

      {isLoading ? <LoadingPanel message={t("courses.loading")} /> : null}
      {error ? <ErrorPanel message={error} /> : null}

      {!isLoading && !error ? (
        <div className="grid grid-2">
          {courses.map((course) => (
            <article key={course.id} className="course-card">
              <div className="pill-row">
                <span className="pill">{course.level}</span>
                <span className="pill">{course.state}</span>
              </div>
              <h3>{course.title}</h3>
              <p className="muted">
                {course.shortDescription ?? t("courses.descPlaceholder")}
              </p>
              <div className="actions">
                <Link className="btn-secondary" to={`/courses/${course.slug}`}>
                  {t("courses.open")}
                </Link>
              </div>
            </article>
          ))}
        </div>
      ) : null}
    </section>
  );
}
