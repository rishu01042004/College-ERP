import { useState } from "react";
import Sidebar from "./Sidebar.jsx";
import Topbar from "./Topbar.jsx";
import { useAuth } from "../context/AuthContext.jsx";

export default function Layout({
  currentPage,
  onNavigate,
  pageContent,
  children,
}) {
  const { user } = useAuth();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  function handleNavigate(page) {
    onNavigate(page);
    setSidebarOpen(false);
  }

  return (
    <div className="app-layout">
      <Sidebar
        user={user}
        currentPage={currentPage}
        onNavigate={handleNavigate}
        open={sidebarOpen}
      />

      {sidebarOpen && (
        <button
          type="button"
          className="mobile-overlay"
          aria-label="Close menu"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      <div className="main-area">
        <Topbar
          page={currentPage}
          currentPage={currentPage}
          user={user}
          onMenu={() => setSidebarOpen((value) => !value)}
          onNavigate={onNavigate}
        />

        <main className="content">
          {pageContent ?? children}
        </main>
      </div>
    </div>
  );
}