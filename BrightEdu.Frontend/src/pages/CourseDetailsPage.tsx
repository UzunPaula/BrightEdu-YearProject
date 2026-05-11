import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../features/auth/AuthContext";
import { brightEduApi } from "../shared/api/brightEduApi";
import { ErrorPanel } from "../shared/components/ErrorPanel";
import { LoadingPanel } from "../shared/components/LoadingPanel";
import type { CourseDetails, EnrollmentStatus, LessonPreview } from "../shared/types/api";

export function CourseDetailsPage() {
  const { slug } = useParams();
  const { user } = useAuth();
  const { t, i18n } = useTranslation();
  const [course, setCourse] = useState<CourseDetails | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [enrollMessage, setEnrollMessage] = useState<string | null>(null);
  const [isEnrolling, setIsEnrolling] = useState(false);
  const [enrollmentStatus, setEnrollmentStatus] = useState<EnrollmentStatus | null>(null);

  useEffect(() => {
    if (!slug) {
      setError(t("course.missingSlug"));
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);
    const load = async () => {
      try {
        setCourse(await brightEduApi.getCourseBySlug(slug, i18n.language));
      } catch (err) {
        setError(err instanceof Error ? err.message : t("course.loadingCourse"));
      } finally {
        setIsLoading(false);
      }
    };
    void load();
  }, [slug, i18n.language]);

  useEffect(() => {
    if (!course || !user?.accessToken) {
      setEnrollmentStatus(null);
      return;
    }

    const loadStatus = async () => {
      try {
        setEnrollmentStatus(await brightEduApi.getEnrollmentStatus(course.id, user.accessToken));
      } catch {
        setEnrollmentStatus(null);
      }
    };
    void loadStatus();
  }, [course, user?.accessToken]);

  const totalLessons =
    (course?.standaloneLessons.length ?? 0) +
    (course?.modules.reduce((sum, module) => sum + module.lessons.length, 0) ?? 0);

  const handleEnroll = async () => {
    if (!course || !user?.accessToken) {
      setError(t("course.authenticateToEnroll"));
      return;
    }
    setIsEnrolling(true);
    setError(null);
    try {
      const result = await brightEduApi.enrollInCourse(course.id, user.accessToken);
      setEnrollMessage(`${t("course.enrolledAt")} ${new Date(result.enrolledAt).toLocaleString()}.`);
      setEnrollmentStatus({ courseId: course.id, isEnrolled: true, enrolledAt: result.enrolledAt, status: result.status });
    } catch (err) {
      setError(err instanceof Error ? err.message : t("course.enroll"));
    } finally {
      setIsEnrolling(false);
    }
  };

  const handleUnsubscribe = async () => {
    if (!course || !user?.accessToken) return;
    setIsEnrolling(true);
    setError(null);
    try {
      const result = await brightEduApi.unsubscribeFromCourse(course.id, user.accessToken);
      setEnrollmentStatus(result);
      setEnrollMessage(t("course.unsubscribed"));
    } catch (err) {
      setError(err instanceof Error ? err.message : t("course.unsubscribe"));
    } finally {
      setIsEnrolling(false);
    }
  };

  const lessonItem = (lesson: LessonPreview) => (
    <article key={lesson.id} className="panel">
      <div className="pill-row">
        <span className="pill">{lesson.estimatedMinutes} {t("lesson.minutes")}</span>
        {lesson.hasQuiz ? <span className="pill">{t("lesson.quiz")}</span> : null}
      </div>
      <h3>{lesson.title}</h3>
      <p className="muted">{lesson.summary}</p>
      <div className="actions">
        <Link className="btn-secondary" to={`/lessons/${lesson.id}`}>
          {t("course.openLesson")}
        </Link>
      </div>
    </article>
  );

  return (
    <section className="section">
      {isLoading ? <LoadingPanel message={t("course.loadingCourse")} /> : null}
      {error ? <ErrorPanel message={error} /> : null}
      {!isLoading && !error && course ? (
        <>
          <div className="page-title">
            <span className="eyebrow">{t("course.eyebrow")}</span>
            <h1>{course.title}</h1>
            <p className="muted">{course.shortDescription ?? t("courses.descPlaceholder")}</p>
          </div>

          <div className="split">
            <article className="panel">
              <h3>{t("course.aboutTitle")}</h3>
              <p className="muted">{course.fullDescription ?? t("course.aboutPlaceholder")}</p>
            </article>

            <aside className="panel">
              <h3>{t("course.statsTitle")}</h3>
              <div className="pill-row">
                <span className="pill">{course.modules.length} {t("course.modules")}</span>
                <span className="pill">{totalLessons} {t("course.lessons")}</span>
                <span className="pill">{course.level}</span>
              </div>
              <div className="actions">
                {enrollmentStatus?.isEnrolled ? (
                  <button className="btn-secondary" type="button" onClick={handleUnsubscribe} disabled={isEnrolling}>
                    {isEnrolling ? t("course.processing") : t("course.unsubscribe")}
                  </button>
                ) : (
                  <button className="btn-primary" type="button" onClick={handleEnroll} disabled={isEnrolling}>
                    {isEnrolling ? t("course.enrolling") : t("course.enroll")}
                  </button>
                )}
              </div>
              {enrollMessage ? <p className="success-text">{enrollMessage}</p> : null}
            </aside>
          </div>

          {course.standaloneLessons.length > 0 ? (
            <div className="section">
              <div className="section-header">
                <div>
                  <span className="eyebrow">{t("course.standaloneEyebrow")}</span>
                  <h2>{t("course.standaloneTitle")}</h2>
                </div>
              </div>
              <div className="grid">{course.standaloneLessons.map(lessonItem)}</div>
            </div>
          ) : null}

          {course.modules.map((module) => (
            <div key={module.id} className="section">
              <div className="section-header">
                <div>
                  <span className="eyebrow">{t("course.moduleEyebrow")} {module.order}</span>
                  <h2>{module.title}</h2>
                </div>
              </div>
              {module.description ? <p className="muted">{module.description}</p> : null}
              <div className="grid">{module.lessons.map(lessonItem)}</div>
            </div>
          ))}
        </>
      ) : null}
    </section>
  );
}
