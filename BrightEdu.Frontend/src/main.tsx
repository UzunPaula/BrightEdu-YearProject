import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import App from "./app/App";
import "./shared/i18n";
import "./app/styles.css";
import { AuthProvider } from "./features/auth/AuthContext";
import { ToastProvider } from "./shared/components/Toast";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <AuthProvider>
      <BrowserRouter>
        <ToastProvider>
          <App />
        </ToastProvider>
      </BrowserRouter>
    </AuthProvider>
  </React.StrictMode>
);
