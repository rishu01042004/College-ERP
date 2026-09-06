import { useEffect, useState } from "react";
import { AuthProvider, useAuth } from "./context/AuthContext.jsx";
import { ToastProvider } from "./context/ToastContext.jsx";
import { ROLE_PAGES, MODULES } from "./config/modules.js";

import LoginPage from "./components/LoginPage.jsx";
import Layout from "./components/Layout.jsx";
import DashboardPage from "./pages/DashboardPage.jsx";
import GenericCrudPage from "./pages/GenericCrudPage.jsx";
import NotificationsPage from "./pages/NotificationsPage.jsx";
import HodAuthorizationPage from "./pages/HodAuthorizationPage.jsx";
import SettingsPage from "./pages/SettingsPage.jsx";

function Application() {
  const { user, checking } = useAuth();
  const [page, setPage] = useState("dashboard");

  useEffect(() => {
    if (!user) return;

    const allowedPages = ROLE_PAGES[user.role] || [];

    if (!allowedPages.includes(page)) {
      setPage("dashboard");
    }
  }, [user, page]);

  if (checking) {
    return (
      <div className="boot-screen">
        <div className="boot-logo">C</div>
        <p>Connecting to CollegeERP API...</p>
      </div>
    );
  }

  if (!user) {
    return <LoginPage />;
  }

  function renderPage() {
    if (page === "dashboard") {
      return <DashboardPage onNavigate={setPage} />;
    }

    if (page === "notifications") {
      return <NotificationsPage />;
    }

    if (page === "hodAuth") {
      return <HodAuthorizationPage />;
    }

    if (page === "settings") {
      return <SettingsPage />;
    }

    if (MODULES[page]) {
      return <GenericCrudPage moduleId={page} />;
    }

    return (
      <div className="page-header">
        <h1>Module not found</h1>
        <p>No configuration exists for {page}.</p>
      </div>
    );
  }

  return (
    <Layout
      currentPage={page}
      onNavigate={setPage}
    >
      {renderPage()}
    </Layout>
  );
}

export default function App() {
  return (
    <ToastProvider>
      <AuthProvider>
        <Application />
      </AuthProvider>
    </ToastProvider>
  );
}