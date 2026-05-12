import { useEffect, useMemo, useRef, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuth } from "../features/auth/AuthContext";
import { brightEduApi } from "../shared/api/brightEduApi";
import { ErrorPanel } from "../shared/components/ErrorPanel";
import { LoadingPanel } from "../shared/components/LoadingPanel";
import Editor from "@monaco-editor/react";
import type {
  AttemptQuestionResult,
  LessonContentBlock,
  LessonDetails,
  QuizHistoryItem,
  QuizQuestion,
  SubmitQuizAttemptResult
} from "../shared/types/api";

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5164";

export function LessonPage() {
  const { lessonId } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const { t, i18n } = useTranslation();
  const [lesson, setLesson] = useState<LessonDetails | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [attemptMessage, setAttemptMessage] = useState<string | null>(null);
  const [currentAttemptId, setCurrentAttemptId] = useState<string | null>(null);
  const [selectedAnswers, setSelectedAnswers] = useState<Record<string, string>>({});
  const [history, setHistory] = useState<QuizHistoryItem[]>([]);
  const [submitResult, setSubmitResult] = useState<SubmitQuizAttemptResult | null>(null);
  const [isQuizMode, setIsQuizMode] = useState(false);
  const [isLessonCompleted, setIsLessonCompleted] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isEnrolled, setIsEnrolled] = useState(false);
  const [isStartingAttempt, setIsStartingAttempt] = useState(false);
  const [isSubmittingAttempt, setIsSubmittingAttempt] = useState(false);

  useEffect(() => {
    if (!lessonId) {
      setError(t("lesson.missingId"));
      setIsLoading(false);
      return;
    }

    const load = async () => {
      try {
        const lessonResponse = await brightEduApi.getLessonById(lessonId, i18n.language);
        setLesson(lessonResponse);
        if (user?.accessToken) {
          const [progress, enrollStatus] = await Promise.all([
            brightEduApi.markLessonOpened(lessonId, user.accessToken),
            lessonResponse.courseId
              ? brightEduApi.getEnrollmentStatus(lessonResponse.courseId, user.accessToken).catch(() => null)
              : null,
          ]);
          setIsLessonCompleted(progress.isCompleted);
          setIsEnrolled(enrollStatus?.isEnrolled ?? false);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : t("lesson.loadError"));
      } finally {
        setIsLoading(false);
      }
    };

    void load();
  }, [lessonId, user?.accessToken, i18n.language]);

  useEffect(() => {
    const quizId = lesson?.quizId;
    const token = user?.accessToken;

    if (!quizId || !token) {
      return;
    }

    const loadHistory = async () => {
      try {
        setHistory(await brightEduApi.getQuizHistory(quizId, token));
      } catch {
        // Ignore transient history failures.
      }
    };

    void loadHistory();
  }, [lesson?.quizId, user?.accessToken]);

  const parsedBlocks = useMemo(() => {
    return (lesson?.contentBlocks ?? []).map((block) => ({
      ...block,
      parsed: parseConfig(block)
    }));
  }, [lesson]);

  const handleStartQuiz = async () => {
    if (!lesson?.quizId) {
      return;
    }

    if (!user?.accessToken) {
      navigate("/login");
      return;
    }

    setIsStartingAttempt(true);
    setAttemptMessage(null);
    setSubmitResult(null);
    try {
      const attempt = await brightEduApi.startQuizAttempt(lesson.quizId, user.accessToken);
      setCurrentAttemptId(attempt.attemptId);
      setIsQuizMode(true);
      setAttemptMessage(t("lesson.attemptStarted", { number: attempt.attemptNumber }));
      setHistory(await brightEduApi.getQuizHistory(lesson.quizId, user.accessToken));
    } catch (err) {
      setError(err instanceof Error ? err.message : t("lesson.startQuizError"));
    } finally {
      setIsStartingAttempt(false);
    }
  };

  const handleSelectOption = (questionId: string, optionId: string) => {
    setSelectedAnswers((current) => ({
      ...current,
      [questionId]: optionId
    }));
  };

  const handleSubmitQuiz = async () => {
    if (!lesson?.quiz || !lesson.quizId || !currentAttemptId || !user?.accessToken) {
      return;
    }

    setIsSubmittingAttempt(true);
    setError(null);
    try {
      const result = await brightEduApi.submitQuizAttempt(
        currentAttemptId,
        {
          attemptId: currentAttemptId,
          answers: lesson.quiz.questions.map((question) => ({
            questionId: question.id,
            selectedOptionId: selectedAnswers[question.id] ?? null,
            textAnswer: null
          }))
        },
        user.accessToken
      );

      setSubmitResult(result);
      setAttemptMessage(null);
      setCurrentAttemptId(null);
      setHistory(await brightEduApi.getQuizHistory(lesson.quizId, user.accessToken));
    } catch (err) {
      setError(err instanceof Error ? err.message : t("lesson.submitQuizError"));
    } finally {
      setIsSubmittingAttempt(false);
    }
  };

  const handleBackToTheory = () => {
    setIsQuizMode(false);
    setAttemptMessage(null);
    setSubmitResult(null);
    setCurrentAttemptId(null);
  };

  const handleMarkLessonCompleted = async () => {
    if (!lesson || !user?.accessToken) {
      navigate("/login");
      return;
    }

    try {
      const progress = await brightEduApi.updateLessonProgress(lesson.id, true, user.accessToken);
      setIsLessonCompleted(progress.isCompleted);
      setToastMessage(t("lesson.completedMessage"));
      window.setTimeout(() => setToastMessage(null), 3000);
    } catch (err) {
      setError(err instanceof Error ? err.message : t("lesson.updateProgressError"));
    }
  };

  const isFirstLesson = lesson?.order === 1;
  const canViewContent = isEnrolled || isFirstLesson;

  return (
    <section className="section">
      {isLoading ? <LoadingPanel text={t("lesson.loading")} /> : null}
      {error ? <ErrorPanel message={error} /> : null}
      {!isLoading && !error && lesson ? (
        <>
          <div className="page-title">
            {lesson.courseSlug && (
              <Link to={`/courses/${lesson.courseSlug}`} className="btn-secondary" style={{ alignSelf: "flex-start", marginBottom: "0.75rem", fontSize: "0.875rem" }}>
                {t("lesson.backToCourse")}
              </Link>
            )}
            <span className="eyebrow">{t("lesson.eyebrow")}</span>
            <h1>{lesson.title}</h1>
            <p className="muted">{lesson.summary}</p>
          </div>

          {/* ── Enrollment gate: lecție blocată ── */}
          {!canViewContent ? (
            <div className="panel" style={{ textAlign: "center", padding: "3rem 2rem" }}>
              <div style={{ fontSize: "3rem", marginBottom: "1rem" }}>🔒</div>
              <h2 style={{ marginBottom: "0.5rem" }}>{t("lesson.gateTitle")}</h2>
              <p className="muted" style={{ maxWidth: 420, margin: "0 auto 1.5rem" }}>{t("lesson.gateText")}</p>
              {lesson.courseSlug && (
                <Link to={`/courses/${lesson.courseSlug}`} className="btn-primary" style={{ textDecoration: "none" }}>
                  {t("lesson.gateCta")}
                </Link>
              )}
            </div>
          ) : null}

          {canViewContent && !isQuizMode ? (
            <div className="panel">
              <h3>{t("lesson.theory")}</h3>
              <div className="pill-row">
                <span className="pill">{lesson.estimatedMinutes} {t("lesson.minutes")}</span>
                <span className="pill">{lesson.state}</span>
                {lesson.codeEditorEnabled ? <span className="pill">{t("lesson.codeEditorEnabled")}</span> : null}
              </div>
              <div className="content-stack">
                {parsedBlocks.map((block) => (
                  <ContentBlock key={block.id} blockType={block.blockType} parsed={block.parsed} />
                ))}
              </div>
              {lesson.attachments.length > 0 ? (
                <>
                  <h3>{t("lesson.attachments")}</h3>
                  <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem", marginTop: "0.5rem" }}>
                    {lesson.attachments.map((attachment) => (
                      <a
                        key={attachment.id}
                        href={attachment.url.startsWith("http") ? attachment.url : `${API_BASE}${attachment.url}`}
                        target="_blank"
                        rel="noreferrer"
                        style={{ display: "inline-flex", alignItems: "center", gap: "0.5rem", color: "var(--accent-strong)", textDecoration: "none", fontWeight: 500 }}
                      >
                        📎 {attachment.displayName}
                      </a>
                    ))}
                  </div>
                </>
              ) : null}
              <div className="actions">
                {isEnrolled && (
                  isLessonCompleted ? (
                    <span className="completion-badge">{t("lesson.completedBadge")}</span>
                  ) : (
                    <button className="btn-secondary" type="button" onClick={handleMarkLessonCompleted}>
                      {t("lesson.markCompleted")}
                    </button>
                  )
                )}
                {isEnrolled && lesson.quizId ? (
                  <button className="btn-primary" type="button" onClick={handleStartQuiz} disabled={isStartingAttempt}>
                    {isStartingAttempt ? t("lesson.startingQuiz") : t("lesson.startQuiz")}
                  </button>
                ) : isEnrolled ? (
                  <span className="muted">{t("lesson.noQuizPublished")}</span>
                ) : null}
              </div>

              {/* Preview banner — vizibil doar pentru prima lecție când nu ești abonat */}
              {!isEnrolled && isFirstLesson && lesson.courseSlug && (
                <div style={{
                  marginTop: "1.5rem",
                  padding: "1.25rem 1.5rem",
                  borderRadius: 14,
                  background: "linear-gradient(135deg, rgba(144,70,207,0.12), rgba(99,36,175,0.08))",
                  border: "1px solid rgba(144,70,207,0.3)",
                  display: "flex",
                  alignItems: "center",
                  gap: "1.25rem",
                  flexWrap: "wrap",
                }}>
                  <div style={{ flex: 1, minWidth: 200 }}>
                    <p style={{ fontWeight: 700, marginBottom: "0.25rem" }}>{t("lesson.previewTitle")}</p>
                    <p className="muted" style={{ fontSize: "0.88rem", margin: 0 }}>{t("lesson.previewText")}</p>
                  </div>
                  <Link to={`/courses/${lesson.courseSlug}`} className="btn-primary" style={{ textDecoration: "none", flexShrink: 0 }}>
                    {t("lesson.previewCta")}
                  </Link>
                </div>
              )}
            </div>
          ) : null}

          {isEnrolled && isQuizMode && lesson.quiz ? (
            <div className="panel quiz-panel">
              <h3>{lesson.quiz.title}</h3>
              <p className="muted">
                {t("lesson.passingScore", { score: lesson.quiz.passingScore })} {t("lesson.maxAttempts", { count: lesson.quiz.maxAttempts })}
              </p>

              <div className="quiz-questions">
                {lesson.quiz.questions.map((question, index) => (
                  <QuizQuestionCard
                    key={question.id}
                    question={question}
                    displayIndex={index + 1}
                    selectedOptionId={selectedAnswers[question.id] ?? null}
                    disabled={!currentAttemptId || isSubmittingAttempt}
                    onSelect={handleSelectOption}
                  />
                ))}
              </div>

              <div className="actions">
                <button className="btn-secondary" type="button" onClick={handleBackToTheory}>
                  {t("lesson.backToTheory")}
                </button>
                <button
                  className="btn-primary"
                  type="button"
                  onClick={handleSubmitQuiz}
                  disabled={!currentAttemptId || isSubmittingAttempt}
                >
                  {isSubmittingAttempt ? t("lesson.submitting") : t("lesson.submitQuiz")}
                </button>
              </div>
              {attemptMessage ? <p className="success-text">{attemptMessage}</p> : null}
              {submitResult ? (
                <div className="panel result-panel">
                  <h3>{t("lesson.quizResult")}</h3>
                  <p className="muted">{submitResult.message}</p>
                  <div className="pill-row">
                    <span className="pill">{t("lesson.score", { score: submitResult.score })}</span>
                    <span className="pill">
                      {t("lesson.correctAnswers", { correct: submitResult.correctAnswers, total: submitResult.totalQuestions })}
                    </span>
                    <span className="pill">{submitResult.passed ? t("lesson.passed") : t("lesson.failed")}</span>
                  </div>
                </div>
              ) : null}
            </div>
          ) : null}

          {isEnrolled && isQuizMode && lesson.quizId && user?.accessToken ? (
            <div className="panel">
              <h3>{t("lesson.attemptHistory")}</h3>
              {history.length === 0 ? (
                <p className="muted">{t("lesson.noAttempts")}</p>
              ) : (
                <div className="history-list">
                  {history.map((item) => (
                    <article key={item.attemptId} className="content-block">
                      <strong>{t("lesson.attemptNumber", { number: item.attemptNumber })}</strong>
                      <p className="muted">
                        {t("lesson.attemptInfo", {
                          score: item.score ?? 0,
                          status: item.passed ? t("lesson.passed") : t("lesson.failed"),
                          date: new Date(item.startedAt).toLocaleString()
                        })}
                      </p>
                      {item.questionResults && item.questionResults.length > 0 && (
                        <AttemptReview results={item.questionResults} />
                      )}
                    </article>
                  ))}
                </div>
              )}
            </div>
          ) : null}
        </>
      ) : null}
      {toastMessage ? <div className="toast">{toastMessage}</div> : null}
    </section>
  );
}

