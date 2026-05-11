import type {
  AuthResponse,
  CourseCard,
  CourseDetails,
  EnrollmentStatus,
  EnrollmentResult,
  LessonDetails,
  LessonProgress,
  QuizHistoryItem,
  QuizAttemptResult,
  StudentDashboard,
  SubmitQuizAttemptPayload,
  SubmitQuizAttemptResult,
  AdminCourse,
  AdminModule,
  AdminLesson,
  CreateCoursePayload,
  UpdateCoursePayload,
  CreateModulePayload,
  UpdateModulePayload,
  CreateLessonPayload,
  UpdateLessonPayload,
  AdminQuiz,
  AdminQuestion,
  CreateQuizPayload,
  UpdateQuizPayload,
  CreateQuestionPayload,
  UpdateQuestionPayload,
  AdminMediaAsset,
  AdminLessonFull,
  AdminAttachment,
  SetContentBlocksPayload,
  AddAttachmentPayload,
  AddAttachmentLinkPayload,
  EntityTranslation,
  UpsertCourseTranslationPayload,
  UpsertModuleTranslationPayload,
  UpsertLessonTranslationPayload
} from "../types/api";
import { apiRequest, apiUpload } from "./client";

export const brightEduApi = {
  register(payload: {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    preferredLanguage: number;
  }) {
    return apiRequest<AuthResponse>("/api/auth/register", {
      method: "POST",
      body: payload
    });
  },

  login(payload: { email: string; password: string }) {
    return apiRequest<AuthResponse>("/api/auth/login", {
      method: "POST",
      body: payload
    });
  },

  getCourses(lang = "ro") {
    return apiRequest<CourseCard[]>(`/api/public/courses?lang=${encodeURIComponent(lang)}`);
  },

  getCourseBySlug(slug: string, lang = "ro") {
    return apiRequest<CourseDetails>(`/api/public/courses/${encodeURIComponent(slug)}?lang=${encodeURIComponent(lang)}`);
  },

  getLessonById(lessonId: string, lang = "ro") {
    return apiRequest<LessonDetails>(`/api/public/lessons/${encodeURIComponent(lessonId)}?lang=${encodeURIComponent(lang)}`);
  },

  startQuizAttempt(quizId: string, token: string) {
    return apiRequest<QuizAttemptResult>(`/api/quizzes/${encodeURIComponent(quizId)}/attempts`, {
      method: "POST",
      token
    });
  },

  submitQuizAttempt(attemptId: string, payload: SubmitQuizAttemptPayload, token: string) {
    return apiRequest<SubmitQuizAttemptResult>(`/api/quizzes/attempts/${encodeURIComponent(attemptId)}/submit`, {
      method: "POST",
      token,
      body: payload
    });
  },

  getQuizHistory(quizId: string, token: string) {
    return apiRequest<QuizHistoryItem[]>(`/api/quizzes/${encodeURIComponent(quizId)}/attempts/history`, {
      token
    });
  },

  enrollInCourse(courseId: string, token: string) {
    return apiRequest<EnrollmentResult>(`/api/courses/${encodeURIComponent(courseId)}/enroll`, {
      method: "POST",
      token
    });
  },

  getEnrollmentStatus(courseId: string, token: string) {
    return apiRequest<EnrollmentStatus>(`/api/courses/${encodeURIComponent(courseId)}/enrollment-status`, {
      token
    });
  },

  unsubscribeFromCourse(courseId: string, token: string) {
    return apiRequest<EnrollmentStatus>(`/api/courses/${encodeURIComponent(courseId)}/enroll`, {
      method: "DELETE",
      token
    });
  },

  getStudentDashboard(token: string) {
    return apiRequest<StudentDashboard>("/api/me/dashboard", {
      token
    });
  },

  markLessonOpened(lessonId: string, token: string) {
    return apiRequest<LessonProgress>(`/api/me/lessons/${encodeURIComponent(lessonId)}/opened`, {
      method: "POST",
      token
    });
  },

  updateLessonProgress(lessonId: string, isCompleted: boolean, token: string) {
    return apiRequest<LessonProgress>("/api/me/lessons/progress", {
      method: "POST",
      token,
      body: {
        lessonId,
        isCompleted
      }
    });
  },

  // ─── Admin: Courses ────────────────────────────────────────────────────────

  adminImportCourse(payload: import("../types/api").CourseImportPayload, token: string) {
    return apiRequest<import("../types/api").CourseImportResult>("/api/admin/courses/import", { method: "POST", token, body: payload });
  },

  adminGetCourses(token: string) {
    return apiRequest<AdminCourse[]>("/api/admin/courses", { token });
  },

  adminGetCourse(id: string, token: string) {
    return apiRequest<AdminCourse>(`/api/admin/courses/${encodeURIComponent(id)}`, { token });
  },

  adminCreateCourse(payload: CreateCoursePayload, token: string) {
    return apiRequest<AdminCourse>("/api/admin/courses", { method: "POST", token, body: payload });
  },

  adminUpdateCourse(id: string, payload: UpdateCoursePayload, token: string) {
    return apiRequest<AdminCourse>(`/api/admin/courses/${encodeURIComponent(id)}`, { method: "PUT", token, body: payload });
  },

  adminPublishCourse(id: string, token: string) {
    return apiRequest<void>(`/api/admin/courses/${encodeURIComponent(id)}/publish`, { method: "POST", token });
  },

  adminArchiveCourse(id: string, token: string) {
    return apiRequest<void>(`/api/admin/courses/${encodeURIComponent(id)}/archive`, { method: "POST", token });
  },

  // ─── Admin: Modules ────────────────────────────────────────────────────────

  adminGetModules(courseId: string, token: string) {
    return apiRequest<AdminModule[]>(`/api/admin/courses/${encodeURIComponent(courseId)}/modules`, { token });
  },

  adminCreateModule(courseId: string, payload: CreateModulePayload, token: string) {
    return apiRequest<AdminModule>(`/api/admin/courses/${encodeURIComponent(courseId)}/modules`, { method: "POST", token, body: payload });
  },

  adminUpdateModule(id: string, payload: UpdateModulePayload, token: string) {
    return apiRequest<AdminModule>(`/api/admin/modules/${encodeURIComponent(id)}`, { method: "PUT", token, body: payload });
  },

  adminDeleteModule(id: string, token: string) {
    return apiRequest<void>(`/api/admin/modules/${encodeURIComponent(id)}`, { method: "DELETE", token });
  },

  // ─── Admin: Lessons ────────────────────────────────────────────────────────

  adminGetLessons(courseId: string, token: string) {
    return apiRequest<AdminLesson[]>(`/api/admin/courses/${encodeURIComponent(courseId)}/lessons`, { token });
  },

  adminCreateLesson(courseId: string, payload: CreateLessonPayload, token: string) {
    return apiRequest<AdminLesson>(`/api/admin/courses/${encodeURIComponent(courseId)}/lessons`, { method: "POST", token, body: payload });
  },

  adminUpdateLesson(id: string, payload: UpdateLessonPayload, token: string) {
    return apiRequest<AdminLesson>(`/api/admin/lessons/${encodeURIComponent(id)}`, { method: "PUT", token, body: payload });
  },

  adminDeleteLesson(id: string, token: string) {
    return apiRequest<void>(`/api/admin/lessons/${encodeURIComponent(id)}`, { method: "DELETE", token });
  },

  adminPublishLesson(id: string, token: string) {
    return apiRequest<void>(`/api/admin/lessons/${encodeURIComponent(id)}/publish`, { method: "POST", token });
  },

  adminGetLessonFull(id: string, token: string) {
    return apiRequest<AdminLessonFull>(`/api/admin/lessons/${encodeURIComponent(id)}/full`, { token });
  },

  adminSetContentBlocks(id: string, lang: string, payload: SetContentBlocksPayload, token: string) {
    return apiRequest<AdminLessonFull>(`/api/admin/lessons/${encodeURIComponent(id)}/content-blocks/${lang}`, { method: "PUT", token, body: payload });
  },

  adminAddAttachment(id: string, payload: AddAttachmentPayload, token: string) {
    return apiRequest<AdminAttachment>(`/api/admin/lessons/${encodeURIComponent(id)}/attachments`, { method: "POST", token, body: payload });
  },

  adminAddAttachmentLink(id: string, payload: AddAttachmentLinkPayload, token: string) {
    return apiRequest<AdminAttachment>(`/api/admin/lessons/${encodeURIComponent(id)}/attachments/link`, { method: "POST", token, body: payload });
  },

  adminRemoveAttachment(lessonId: string, attachmentId: string, token: string) {
    return apiRequest<void>(`/api/admin/lessons/${encodeURIComponent(lessonId)}/attachments/${encodeURIComponent(attachmentId)}`, { method: "DELETE", token });
  },

  // ─── Admin: Quizzes ────────────────────────────────────────────────────────

  adminGetLessonQuiz(lessonId: string, token: string) {
    return apiRequest<AdminQuiz>(`/api/admin/lessons/${encodeURIComponent(lessonId)}/quiz`, { token });
  },

  adminGetModuleQuiz(moduleId: string, token: string) {
    return apiRequest<AdminQuiz>(`/api/admin/modules/${encodeURIComponent(moduleId)}/quiz`, { token });
  },

  adminCreateLessonQuiz(lessonId: string, payload: CreateQuizPayload, token: string) {
    return apiRequest<AdminQuiz>(`/api/admin/lessons/${encodeURIComponent(lessonId)}/quiz`, { method: "POST", token, body: payload });
  },

  adminCreateModuleQuiz(moduleId: string, payload: CreateQuizPayload, token: string) {
    return apiRequest<AdminQuiz>(`/api/admin/modules/${encodeURIComponent(moduleId)}/quiz`, { method: "POST", token, body: payload });
  },

  adminUpdateQuiz(id: string, payload: UpdateQuizPayload, token: string) {
    return apiRequest<AdminQuiz>(`/api/admin/quizzes/${encodeURIComponent(id)}`, { method: "PUT", token, body: payload });
  },

  adminDeleteQuiz(id: string, token: string) {
    return apiRequest<void>(`/api/admin/quizzes/${encodeURIComponent(id)}`, { method: "DELETE", token });
  },

  adminPublishQuiz(id: string, token: string) {
    return apiRequest<void>(`/api/admin/quizzes/${encodeURIComponent(id)}/publish`, { method: "POST", token });
  },

  // ─── Admin: Questions ──────────────────────────────────────────────────────

  adminGetQuestions(quizId: string, token: string) {
    return apiRequest<AdminQuestion[]>(`/api/admin/quizzes/${encodeURIComponent(quizId)}/questions`, { token });
  },

  adminCreateQuestion(quizId: string, payload: CreateQuestionPayload, token: string) {
    return apiRequest<AdminQuestion>(`/api/admin/quizzes/${encodeURIComponent(quizId)}/questions`, { method: "POST", token, body: payload });
  },

  adminUpdateQuestion(id: string, payload: UpdateQuestionPayload, token: string) {
    return apiRequest<AdminQuestion>(`/api/admin/questions/${encodeURIComponent(id)}`, { method: "PUT", token, body: payload });
  },

  adminDeleteQuestion(id: string, token: string) {
    return apiRequest<void>(`/api/admin/questions/${encodeURIComponent(id)}`, { method: "DELETE", token });
  },

  // ─── Admin: Media ──────────────────────────────────────────────────────────

  adminGetMedia(token: string) {
    return apiRequest<AdminMediaAsset[]>("/api/admin/media", { token });
  },

  adminUploadMedia(file: File, token: string) {
    const fd = new FormData();
    fd.append("file", file);
    return apiUpload<AdminMediaAsset>("/api/admin/media/upload", fd, token);
  },

  adminDeleteMedia(id: string, token: string) {
    return apiRequest<void>(`/api/admin/media/${encodeURIComponent(id)}`, { method: "DELETE", token });
  },

  // ─── Translations ──────────────────────────────────────────────────────────
  adminGetCourseTranslations(id: string, token: string) {
    return apiRequest<EntityTranslation[]>(`/api/admin/courses/${encodeURIComponent(id)}/translations`, { token });
  },
  adminUpsertCourseTranslation(id: string, lang: string, payload: UpsertCourseTranslationPayload, token: string) {
    return apiRequest<void>(`/api/admin/courses/${encodeURIComponent(id)}/translations/${lang}`, { method: "PUT", token, body: payload });
  },

  adminGetModuleTranslations(id: string, token: string) {
    return apiRequest<EntityTranslation[]>(`/api/admin/modules/${encodeURIComponent(id)}/translations`, { token });
  },
  adminUpsertModuleTranslation(id: string, lang: string, payload: UpsertModuleTranslationPayload, token: string) {
    return apiRequest<void>(`/api/admin/modules/${encodeURIComponent(id)}/translations/${lang}`, { method: "PUT", token, body: payload });
  },

  adminGetLessonTranslations(id: string, token: string) {
    return apiRequest<EntityTranslation[]>(`/api/admin/lessons/${encodeURIComponent(id)}/translations`, { token });
  },
  adminUpsertLessonTranslation(id: string, lang: string, payload: UpsertLessonTranslationPayload, token: string) {
    return apiRequest<void>(`/api/admin/lessons/${encodeURIComponent(id)}/translations/${lang}`, { method: "PUT", token, body: payload });
  }
};
