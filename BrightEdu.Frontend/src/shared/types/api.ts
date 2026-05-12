export type CourseCard = {
  id: string;
  slug: string;
  level: string;
  title: string;
  shortDescription: string | null;
  state: string;
  thumbnailUrl: string | null;
};

export type LessonPreview = {
  id: string;
  moduleId: string | null;
  order: number;
  title: string;
  summary: string;
  estimatedMinutes: number;
  hasQuiz: boolean;
};

export type CourseModuleDetails = {
  id: string;
  order: number;
  title: string;
  description: string | null;
  lessons: LessonPreview[];
};

export type CourseDetails = {
  id: string;
  slug: string;
  level: string;
  title: string;
  shortDescription: string | null;
  fullDescription: string | null;
  state: string;
  modules: CourseModuleDetails[];
  standaloneLessons: LessonPreview[];
};

export type LessonContentBlock = {
  id: string;
  order: number;
  blockType: string;
  configJson: string;
};

export type LessonAttachment = {
  id: string;
  displayName: string;
  mediaAssetId: string;
  url: string;
};

export type QuizAnswerOption = {
  id: string;
  text: string;
};

export type QuizQuestion = {
  id: string;
  text: string;
  questionType: string;
  order: number;
  options: QuizAnswerOption[];
};

export type LessonQuiz = {
  id: string;
  title: string;
  passingScore: number;
  maxAttempts: number;
  showMistakesAfterAttempt: boolean;
  showOnlyWrongAnswers: boolean;
  showCorrectAnswer: boolean;
  questions: QuizQuestion[];
};

export type LessonDetails = {
  id: string;
  courseId: string;
  courseSlug: string | null;
  moduleId: string | null;
  order: number;
  title: string;
  summary: string;
  estimatedMinutes: number;
  codeEditorEnabled: boolean;
  state: string;
  contentBlocks: LessonContentBlock[];
  attachments: LessonAttachment[];
  quizId: string | null;
  quiz: LessonQuiz | null;
};

export type AuthResponse = {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  accessToken: string;
  expiresAt: string;
  roles: string[];
  preferredLanguage: number; // 1=ro, 2=en, 3=ru
};

export type QuizAttemptResult = {
  attemptId: string;
  score: number | null;
  passed: boolean;
  attemptNumber: number;
  startedAt: string;
  submittedAt: string | null;
};

export type SubmitQuizAttemptPayload = {
  attemptId: string;
  answers: {
    questionId: string;
    selectedOptionId: string | null;
    textAnswer: string | null;
  }[];
};

export type SubmitQuizAttemptResult = {
  attemptId: string;
  score: number;
  passed: boolean;
  correctAnswers: number;
  totalQuestions: number;
  message: string;
};

export type AttemptQuestionResult = {
  questionId: string;
  questionText: string;
  selectedOptionId: string | null;
  selectedOptionText: string | null;
  correctOptionId: string | null;
  correctOptionText: string | null;
  isCorrect: boolean;
};

export type QuizHistoryItem = {
  attemptId: string;
  quizId: string;
  attemptNumber: number;
  score: number | null;
  passed: boolean;
  startedAt: string;
  submittedAt: string | null;
  questionResults: AttemptQuestionResult[] | null;
};

export type EnrollmentResult = {
  enrollmentId: string;
  courseId: string;
  enrolledAt: string;
  status: string;
};

export type EnrollmentStatus = {
  courseId: string;
  isEnrolled: boolean;
  enrolledAt: string | null;
  status: string | null;
};

export type LessonProgress = {
  lessonId: string;
  lessonTitle: string;
  courseSlug: string | null;
  courseTitle: string | null;
  isCompleted: boolean;
  completedAt: string | null;
  lastOpenedAt: string;
};

export type StudentCourseProgress = {
  courseId: string;
  slug: string;
  title: string;
  totalLessons: number;
  completedLessons: number;
  completionPercentage: number;
  enrolledAt: string;
};

export type QuizStats = {
  totalAttempts: number;
  totalPassed: number;
  uniqueQuizzes: number;
  averageScore: number;
  bestScore: number;
};

export type StudentDashboard = {
  courses: StudentCourseProgress[];
  recentLessons: LessonProgress[];
  quizStats: QuizStats;
};

// ─── Admin types ──────────────────────────────────────────────────────────────

export type AdminCourse = {
  id: string;
  slug: string;
  level: string;
  title: string;
  shortDescription: string | null;
  fullDescription: string | null;
  state: string;
  isPublished: boolean;
  moduleCount: number;
  lessonCount: number;
  createdAt: string;
  updatedAt: string;
};

export type AdminModule = {
  id: string;
  courseId: string;
  order: number;
  title: string;
  description: string | null;
  state: string;
  lessonCount: number;
};

export type AdminLesson = {
  id: string;
  courseId: string;
  moduleId: string | null;
  order: number;
  title: string;
  summary: string;
  estimatedMinutes: number;
  codeEditorEnabled: boolean;
  state: string;
  hasQuiz: boolean;
};

export type CreateCoursePayload = {
  title: string;
  shortDescription?: string;
  fullDescription?: string;
  level: string;
};