function parseConfig(block: LessonContentBlock) {
  try {
    return JSON.parse(block.configJson) as Record<string, string>;
  } catch {
    return { raw: block.configJson };
  }
}

// ─── YouTube URL helpers ──────────────────────────────────────────────────────

function extractYoutubeId(url: string): string | null {
  const patterns = [
    /[?&]v=([a-zA-Z0-9_-]{11})/,
    /youtu\.be\/([a-zA-Z0-9_-]{11})/,
    /embed\/([a-zA-Z0-9_-]{11})/
  ];
  for (const p of patterns) {
    const m = url.match(p);
    if (m) return m[1];
  }
  return null;
}

function extractVimeoId(url: string): string | null {
  const m = url.match(/vimeo\.com\/(\d+)/);
  return m ? m[1] : null;
}

// ─── Block renderers ──────────────────────────────────────────────────────────

function TextBlock({ content }: { content: string }) {
  return (
    <div style={{ lineHeight: 1.7, whiteSpace: "pre-wrap", wordBreak: "break-word" }}>
      {content}
    </div>
  );
}

function ImageBlock({ url, alt }: { url: string; alt?: string }) {
  const { t } = useTranslation();
  if (!url) return <p className="muted">{t("lesson.imageNoUrl")}</p>;
  const src = url.startsWith("http") ? url : `${API_BASE}${url}`;
  return (
    <img
      src={src}
      alt={alt ?? ""}
      style={{ maxWidth: "100%", borderRadius: 12, display: "block" }}
    />
  );
}

