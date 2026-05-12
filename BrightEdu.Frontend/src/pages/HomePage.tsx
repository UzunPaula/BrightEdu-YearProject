import { Link, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useEffect, useState } from "react";
import { brightEduApi } from "../shared/api/brightEduApi";
import { useAuth } from "../features/auth/AuthContext";
import type { CourseCard } from "../shared/types/api";
import { LoadingPanel } from "../shared/components/LoadingPanel";
import { ErrorPanel } from "../shared/components/ErrorPanel";
import bannerRightImage from "../assets/banner-right-image.png";

export function HomePage() {
  const { t, i18n } = useTranslation();
  const { user } = useAuth();
  const navigate = useNavigate();
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

  const handleViewCourse = (slug: string) => {
    if (!user) {
      navigate("/login");
      return;
    }
    navigate(`/courses/${slug}`);
  };

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
            {user?.roles.includes("Admin") && (
              <Link className="btn-secondary" to="/admin">
                {t("hero.secondary")}
              </Link>
            )}
          </div>
        </div>

        <div className="hero-visual">
          <img src={bannerRightImage} alt="Student learning" className="hero-main-img" />
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
          <>
            <div className="grid grid-3">
              {courses.slice(0, 6).map((course) => (
                <article key={course.id} className="course-card">
                  <div className="pill-row">
                    <span className="pill">{course.level}</span>
                    <span className="pill">{course.state}</span>
                  </div>
                  <h3>{course.title}</h3>
                  <p className="muted">{course.shortDescription ?? t("hero.descPlaceholder")}</p>
                  <div className="actions">
                    <button className="btn-secondary" onClick={() => handleViewCourse(course.slug)}>
                      {t("hero.viewCourse")}
                    </button>
                  </div>
                </article>
              ))}
            </div>
            {courses.length > 6 && (
              <div style={{ textAlign: "center", marginTop: "2rem" }}>
                <Link className="btn-primary" to="/courses">{t("hero.viewAll")}</Link>
              </div>
            )}
          </>
        ) : null}
      </section>
    </>
  );
}
