import { Route, Routes } from "react-router-dom";
import { MainLayout } from "../shared/layouts/MainLayout";
import { ProtectedRoute } from "../shared/components/ProtectedRoute";
import { HomePage } from "../pages/HomePage";
import { CoursesPage } from "../pages/CoursesPage";
import { CourseDetailsPage } from "../pages/CourseDetailsPage";
import { LessonPage } from "../pages/LessonPage";
import { LoginPage } from "../pages/LoginPage";
import { RegisterPage } from "../pages/RegisterPage";
import { AdminPage } from "../pages/AdminPage";
import { StudentDashboardPage } from "../pages/StudentDashboardPage";
import { ProfilePage } from "../pages/ProfilePage";
import { MyCoursesPage } from "../pages/MyCoursesPage";

export default function App() {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route index element={<HomePage />} />
        <Route path="/courses" element={<CoursesPage />} />
        <Route path="/courses/:slug" element={<CourseDetailsPage />} />
        <Route path="/lessons/:lessonId" element={<LessonPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        <Route element={<ProtectedRoute />}>
          <Route path="/student" element={<StudentDashboardPage />} />
          <Route path="/my-courses" element={<MyCoursesPage />} />
          <Route path="/admin" element={<AdminPage />} />
          <Route path="/profile" element={<ProfilePage />} />
        </Route>
      </Route>
    </Routes>
  );
}