function VideoBlock({ url, title }: { url: string; title?: string }) {
  const { t } = useTranslation();
  if (!url) return <p className="muted">{t("lesson.videoNoUrl")}</p>;

  const youtubeId = extractYoutubeId(url);
  if (youtubeId) {
    return (
      <div>
        {title && <p style={{ fontWeight: 600, marginBottom: "0.5rem" }}>{title}</p>}
        <div style={{ position: "relative", paddingBottom: "56.25%", height: 0, borderRadius: 12, overflow: "hidden" }}>
          <iframe
            src={`https://www.youtube.com/embed/${youtubeId}`}
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowFullScreen
            style={{ position: "absolute", inset: 0, width: "100%", height: "100%", border: "none" }}
          />
        </div>
      </div>
    );
  }

  const vimeoId = extractVimeoId(url);
  if (vimeoId) {
    return (
      <div>
        {title && <p style={{ fontWeight: 600, marginBottom: "0.5rem" }}>{title}</p>}
        <div style={{ position: "relative", paddingBottom: "56.25%", height: 0, borderRadius: 12, overflow: "hidden" }}>
          <iframe
            src={`https://player.vimeo.com/video/${vimeoId}`}
            allowFullScreen
            style={{ position: "absolute", inset: 0, width: "100%", height: "100%", border: "none" }}
          />
        </div>
      </div>
    );
  }

  if (url.match(/\.(mp4|webm|ogg)$/i)) {
    return (
      <div>
        {title && <p style={{ fontWeight: 600, marginBottom: "0.5rem" }}>{title}</p>}
        <video controls style={{ width: "100%", borderRadius: 12 }}>
          <source src={url.startsWith("http") ? url : `${API_BASE}${url}`} />
        </video>
      </div>
    );
  }

  return (
    <div>
      {title && <p style={{ fontWeight: 600, marginBottom: "0.25rem" }}>{title}</p>}
      <a href={url} target="_blank" rel="noreferrer" style={{ color: "var(--accent-strong)", wordBreak: "break-all" }}>
        🎬 {url}
      </a>
    </div>
  );
}