export type UpdateCoursePayload = {
  title: string;
  shortDescription?: string;
  fullDescription?: string;
  level: string;
};

export type CreateModulePayload = {
  courseId: string;
  order: number;
  title: string;
  description?: string;
};

export type UpdateModulePayload = {
  title: string;
  description?: string;
  order: number;
};

export type CreateLessonPayload = {
  courseId: string;
  moduleId?: string | null;
  order: number;
  title: string;
  summary: string;
  estimatedMinutes: number;
};

export type UpdateLessonPayload = {
  title: string;
  summary: string;
  order: number;
  estimatedMinutes: number;
  codeEditorEnabled: boolean;
};

export type AdminQuiz = {
  id: string;
  lessonId: string | null;
  moduleId: string | null;
  title: string;
  passingScore: number;
  maxAttempts: number;
  shuffleQuestions: boolean;
  shuffleAnswers: boolean;
  showMistakesAfterAttempt: boolean;
  showOnlyWrongAnswers: boolean;
  showCorrectAnswer: boolean;
  state: string;
  questionCount: number;
};

export type AdminAnswer = {
  id: string;
  text: string;
  isCorrect: boolean;
  order: number;
};

export type AdminQuestion = {
  id: string;
  quizId: string;
  text: string;
  type: string;
  order: number;
  points: number;
  answers: AdminAnswer[];
};

export type AnswerInputPayload = { text: string; isCorrect: boolean; order: number };

export type CreateQuizPayload = { title: string; passingScore: number; maxAttempts: number; shuffleQuestions: boolean; shuffleAnswers: boolean; showMistakesAfterAttempt: boolean; showOnlyWrongAnswers: boolean; showCorrectAnswer: boolean };
export type UpdateQuizPayload = CreateQuizPayload;

export type CreateQuestionPayload = { quizId: string; text: string; type: string; order: number; points: number; answers: AnswerInputPayload[] };
export type UpdateQuestionPayload = { text: string; type: string; order: number; points: number; answers: AnswerInputPayload[] };

export type AdminMediaAsset = {
  id: string;
  fileName: string;
  url: string;
  mimeType: string;
  sizeInBytes: number;
  createdAt: string;
};

export type AdminContentBlock = {
  id: string;
  blockType: string;
  order: number;
  configJson: string;
  lang: string;
};

export type AdminAttachment = {
  id: string;
  mediaAssetId: string;
  displayName: string;
  url: string;
};

export type AdminLessonFull = {
  id: string;
  courseId: string;
  moduleId: string | null;
  order: number;
  title: string;
  summary: string;
  estimatedMinutes: number;
  codeEditorEnabled: boolean;
  state: string;
  hasQuiz: boolean;
  contentBlocks: AdminContentBlock[];
  attachments: AdminAttachment[];
};

export type ContentBlockInput = {
  blockType: string;
  order: number;
  configJson: string;
};

export type SetContentBlocksPayload = { blocks: ContentBlockInput[] };
export type AddAttachmentPayload = { mediaAssetId: string; displayName: string };

export type CourseImportTranslation = { lang: string; title: string; shortDescription?: string; fullDescription?: string };
export type ModuleImportTranslation = { lang: string; title: string; description?: string };
export type LessonImportTranslation = { lang: string; title: string; summary: string };
export type AnswerImport = { order: number; text: string; isCorrect: boolean };
export type QuestionImport = { order: number; text: string; type: string; points: number; answers: AnswerImport[] };
export type QuizImport = { title: string; passingScore: number; maxAttempts: number; shuffleQuestions: boolean; shuffleAnswers: boolean; questions: QuestionImport[]; published?: boolean };
export type ContentBlockImport = { lang: string; order: number; blockType: string; configJson: string };
export type LessonImport = { order: number; title: string; summary: string; estimatedMinutes: number; codeEditorEnabled: boolean; translations: LessonImportTranslation[]; contentBlocks: ContentBlockImport[]; quiz: QuizImport | null; published?: boolean };
export type ModuleImport = { order: number; title: string; description?: string; translations: ModuleImportTranslation[]; lessons: LessonImport[]; published?: boolean };
export type CourseImportPayload = { title: string; shortDescription?: string; fullDescription?: string; level: string; translations: CourseImportTranslation[]; modules: ModuleImport[]; standaloneLessons: LessonImport[]; published?: boolean };
export type CourseImportResult = { courseId: string; title: string; modulesImported: number; lessonsImported: number; quizzesImported: number };
export type AddAttachmentLinkPayload = { externalUrl: string; displayName: string };

export type EntityTranslation = {
  lang: string;
  title: string;
  field2: string | null;
  field3: string | null;
};

export type UpsertCourseTranslationPayload = { title: string; shortDescription?: string; fullDescription?: string };
export type UpsertModuleTranslationPayload = { title: string; description?: string };
export type UpsertLessonTranslationPayload = { title: string; summary: string };

export type UserProfile = {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  preferredLanguage: number;
};

export type UpdateProfilePayload = {
  firstName: string;
  lastName: string;
  preferredLanguage: number;
};
