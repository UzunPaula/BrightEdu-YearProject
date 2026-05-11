import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useEffect, useState } from "react";
import { brightEduApi } from "../shared/api/brightEduApi";
import type { CourseCard } from "../shared/types/api";
import { LoadingPanel } from "../shared/components/LoadingPanel";
import { ErrorPanel } from "../shared/components/ErrorPanel";

export function HomePage() {
  const { t, i18n } = useTranslation();
  const [courses, setCourses] = useState<CourseCard[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    setIsLoading(true);
    const load = async () => {
      try {
        setCourses(await brightEduApi.getCourses(i18n.language));
      } catch (err) {
        setError(err instanceof Error ? err.message : t("common.loading"));
      } finally {
        setIsLoading(false);
      }
    };
    void load();
  }, [i18n.language]);

  return (
    <>
      <section className="hero">
        <div className="hero-copy">
          <span className="eyebrow">{t("hero.eyebrow")}</span>
          <h1>{t("hero.title")}</h1>
          <p>{t("hero.text")}</p>

          <div className="actions">
            <Link className="btn-primary" to="/courses">
              {t("hero.primary")}
            </Link>
            <Link className="btn-secondary" to="/admin">
              {t("hero.secondary")}
            </Link>
          </div>
        </div>

        <div className="hero-card hero-visual">
          <div className="hero-visual-content">
            <div className="floating-stat">
              <strong>{t("hero.statPatterns")}</strong>
              <div className="muted">{t("hero.statPatternsLabel")}</div>
            </div>
            <div className="floating-card">
              <strong>{t("hero.statFlow")}</strong>
              <div className="muted">{t("hero.statFlowLabel")}</div>
            </div>
            <div className="floating-card">
              <strong>{t("hero.statMultilang")}</strong>
              <div className="muted">{t("hero.statMultilangLabel")}</div>
            </div>
          </div>
        </div>
      </section>

      <section className="section">
        <div className="section-header">
          <div>
            <span className="eyebrow">{t("hero.catalogEyebrow")}</span>
            <h2>{t("hero.catalogTitle")}</h2>
          </div>
        </div>

        {isLoading ? <LoadingPanel message={t("hero.loadingCourses")} /> : null}
        {error ? <ErrorPanel message={error} /> : null}
        {!isLoading && !error ? (
          <div className="grid grid-3">
            {courses.map((course) => (
              <article key={course.id} className="course-card">
                <div className="pill-row">
                  <span className="pill">{course.level}</span>
                  <span className="pill">{course.state}</span>
                </div>
                <h3>{course.title}</h3>
                <p className="muted">{course.shortDescription ?? t("hero.descPlaceholder")}</p>
                <div className="actions">
                  <Link className="btn-secondary" to={`/courses/${course.slug}`}>
                    {t("hero.viewCourse")}
                  </Link>
                </div>
              </article>
            ))}
          </div>
        ) : null}
      </section>
    </>
  );
}