function PdfBlock({ url, title }: { url: string; title?: string }) {
  const { t } = useTranslation();
  if (!url) return <p className="muted">{t("lesson.pdfNoUrl")}</p>;
  const src = url.startsWith("http") ? url : `${API_BASE}${url}`;
  return (
    <div style={{ display: "flex", alignItems: "center", gap: "0.75rem", padding: "0.75rem 1rem", background: "var(--card-bg)", border: "1px solid var(--line)", borderRadius: 10 }}>
      <span style={{ fontSize: "1.5rem" }}>📄</span>
      <span style={{ fontWeight: 600, flex: 1, fontSize: "0.95rem" }}>{title ?? "PDF"}</span>
      <a href={src} target="_blank" rel="noreferrer" className="btn-secondary" style={{ fontSize: "0.82rem", textDecoration: "none", padding: "0.25rem 0.75rem" }}>
        {t("lesson.openInNewTab")}
      </a>
      <a href={src} download className="btn-primary" style={{ fontSize: "0.82rem", textDecoration: "none", padding: "0.25rem 0.75rem" }}>
        {t("lesson.download")}
      </a>
    </div>
  );
}

// Languages run directly in the browser — no external API needed
const BROWSER_RUN_LANGS = new Set(["html", "javascript"]);

// Playground links for languages that can't run in-browser
const PLAYGROUND_LINKS: Record<string, { label: string; url: string }> = {
  csharp: { label: ".NET Fiddle", url: "https://dotnetfiddle.net" },
  typescript: { label: "TypeScript Playground", url: "https://www.typescriptlang.org/play" },
};

// Pyodide loader — cached promise so we load at most once
let pyodidePromise: Promise<unknown> | null = null;
function loadPyodide(): Promise<unknown> {
  if (pyodidePromise) return pyodidePromise;
  pyodidePromise = new Promise((resolve, reject) => {
    const existing = document.getElementById("pyodide-script");
    const load = () => {
      const loader = (window as unknown as { loadPyodide: (opts: Record<string, unknown>) => Promise<unknown> }).loadPyodide;
      loader({ indexURL: "https://cdn.jsdelivr.net/pyodide/v0.26.4/full/" }).then(resolve).catch(reject);
    };
    if (existing) { load(); return; }
    const script = document.createElement("script");
    script.id = "pyodide-script";
    script.src = "https://cdn.jsdelivr.net/pyodide/v0.26.4/full/pyodide.js";
    script.onload = load;
    script.onerror = () => reject(new Error("Nu s-a putut încărca Pyodide."));
    document.head.appendChild(script);
  });
  return pyodidePromise;
}

async function runPython(code: string, noOutput: string): Promise<string> {
  const pyodide = await loadPyodide() as {
    runPythonAsync: (code: string) => Promise<unknown>;
  };
  // Capture stdout by redirecting sys.stdout before running user code
  const harness = `
import sys, io as _io, textwrap as _tw
_buf = _io.StringIO()
sys.stdout = _buf
try:
    exec(_tw.dedent(${JSON.stringify(code)}), {})
except Exception as _e:
    print(f"Error: {_e}")
finally:
    sys.stdout = sys.__stdout__
_buf.getvalue()
`;
  const result = await pyodide.runPythonAsync(harness);
  return String(result ?? "").trim() || noOutput;
}

// Run JS in a sandboxed iframe and capture console.log output via postMessage
function runJsInBrowser(code: string, noOutput: string, timeout: string, onOutput: (out: string) => void) {
  const html = `<!DOCTYPE html><html><body><script>
    (function(){
      var logs=[];
      var _log=console.log.bind(console);
      console.log=function(){var a=Array.from(arguments).map(String);logs.push(a.join(' '));_log.apply(console,arguments);};
      console.error=function(){var a=Array.from(arguments).map(String);logs.push('Error: '+a.join(' '));};
      try{ ${code} }catch(e){logs.push('Uncaught: '+e.message);}
      window.parent.postMessage({type:'js-result',logs:logs},'*');
    })();
  <\/script></body></html>`;
  const blob = new Blob([html], { type: "text/html" });
  const url = URL.createObjectURL(blob);
  const iframe = document.createElement("iframe");
  iframe.style.display = "none";
  iframe.sandbox.add("allow-scripts");
  iframe.src = url;

  const handler = (e: MessageEvent) => {
    if (e.data?.type === "js-result") {
      window.removeEventListener("message", handler);
      document.body.removeChild(iframe);
      URL.revokeObjectURL(url);
      onOutput((e.data.logs as string[]).join("\n") || noOutput);
    }
  };
  window.addEventListener("message", handler);
  document.body.appendChild(iframe);
  // Timeout safety — if iframe never responds
  setTimeout(() => {
    if (document.body.contains(iframe)) {
      window.removeEventListener("message", handler);
      document.body.removeChild(iframe);
      URL.revokeObjectURL(url);
      onOutput(timeout);
    }
  }, 5000);
}

function CodeEditorBlock({ language, starterCode, readOnly = false }: { language: string; starterCode: string; readOnly?: boolean }) {
  const { t } = useTranslation();
  const [code, setCode] = useState(starterCode);
  const [output, setOutput] = useState<string | null>(null);
  const [running, setRunning] = useState(false);
  const iframeRef = useRef<HTMLIFrameElement>(null);

  const isHtml = language === "html";
  const isJs = language === "javascript";
  const isPython = language === "python";
  const runsInBrowser = BROWSER_RUN_LANGS.has(language) || isPython;
  const playground = PLAYGROUND_LINKS[language];

  const noOutput = t("lesson.noOutput");
  const timeoutMsg = t("lesson.timeout");

  const runCode = async () => {
    setRunning(true);
    setOutput(null);
    try {
      // HTML — write directly into preview iframe
      if (isHtml && iframeRef.current) {
        const doc = iframeRef.current.contentDocument;
        if (doc) { doc.open(); doc.write(code); doc.close(); }
        setRunning(false);
        return;
      }
      // JavaScript — run in sandboxed iframe, capture console output
      if (isJs) {
        runJsInBrowser(code, noOutput, timeoutMsg, out => { setOutput(out); setRunning(false); });
        return;
      }
      // Python — run via Pyodide (WASM, no server needed)
      if (isPython) {
        const out = await runPython(code, noOutput);
        setOutput(out);
        return;
      }
    } catch (e) {
      setOutput(`Error: ${e instanceof Error ? e.message : String(e)}`);
    } finally {
      if (!isJs) setRunning(false);
    }
  };

  const lineCount = code.split("\n").length;
  const editorHeight = Math.max(80, Math.min(640, lineCount * 21 + 20));

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "0.75rem" }}>
      <div style={{ border: "1px solid var(--line)", borderRadius: 12, overflow: "hidden" }}>
        <Editor
          height={`${editorHeight}px`}
          language={language}
          value={code}
          onChange={readOnly ? undefined : v => setCode(v ?? "")}
          theme="vs-dark"
          options={{
            minimap: { enabled: false },
            fontSize: 14,
            scrollBeyondLastLine: false,
            automaticLayout: true,
            readOnly,
            domReadOnly: readOnly,
          }}
        />
      </div>
      {!readOnly && (
        <div style={{ display: "flex", gap: "0.75rem", alignItems: "center", flexWrap: "wrap" }}>
          {runsInBrowser ? (
            <button className="btn-primary" onClick={() => void runCode()} disabled={running} style={{ minWidth: 120 }}>
              {running ? t("lesson.running") : t("lesson.run")}
            </button>
          ) : playground ? (
            <>
              <button
                className="btn-secondary"
                onClick={() => { void navigator.clipboard.writeText(code); }}
                style={{ minWidth: 120 }}
              >
                {t("lesson.copyCode")}
              </button>
              <a href={playground.url} target="_blank" rel="noreferrer" className="btn-primary" style={{ minWidth: 140, textDecoration: "none", textAlign: "center" }}>
                {t("lesson.openPlayground", { label: playground.label })}
              </a>
            </>
          ) : null}
          <span className="muted" style={{ fontSize: "0.8rem" }}>{language}</span>
        </div>
      )}
      {readOnly && (
        <div style={{ display: "flex", gap: "0.75rem", alignItems: "center" }}>
          <button
            className="btn-secondary"
            onClick={() => { void navigator.clipboard.writeText(code); }}
            style={{ minWidth: 120, fontSize: "0.88rem" }}
          >
            {t("lesson.copyCode")}
          </button>
          <span className="muted" style={{ fontSize: "0.8rem" }}>{language}</span>
        </div>
      )}
      {isHtml && (
        <div>
          <p className="muted" style={{ fontSize: "0.8rem", marginBottom: "0.4rem" }}>{t("lesson.previewHtml")}</p>
          <iframe
            ref={iframeRef}
            style={{ width: "100%", height: 240, border: "1px solid var(--line)", borderRadius: 12 }}
            sandbox="allow-scripts"
            title="html-preview"
          />
        </div>
      )}
      {output !== null && (
        <pre style={{
          background: "#1e1e1e", color: "#d4d4d4", padding: "0.85rem 1rem",
          borderRadius: 12, fontSize: "0.85rem", overflowX: "auto", whiteSpace: "pre-wrap", margin: 0
        }}>
          {output}
        </pre>
      )}
    </div>
  );
}

function ContentBlock({ blockType, parsed }: { blockType: string; parsed: Record<string, string> }) {
  switch (blockType) {
    case "Text":
      return <TextBlock content={parsed.content ?? ""} />;
    case "Image":
      return <ImageBlock url={parsed.url ?? ""} alt={parsed.alt} />;
    case "Video":
      return <VideoBlock url={parsed.url ?? ""} title={parsed.title} />;
    case "PdfEmbed":
      return <PdfBlock url={parsed.url ?? ""} title={parsed.title} />;
    case "CodeEditor":
      return <CodeEditorBlock language={parsed.language ?? "javascript"} starterCode={parsed.starterCode ?? ""} readOnly={parsed.readOnly === "true"} />;
    default:
      return null;
  }
}

function AttemptReview({ results }: { results: AttemptQuestionResult[] }) {
  const { t } = useTranslation();
  return (
    <div style={{ marginTop: "0.75rem", display: "flex", flexDirection: "column", gap: "0.5rem" }}>
      <span style={{ fontWeight: 600, fontSize: "0.85rem", opacity: 0.7 }}>{t("lesson.reviewMistakes")}</span>
      {results.map((r, i) => (
        <div key={r.questionId} style={{
          padding: "0.6rem 0.85rem",
          borderRadius: 8,
          background: r.isCorrect ? "rgba(34,197,94,0.08)" : "rgba(239,68,68,0.08)",
          borderLeft: `3px solid ${r.isCorrect ? "#22c55e" : "#ef4444"}`
        }}>
          <div style={{ fontWeight: 600, fontSize: "0.875rem", marginBottom: "0.25rem" }}>
            <span style={{ color: r.isCorrect ? "#22c55e" : "#ef4444", marginRight: "0.4rem" }}>
              {r.isCorrect ? "✓" : "✗"}
            </span>
            {i + 1}. {r.questionText}
          </div>
          {!r.isCorrect && (
            <div style={{ fontSize: "0.82rem", display: "flex", flexDirection: "column", gap: "0.15rem", marginTop: "0.2rem" }}>
              <span style={{ opacity: 0.8 }}>
                {t("lesson.yourAnswer")}: <em>{r.selectedOptionText ?? t("lesson.noAnswerGiven")}</em>
              </span>
              {r.correctOptionText !== null && r.correctOptionText !== undefined && (
                <span style={{ color: "#22c55e" }}>
                  {t("lesson.correctAnswer")}: <em>{r.correctOptionText}</em>
                </span>
              )}
            </div>
          )}
        </div>
      ))}
    </div>
  );
}

function QuizQuestionCard({
  question,
  displayIndex,
  selectedOptionId,
  disabled,
  onSelect
}: {
  question: QuizQuestion;
  displayIndex: number;
  selectedOptionId: string | null;
  disabled: boolean;
  onSelect: (questionId: string, optionId: string) => void;
}) {
  return (
    <article className="content-block">
      <strong>
        {displayIndex}. {question.text}
      </strong>
      <div className="option-list">
        {question.options.map((option) => (
          <label key={option.id} className="option-item">
            <input
              type="radio"
              name={question.id}
              checked={selectedOptionId === option.id}
              onChange={() => onSelect(question.id, option.id)}
              disabled={disabled}
            />
            <span>{option.text}</span>
          </label>
        ))}
      </div>
    </article>
  );
}
